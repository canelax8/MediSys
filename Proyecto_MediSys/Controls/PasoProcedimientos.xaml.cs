using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Controls
{
    public partial class PasoProcedimientos : UserControl
    {
        // ============================================================
        // DAO
        // ============================================================

        private readonly ItemClinicoDAO itemClinicoDAO = new();
        private readonly TarifaDAO tarifaDAO = new();
        private readonly AlergiaDAO alergiaDAO = new();

        // ============================================================
        // PACIENTE
        // ============================================================

        private Paciente? pacienteActual;

        private PlanTarifario? planTarifarioActual;


        // ============================================================
        // CATÁLOGOS
        // ============================================================

        private List<ItemClinico> catalogoMedicamentos = new();
        private List<ItemClinico> catalogoMateriales = new();
        private List<ItemClinico> catalogoProcedimientos = new();
        private List<ItemClinico> catalogoLaboratorios = new();
        private List<ItemClinico> catalogoImagenes = new();


        // ============================================================
        // ITEMS SELECCIONADOS
        // ============================================================

        private ObservableCollection<EmergenciaItem> medicamentosSeleccionados
            = new();

        private ObservableCollection<EmergenciaItem> materialesSeleccionados
            = new();

        private ObservableCollection<EmergenciaItem> procedimientosSeleccionados
            = new();

        private ObservableCollection<EmergenciaItem> laboratoriosSeleccionados
            = new();

        private ObservableCollection<EmergenciaItem> imagenesSeleccionadas
            = new();


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public PasoProcedimientos()
        {
            InitializeComponent();

            ConfigurarDataGrids();

            ConfigurarEventosBusqueda();

            ConfigurarEventosBotones();
            btnVerAlergias.Click += btnVerAlergias_Click;

            CargarCatalogos();
        }


        // ============================================================
        // CARGAR PACIENTE
        // ============================================================

        public void CargarPaciente(Paciente paciente)
        {
            pacienteActual = paciente;

            CargarPlanTarifario();

            CargarAlergiasPaciente();
        }

        // ============================================================
        // CARGAR ALERTA DE ALERGIAS
        // ============================================================

        private void CargarAlergiasPaciente()
        {
            if (pacienteActual == null)
                return;

            try
            {
                List<PacienteAlergia> alergias =
                    alergiaDAO.ObtenerPorPaciente(
                        pacienteActual.IdPaciente);

                if (alergias.Count == 0)
                {
                    panelAlertaAlergias.Visibility =
                        Visibility.Collapsed;

                    txtResumenAlergias.Text = "";

                    return;
                }


                panelAlertaAlergias.Visibility =
                    Visibility.Visible;


                txtResumenAlergias.Text =
                    string.Join(
                        " • ",
                        alergias.Select(
                            a => a.AlergiaMostrar));


                btnVerAlergias.Tag = alergias;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible consultar las alergias del paciente.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        // ============================================================
        // CONFIGURAR DATAGRIDS
        // ============================================================

        private void ConfigurarDataGrids()
        {
            dgMedicamentos.ItemsSource =
                medicamentosSeleccionados;

            dgMateriales.ItemsSource =
                materialesSeleccionados;

            dgProcedimientos.ItemsSource =
                procedimientosSeleccionados;

            dgLaboratorios.ItemsSource =
                laboratoriosSeleccionados;

            dgImagenes.ItemsSource =
                imagenesSeleccionadas;
        }


        private void btnVerAlergias_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (pacienteActual == null)
                return;


            List<PacienteAlergia> alergias =
                alergiaDAO.ObtenerPorPaciente(
                    pacienteActual.IdPaciente);


            if (alergias.Count == 0)
            {
                MessageBox.Show(
                    "El paciente no tiene alergias registradas.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            string detalle =
                string.Join(
                    "\n",
                    alergias.Select(
                        a => "• " + a.AlergiaMostrar));


            MessageBox.Show(
                $"ALERGIAS DE {pacienteActual.NombreCompleto.ToUpper()}\n\n" +
                detalle,
                "Alerta de alergias",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }


        // ============================================================
        // CONFIGURAR EVENTOS DE BÚSQUEDA
        // ============================================================

        private void ConfigurarEventosBusqueda()
        {
            txtBuscarMedicamento.TextChanged +=
                txtBuscarMedicamento_TextChanged;

            txtBuscarMaterial.TextChanged +=
                txtBuscarMaterial_TextChanged;

            txtBuscarProcedimiento.TextChanged +=
                txtBuscarProcedimiento_TextChanged;

            txtBuscarLaboratorio.TextChanged +=
                txtBuscarLaboratorio_TextChanged;

            txtBuscarImagen.TextChanged +=
                txtBuscarImagen_TextChanged;
        }


        // ============================================================
        // CONFIGURAR BOTONES
        // ============================================================

        private void ConfigurarEventosBotones()
        {
            btnAgregarMedicamento.Click +=
                btnAgregarMedicamento_Click;

            btnAgregarMaterial.Click +=
                btnAgregarMaterial_Click;

            btnAgregarProcedimiento.Click +=
                btnAgregarProcedimiento_Click;

            btnAgregarLaboratorio.Click +=
                btnAgregarLaboratorio_Click;

            btnAgregarImagen.Click +=
                btnAgregarImagen_Click;


            ConfigurarBotonesQuitar();
        }


        // ============================================================
        // BOTONES QUITAR DE LOS DATAGRIDS
        // ============================================================

        private void ConfigurarBotonesQuitar()
        {
            dgMedicamentos.AddHandler(
                Button.ClickEvent,
                new RoutedEventHandler(
                    dgMedicamentos_ButtonClick));

            dgMateriales.AddHandler(
                Button.ClickEvent,
                new RoutedEventHandler(
                    dgMateriales_ButtonClick));

            dgProcedimientos.AddHandler(
                Button.ClickEvent,
                new RoutedEventHandler(
                    dgProcedimientos_ButtonClick));

            dgLaboratorios.AddHandler(
                Button.ClickEvent,
                new RoutedEventHandler(
                    dgLaboratorios_ButtonClick));

            dgImagenes.AddHandler(
                Button.ClickEvent,
                new RoutedEventHandler(
                    dgImagenes_ButtonClick));
        }


        // ============================================================
        // CARGAR CATÁLOGOS
        // ============================================================

        private void CargarCatalogos()
        {
            try
            {
                catalogoMedicamentos =
                    itemClinicoDAO.ObtenerPorTipo(
                        "Medicamento");

                catalogoMateriales =
                    itemClinicoDAO.ObtenerPorTipo(
                        "Material gastable");

                catalogoProcedimientos =
                    itemClinicoDAO.ObtenerPorTipo(
                        "Procedimiento");

                catalogoLaboratorios =
                    itemClinicoDAO.ObtenerPorTipo(
                        "Laboratorio");

                catalogoImagenes =
                    itemClinicoDAO.ObtenerPorTipo(
                        "Imagen");


                lstMedicamentos.ItemsSource =
                    catalogoMedicamentos;

                lstMateriales.ItemsSource =
                    catalogoMateriales;

                lstProcedimientos.ItemsSource =
                    catalogoProcedimientos;

                lstLaboratorios.ItemsSource =
                    catalogoLaboratorios;

                lstImagenes.ItemsSource =
                    catalogoImagenes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar los catálogos clínicos.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // PLAN TARIFARIO
        // ============================================================

        private void CargarPlanTarifario()
        {
            if (pacienteActual == null)
                return;

            try
            {
                planTarifarioActual =
                    tarifaDAO.ObtenerPlanPaciente(
                        pacienteActual);

                if (planTarifarioActual == null)
                {
                    txtPlanTarifario.Text =
                        "Sin tarifa configurada";

                    return;
                }

                txtPlanTarifario.Text =
                    planTarifarioActual.Nombre;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible determinar el plan tarifario.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // BUSCAR MEDICAMENTO
        // ============================================================

        private void txtBuscarMedicamento_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            lstMedicamentos.ItemsSource =
                FiltrarCatalogo(
                    catalogoMedicamentos,
                    txtBuscarMedicamento.Text);
        }


        // ============================================================
        // BUSCAR MATERIAL
        // ============================================================

        private void txtBuscarMaterial_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            lstMateriales.ItemsSource =
                FiltrarCatalogo(
                    catalogoMateriales,
                    txtBuscarMaterial.Text);
        }


        // ============================================================
        // BUSCAR PROCEDIMIENTO
        // ============================================================

        private void txtBuscarProcedimiento_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            lstProcedimientos.ItemsSource =
                FiltrarCatalogo(
                    catalogoProcedimientos,
                    txtBuscarProcedimiento.Text);
        }


        // ============================================================
        // BUSCAR LABORATORIO
        // ============================================================

        private void txtBuscarLaboratorio_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            lstLaboratorios.ItemsSource =
                FiltrarCatalogo(
                    catalogoLaboratorios,
                    txtBuscarLaboratorio.Text);
        }


        // ============================================================
        // BUSCAR IMAGEN
        // ============================================================

        private void txtBuscarImagen_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            lstImagenes.ItemsSource =
                FiltrarCatalogo(
                    catalogoImagenes,
                    txtBuscarImagen.Text);
        }


        // ============================================================
        // FILTRADO GENERAL
        // ============================================================

        private List<ItemClinico> FiltrarCatalogo(
            List<ItemClinico> catalogo,
            string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return catalogo;

            texto = texto
                .Trim()
                .ToLower();

            return catalogo
                .Where(i =>
                    i.Codigo
                        .ToLower()
                        .Contains(texto)
                    ||
                    i.Nombre
                        .ToLower()
                        .Contains(texto)
                    ||
                    i.Descripcion
                        .ToLower()
                        .Contains(texto)
                    ||
                    i.PrincipioActivo
                        .ToLower()
                        .Contains(texto))
                .ToList();
        }


        // ============================================================
        // AGREGAR MEDICAMENTO
        // ============================================================

        private void btnAgregarMedicamento_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (lstMedicamentos.SelectedItem
                is not ItemClinico item)
            {
                MostrarSeleccioneItem(
                    "medicamento");

                return;
            }


            if (!decimal.TryParse(
                txtCantidadMedicamento.Text,
                out decimal cantidad)
                ||
                cantidad <= 0)
            {
                MessageBox.Show(
                    "Ingrese una cantidad válida.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (ExisteItem(
                medicamentosSeleccionados,
                item.IdItemClinico))
            {
                MessageBox.Show(
                    "Este medicamento ya fue agregado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            EmergenciaItem? emergenciaItem =
                CrearEmergenciaItem(
                    item,
                    cantidad);

            if (emergenciaItem == null)
                return;


            emergenciaItem.Dosis =
                txtDosisMedicamento.Text.Trim();

            emergenciaItem.ViaAdministracion =
                ObtenerViaSeleccionada();

            emergenciaItem.Frecuencia =
                txtFrecuenciaMedicamento.Text.Trim();

            emergenciaItem.Indicaciones =
                txtIndicacionesMedicamento.Text.Trim();


            medicamentosSeleccionados.Add(
                emergenciaItem);


            LimpiarMedicamento();

            ActualizarSubtotal();
        }


        // ============================================================
        // AGREGAR MATERIAL
        // ============================================================

        private void btnAgregarMaterial_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (lstMateriales.SelectedItem
                is not ItemClinico item)
            {
                MostrarSeleccioneItem(
                    "material");

                return;
            }


            if (!decimal.TryParse(
                txtCantidadMaterial.Text,
                out decimal cantidad)
                ||
                cantidad <= 0)
            {
                MessageBox.Show(
                    "Ingrese una cantidad válida.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (ExisteItem(
                materialesSeleccionados,
                item.IdItemClinico))
            {
                MessageBox.Show(
                    "Este material ya fue agregado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            EmergenciaItem? emergenciaItem =
                CrearEmergenciaItem(
                    item,
                    cantidad);

            if (emergenciaItem == null)
                return;


            materialesSeleccionados.Add(
                emergenciaItem);


            txtCantidadMaterial.Text = "1";

            ActualizarSubtotal();
        }


        // ============================================================
        // AGREGAR PROCEDIMIENTO
        // ============================================================

        private void btnAgregarProcedimiento_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (lstProcedimientos.SelectedItem
                is not ItemClinico item)
            {
                MostrarSeleccioneItem(
                    "procedimiento");

                return;
            }


            if (!decimal.TryParse(
                txtCantidadProcedimiento.Text,
                out decimal cantidad)
                ||
                cantidad <= 0)
            {
                MessageBox.Show(
                    "Ingrese una cantidad válida.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (ExisteItem(
                procedimientosSeleccionados,
                item.IdItemClinico))
            {
                MessageBox.Show(
                    "Este procedimiento ya fue agregado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            EmergenciaItem? emergenciaItem =
                CrearEmergenciaItem(
                    item,
                    cantidad);

            if (emergenciaItem == null)
                return;


            procedimientosSeleccionados.Add(
                emergenciaItem);


            txtCantidadProcedimiento.Text =
                "1";

            ActualizarSubtotal();
        }


        // ============================================================
        // AGREGAR LABORATORIO
        // ============================================================

        private void btnAgregarLaboratorio_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (lstLaboratorios.SelectedItem
                is not ItemClinico item)
            {
                MostrarSeleccioneItem(
                    "laboratorio");

                return;
            }


            if (ExisteItem(
                laboratoriosSeleccionados,
                item.IdItemClinico))
            {
                MessageBox.Show(
                    "Este laboratorio ya fue agregado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            EmergenciaItem? emergenciaItem =
                CrearEmergenciaItem(
                    item,
                    1);

            if (emergenciaItem == null)
                return;


            laboratoriosSeleccionados.Add(
                emergenciaItem);


            ActualizarSubtotal();
        }


        // ============================================================
        // AGREGAR IMAGEN
        // ============================================================

        private void btnAgregarImagen_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (lstImagenes.SelectedItem
                is not ItemClinico item)
            {
                MostrarSeleccioneItem(
                    "estudio de imágenes");

                return;
            }


            if (ExisteItem(
                imagenesSeleccionadas,
                item.IdItemClinico))
            {
                MessageBox.Show(
                    "Este estudio ya fue agregado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            EmergenciaItem? emergenciaItem =
                CrearEmergenciaItem(
                    item,
                    1);

            if (emergenciaItem == null)
                return;


            imagenesSeleccionadas.Add(
                emergenciaItem);


            ActualizarSubtotal();
        }


        // ============================================================
        // CREAR EMERGENCIA ITEM
        // ============================================================

        private EmergenciaItem? CrearEmergenciaItem(
            ItemClinico item,
            decimal cantidad)
        {
            if (pacienteActual == null)
            {
                MessageBox.Show(
                    "No hay un paciente seleccionado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return null;
            }


            if (planTarifarioActual == null)
            {
                MessageBox.Show(
                    "No se pudo determinar el plan tarifario del paciente.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return null;
            }


            TarifaItem? tarifa =
                tarifaDAO.ObtenerTarifaVigente(
                    item.IdItemClinico,
                    planTarifarioActual.IdPlanTarifario);


            if (tarifa == null)
            {
                MessageBox.Show(
                    $"No existe una tarifa vigente para:\n\n" +
                    $"{item.Nombre}\n" +
                    $"Plan: {planTarifarioActual.Nombre}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return null;
            }


            EmergenciaItem emergenciaItem = new()
            {
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
                    planTarifarioActual.IdPlanTarifario,

                NombrePlanTarifario =
                    planTarifarioActual.Nombre,

                Estado =
                    "Registrado",

                Activo =
                    true,

                FechaRegistro =
                    DateTime.Now
            };


            return emergenciaItem;
        }


        // ============================================================
        // OBTENER VÍA
        // ============================================================

        private string ObtenerViaSeleccionada()
        {
            if (cmbViaMedicamento.SelectedItem
                is ComboBoxItem item)
            {
                return item.Content
                    ?.ToString() ?? "";
            }

            return "";
        }


        // ============================================================
        // VERIFICAR DUPLICADO
        // ============================================================

        private bool ExisteItem(
            ObservableCollection<EmergenciaItem> lista,
            long idItemClinico)
        {
            return lista.Any(
                i =>
                    i.IdItemClinico ==
                    idItemClinico);
        }


        // ============================================================
        // QUITAR MEDICAMENTO
        // ============================================================

        private void dgMedicamentos_ButtonClick(
            object sender,
            RoutedEventArgs e)
        {
            if (e.OriginalSource
                is Button boton
                &&
                boton.Tag
                is EmergenciaItem item)
            {
                medicamentosSeleccionados.Remove(
                    item);

                ActualizarSubtotal();
            }
        }


        // ============================================================
        // QUITAR MATERIAL
        // ============================================================

        private void dgMateriales_ButtonClick(
            object sender,
            RoutedEventArgs e)
        {
            if (e.OriginalSource
                is Button boton
                &&
                boton.Tag
                is EmergenciaItem item)
            {
                materialesSeleccionados.Remove(
                    item);

                ActualizarSubtotal();
            }
        }


        // ============================================================
        // QUITAR PROCEDIMIENTO
        // ============================================================

        private void dgProcedimientos_ButtonClick(
            object sender,
            RoutedEventArgs e)
        {
            if (e.OriginalSource
                is Button boton
                &&
                boton.Tag
                is EmergenciaItem item)
            {
                procedimientosSeleccionados.Remove(
                    item);

                ActualizarSubtotal();
            }
        }


        // ============================================================
        // QUITAR LABORATORIO
        // ============================================================

        private void dgLaboratorios_ButtonClick(
            object sender,
            RoutedEventArgs e)
        {
            if (e.OriginalSource
                is Button boton
                &&
                boton.Tag
                is EmergenciaItem item)
            {
                laboratoriosSeleccionados.Remove(
                    item);

                ActualizarSubtotal();
            }
        }


        // ============================================================
        // QUITAR IMAGEN
        // ============================================================

        private void dgImagenes_ButtonClick(
            object sender,
            RoutedEventArgs e)
        {
            if (e.OriginalSource
                is Button boton
                &&
                boton.Tag
                is EmergenciaItem item)
            {
                imagenesSeleccionadas.Remove(
                    item);

                ActualizarSubtotal();
            }
        }


        // ============================================================
        // SUBTOTAL
        // ============================================================

        private void ActualizarSubtotal()
        {
            decimal total = 0;


            total +=
                medicamentosSeleccionados.Sum(
                    i => i.Total);

            total +=
                materialesSeleccionados.Sum(
                    i => i.Total);

            total +=
                procedimientosSeleccionados.Sum(
                    i => i.Total);

            total +=
                laboratoriosSeleccionados.Sum(
                    i => i.Total);

            total +=
                imagenesSeleccionadas.Sum(
                    i => i.Total);


            txtSubtotalGeneral.Text =
                $"RD$ {total:N2}";
        }


        // ============================================================
        // LIMPIAR MEDICAMENTO
        // ============================================================

        private void LimpiarMedicamento()
        {
            txtDosisMedicamento.Clear();

            cmbViaMedicamento.SelectedIndex =
                -1;

            txtFrecuenciaMedicamento.Clear();

            txtIndicacionesMedicamento.Clear();

            txtCantidadMedicamento.Text =
                "1";

            lstMedicamentos.SelectedItem =
                null;
        }


        // ============================================================
        // MENSAJE GENÉRICO
        // ============================================================

        private void MostrarSeleccioneItem(
            string tipo)
        {
            MessageBox.Show(
                $"Seleccione un {tipo} del catálogo.",
                "MediSys",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }


        // ============================================================
        // VALIDAR
        // ============================================================

        public bool Validar()
        {
            if (medicamentosSeleccionados.Count == 0 &&
                materialesSeleccionados.Count == 0 &&
                procedimientosSeleccionados.Count == 0 &&
                laboratoriosSeleccionados.Count == 0 &&
                imagenesSeleccionadas.Count == 0)
            {
                MessageBox.Show(
                    "Debe registrar al menos un medicamento, material, procedimiento, laboratorio o estudio de imágenes.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }


        // ============================================================
        // OBTENER TODOS LOS ITEMS
        // ============================================================

        public List<EmergenciaItem> ObtenerItems()
        {
            List<EmergenciaItem> items = new();


            items.AddRange(
                medicamentosSeleccionados);

            items.AddRange(
                materialesSeleccionados);

            items.AddRange(
                procedimientosSeleccionados);

            items.AddRange(
                laboratoriosSeleccionados);

            items.AddRange(
                imagenesSeleccionadas);


            return items;
        }


        // ============================================================
        // COMPATIBILIDAD CON PROCEDIMIENTOEMERGENCIA ACTUAL
        // ============================================================

        public ProcedimientoEmergencia ObtenerProcedimientos()
        {
            ProcedimientoEmergencia procedimiento =
                new();


            procedimiento.Medicamentos =
                string.Join(
                    ", ",
                    medicamentosSeleccionados
                        .Select(i =>
                            i.Nombre));


            procedimiento.Procedimientos =
                string.Join(
                    ", ",
                    procedimientosSeleccionados
                        .Select(i =>
                            i.Nombre));


            procedimiento.Laboratorios =
                string.Join(
                    ", ",
                    laboratoriosSeleccionados
                        .Select(i =>
                            i.Nombre));


            procedimiento.Imagenes =
                string.Join(
                    ", ",
                    imagenesSeleccionadas
                        .Select(i =>
                            i.Nombre));


            return procedimiento;
        }
    }
}