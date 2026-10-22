using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Controls
{
    public partial class ConsumosInternamientoDialog : Window
    {
        // ============================================================
        // DAO
        // ============================================================

        private readonly InternamientoItemDAO
            internamientoItemDAO =
                new InternamientoItemDAO();


        private readonly ItemClinicoDAO
            itemClinicoDAO =
                new ItemClinicoDAO();


        private readonly TarifaDAO
            tarifaDAO =
                new TarifaDAO();


        private readonly PacienteDAO
            pacienteDAO =
                new PacienteDAO();


        // ============================================================
        // INTERNAMIENTO
        // ============================================================

        private readonly long idInternamiento;

        private readonly int idPaciente;

        private readonly string codigoInternamiento;

        private readonly string nombrePaciente;


        // ============================================================
        // PACIENTE / PLAN
        // ============================================================

        private Paciente? pacienteActual;

        private PlanTarifario? planTarifarioActual;


        // ============================================================
        // CATÁLOGO
        // ============================================================

        private List<ItemClinico> catalogoActual =
            new List<ItemClinico>();


        // ============================================================
        // CONSUMOS
        // ============================================================

        private List<InternamientoItem> consumos =
            new List<InternamientoItem>();


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public ConsumosInternamientoDialog(
            long idInternamiento,
            int idPaciente,
            string codigoInternamiento,
            string nombrePaciente)
        {
            InitializeComponent();


            this.idInternamiento =
                idInternamiento;

            this.idPaciente =
                idPaciente;

            this.codigoInternamiento =
                codigoInternamiento ?? "";

            this.nombrePaciente =
                nombrePaciente ?? "";


            Inicializar();
        }


        // ============================================================
        // INICIALIZAR
        // ============================================================

        private void Inicializar()
        {
            try
            {
                txtCodigoInternamiento.Text =
                    codigoInternamiento;


                txtPaciente.Text =
                    nombrePaciente;


                // ====================================================
                // OBTENER PACIENTE
                // ====================================================

                pacienteActual =
                    pacienteDAO
                        .ObtenerTodos()
                        .FirstOrDefault(
                            x =>
                                x.IdPaciente ==
                                idPaciente);


                if (pacienteActual == null)
                {
                    throw new Exception(
                        "No fue posible encontrar el paciente del internamiento.");
                }


                // ====================================================
                // PLAN TARIFARIO
                // ====================================================

                planTarifarioActual =
                    tarifaDAO
                        .ObtenerPlanPaciente(
                            pacienteActual);


                if (planTarifarioActual == null)
                {
                    throw new Exception(
                        "El paciente no tiene un plan tarifario configurado.");
                }


                txtPlanTarifario.Text =
                    planTarifarioActual.Nombre;


                // ====================================================
                // TIPO POR DEFECTO
                // ====================================================

                cmbTipoConsumo.SelectedIndex =
                    0;


                CargarCatalogo();

                CargarConsumos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible abrir los consumos del internamiento.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);


                Close();
            }
        }


        // ============================================================
        // OBTENER TIPO
        // ============================================================

        private string ObtenerTipoSeleccionado()
        {
            if (cmbTipoConsumo.SelectedItem
                is ComboBoxItem item)
            {
                return
                    item.Content
                        ?.ToString()
                    ?? "";
            }


            return "";
        }


        // ============================================================
        // CARGAR CATÁLOGO
        // ============================================================

        private void CargarCatalogo()
        {
            string tipo =
                ObtenerTipoSeleccionado();


            if (string.IsNullOrWhiteSpace(
                tipo))
            {
                catalogoActual.Clear();

                lstCatalogo.ItemsSource =
                    null;

                return;
            }


            catalogoActual =
                itemClinicoDAO
                    .ObtenerPorTipo(
                        tipo);


            AplicarFiltroCatalogo();
        }


        // ============================================================
        // FILTRAR CATÁLOGO
        // ============================================================

        private void AplicarFiltroCatalogo()
        {
            string texto =
                txtBuscar.Text
                    .Trim()
                    .ToLower();


            if (string.IsNullOrWhiteSpace(
                texto))
            {
                lstCatalogo.ItemsSource =
                    catalogoActual;

                return;
            }


            lstCatalogo.ItemsSource =
                catalogoActual
                    .Where(
                        x =>
                            x.Nombre
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.Codigo
                                .ToLower()
                                .Contains(texto))
                    .ToList();
        }


        // ============================================================
        // CAMBIO DE TIPO
        // ============================================================

        private void cmbTipoConsumo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;


            txtBuscar.Clear();

            CargarCatalogo();
        }


        // ============================================================
        // BUSCAR
        // ============================================================

        private void txtBuscar_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (!IsLoaded)
                return;


            AplicarFiltroCatalogo();
        }


        // ============================================================
        // AGREGAR
        // ============================================================

        private void btnAgregar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (lstCatalogo.SelectedItem
                is not ItemClinico item)
            {
                MessageBox.Show(
                    "Seleccione un servicio del catálogo.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (!decimal.TryParse(
                    txtCantidad.Text,
                    out decimal cantidad)

                ||

                cantidad <= 0)
            {
                MessageBox.Show(
                    "Ingrese una cantidad válida.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtCantidad.Focus();

                return;
            }


            if (planTarifarioActual == null)
            {
                MessageBox.Show(
                    "No se pudo determinar el plan tarifario.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            try
            {
                // ====================================================
                // TARIFA VIGENTE
                // ====================================================

                TarifaItem? tarifa =
                    tarifaDAO
                        .ObtenerTarifaVigente(
                            item.IdItemClinico,
                            planTarifarioActual
                                .IdPlanTarifario);


                if (tarifa == null)
                {
                    MessageBox.Show(
                        $"No existe una tarifa vigente para:\n\n" +
                        $"{item.Nombre}\n\n" +
                        $"Plan: {planTarifarioActual.Nombre}",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                // ====================================================
                // CREAR CONSUMO
                // ====================================================

                InternamientoItem consumo =
                    new InternamientoItem
                    {
                        IdInternamiento =
                            idInternamiento,

                        IdItemClinico =
                            item.IdItemClinico,

                        Codigo =
                            item.Codigo,

                        Nombre =
                            item.Nombre,

                        TipoItem =
                            item.TipoItem,

                        Cantidad =
                            cantidad,

                        PrecioUnitarioAplicado =
                            tarifa.Precio,

                        IdPlanTarifarioAplicado =
                            planTarifarioActual
                                .IdPlanTarifario,

                        NombrePlanTarifario =
                            planTarifarioActual.Nombre,

                        Estado =
                            "Registrado",

                        FechaRegistro =
                            DateTime.Now,

                        Activo =
                            true
                    };


                internamientoItemDAO
                    .Insertar(
                        consumo);


                // ====================================================
                // LIMPIAR
                // ====================================================

                txtCantidad.Text =
                    "1";


                txtBuscar.Clear();


                lstCatalogo.SelectedItem =
                    null;


                // ====================================================
                // RECARGAR
                // ====================================================

                CargarConsumos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible registrar el consumo.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // CARGAR CONSUMOS
        // ============================================================

        private void CargarConsumos()
        {
            consumos =
                internamientoItemDAO
                    .ObtenerPorInternamiento(
                        idInternamiento);


            dgConsumos.ItemsSource =
                null;


            dgConsumos.ItemsSource =
                consumos;


            decimal subtotal =
                consumos.Sum(
                    x => x.Total);


            txtSubtotal.Text =
                $"RD$ {subtotal:N2}";
        }


        // ============================================================
        // QUITAR
        // ============================================================

        private void btnQuitar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;


            if (boton.DataContext
                is not InternamientoItem item)
            {
                return;
            }


            MessageBoxResult respuesta =
                MessageBox.Show(
                    $"¿Desea quitar este consumo?\n\n" +
                    $"{item.Nombre}\n" +
                    $"Cantidad: {item.Cantidad:N2}",
                    "MediSys",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (respuesta !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                bool resultado =
                    internamientoItemDAO
                        .Anular(
                            item.IdInternamientoItem);


                if (!resultado)
                {
                    MessageBox.Show(
                        "No fue posible quitar el consumo.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                CargarConsumos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }


        // ============================================================
        // CERRAR
        // ============================================================

        private void btnCerrar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult =
                true;


            Close();
        }
    }
}