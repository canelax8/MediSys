using Proyecto_MediSys.Controls;
using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Pages.Procesos
{
    public partial class InternamientosPage : Page
    {
        // =========================================================
        // DAO
        // =========================================================

        private readonly InternamientoDAO dao =
            new InternamientoDAO();


        // =========================================================
        // DATOS
        // =========================================================

        private List<Internamiento> listaInternamientos =
            new List<Internamiento>();


        private bool cargandoFiltros =
            false;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public InternamientosPage()
        {
            InitializeComponent();

            CargarInternamientos();
        }


        // =========================================================
        // CARGAR
        // =========================================================

        private void CargarInternamientos()
        {
            try
            {
                listaInternamientos =
                    dao.ObtenerTodos();


                CargarFiltros();

                AplicarFiltros();

                ActualizarContadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar los internamientos.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // FILTROS
        // =========================================================

        private void CargarFiltros()
        {
            cargandoFiltros =
                true;


            try
            {
                // =================================================
                // GUARDAR SELECCIONES ACTUALES
                // =================================================

                string estadoSeleccionado =
                    cmbEstado.SelectedItem
                        ?.ToString()
                    ?? "Todos";


                string areaSeleccionada =
                    cmbArea.SelectedItem
                        ?.ToString()
                    ?? "Todas";


                // =================================================
                // ESTADOS
                // =================================================

                List<string> estados =
                    new List<string>
                    {
                        "Todos"
                    };


                estados.AddRange(
                    dao.ObtenerEstados()
                        .Select(x => x.Nombre));


                cmbEstado.ItemsSource =
                    estados;


                cmbEstado.SelectedItem =
                    estados.Contains(
                        estadoSeleccionado)

                        ? estadoSeleccionado
                        : "Todos";


                // =================================================
                // ÁREAS
                // =================================================

                List<string> areas =
                    new List<string>
                    {
                        "Todas"
                    };


                areas.AddRange(
                    dao.ObtenerAreas()
                        .Select(x => x.Nombre));


                cmbArea.ItemsSource =
                    areas;


                cmbArea.SelectedItem =
                    areas.Contains(
                        areaSeleccionada)

                        ? areaSeleccionada
                        : "Todas";
            }
            finally
            {
                cargandoFiltros =
                    false;
            }
        }


        // =========================================================
        // APLICAR FILTROS
        // =========================================================

        private void AplicarFiltros()
        {
            if (cargandoFiltros)
                return;


            IEnumerable<Internamiento> consulta =
                listaInternamientos;


            // =====================================================
            // TEXTO
            // =====================================================

            string buscar =
                txtBuscar.Text
                    ?.Trim()
                    .ToLower()
                ?? "";


            if (!string.IsNullOrWhiteSpace(
                buscar))
            {
                consulta =
                    consulta.Where(
                        x =>
                            Contiene(
                                x.CodigoInternamiento,
                                buscar)

                            ||

                            Contiene(
                                x.NombrePaciente,
                                buscar)

                            ||

                            Contiene(
                                x.CodigoPaciente,
                                buscar)

                            ||

                            Contiene(
                                x.DocumentoPaciente,
                                buscar)

                            ||

                            Contiene(
                                x.NombreMedico,
                                buscar)

                            ||

                            Contiene(
                                x.Especialidad,
                                buscar)

                            ||

                            Contiene(
                                x.Area,
                                buscar)

                            ||

                            Contiene(
                                x.Habitacion,
                                buscar)

                            ||

                            Contiene(
                                x.CodigoCama,
                                buscar));
            }


            // =====================================================
            // ESTADO
            // =====================================================

            string estado =
                cmbEstado.SelectedItem
                    ?.ToString()
                ?? "Todos";


            if (!estado.Equals(
                "Todos",
                StringComparison.OrdinalIgnoreCase))
            {
                consulta =
                    consulta.Where(
                        x =>
                            x.Estado.Equals(
                                estado,
                                StringComparison.OrdinalIgnoreCase));
            }


            // =====================================================
            // ÁREA
            // =====================================================

            string area =
                cmbArea.SelectedItem
                    ?.ToString()
                ?? "Todas";


            if (!area.Equals(
                "Todas",
                StringComparison.OrdinalIgnoreCase))
            {
                consulta =
                    consulta.Where(
                        x =>
                            x.Area.Equals(
                                area,
                                StringComparison.OrdinalIgnoreCase));
            }


            // =====================================================
            // ORDEN
            // Activos primero
            // =====================================================

            List<Internamiento> resultado =
                consulta
                    .OrderBy(
                        x =>
                            x.IdEstadoInternamiento == 1
                                ? 0
                                : 1)
                    .ThenByDescending(
                        x =>
                            x.FechaIngreso)
                    .ToList();


            dgInternamientos.ItemsSource =
                resultado;


            txtResultados.Text =
                resultado.Count == 1

                    ? "1 internamiento"

                    : $"{resultado.Count} internamientos";
        }


        private bool Contiene(
            string? texto,
            string buscar)
        {
            return
                (texto ?? "")
                    .ToLower()
                    .Contains(buscar);
        }


        // =========================================================
        // CONTADORES
        // =========================================================

        private void ActualizarContadores()
        {
            try
            {
                txtActivos.Text =
                    listaInternamientos
                        .Count(
                            x =>
                                x.IdEstadoInternamiento == 1)
                        .ToString();


                txtHospitalizacion.Text =
                    listaInternamientos
                        .Count(
                            x =>
                                x.IdEstadoInternamiento == 1
                                &&
                                x.TipoInternamiento.Equals(
                                    "Hospitalización",
                                    StringComparison.OrdinalIgnoreCase))
                        .ToString();


                txtUci.Text =
                    listaInternamientos
                        .Count(
                            x =>
                                x.IdEstadoInternamiento == 1
                                &&
                                x.TipoInternamiento.Equals(
                                    "UCI",
                                    StringComparison.OrdinalIgnoreCase))
                        .ToString();


                txtAltasHoy.Text =
                    listaInternamientos
                        .Count(
                            x =>
                                x.IdEstadoInternamiento == 2
                                &&
                                x.FechaAlta.HasValue
                                &&
                                x.FechaAlta.Value.Date ==
                                DateTime.Today)
                        .ToString();


                txtCamasDisponibles.Text =
                    dao.ContarCamasDisponibles()
                        .ToString();


                txtCamasOcupadas.Text =
                    dao.ContarCamasOcupadas()
                        .ToString();
            }
            catch
            {
                txtCamasDisponibles.Text =
                    "0";

                txtCamasOcupadas.Text =
                    "0";
            }
        }


        // =========================================================
        // EVENTOS DE FILTROS
        // =========================================================

        private void txtBuscar_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            AplicarFiltros();
        }


        private void cmbEstado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            AplicarFiltros();
        }


        private void cmbArea_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            AplicarFiltros();
        }


        // =========================================================
        // NUEVO INTERNAMIENTO
        // =========================================================

        private void btnNuevoInternamiento_Click( object sender, RoutedEventArgs e)
        {
            try
            {
                InternamientoDialog dialog =
                    new InternamientoDialog();


                dialog.Owner =
                    Window.GetWindow(this);


                bool? resultado =
                    dialog.ShowDialog();


                if (resultado == true)
                {
                    CargarInternamientos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible abrir el formulario de internamiento.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // VER
        // =========================================================

        private void btnVerInternamiento_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.DataContext is not Internamiento internamiento)
            {
                return;
            }


            try
            {
                Internamiento? detalle =
                    dao.ObtenerPorId(
                        internamiento.IdInternamiento);


                if (detalle == null)
                {
                    MessageBox.Show(
                        "No fue posible cargar el internamiento seleccionado.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                InternamientoDialog dialog =
                    new InternamientoDialog(
                        detalle,
                        true);


                dialog.Owner =
                    Window.GetWindow(this);


                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible consultar el internamiento.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // EDITAR
        // =========================================================

        private void btnEditarInternamiento_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.DataContext is not Internamiento internamiento)
            {
                return;
            }


            try
            {
                Internamiento? detalle =
                    dao.ObtenerPorId(
                        internamiento.IdInternamiento);


                if (detalle == null)
                {
                    MessageBox.Show(
                        "No fue posible cargar el internamiento.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                if (detalle.IdEstadoInternamiento != 1)
                {
                    MessageBox.Show(
                        "Solo los internamientos activos pueden modificarse.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                InternamientoDialog dialog =
                    new InternamientoDialog(
                        detalle);


                dialog.Owner =
                    Window.GetWindow(this);


                bool? resultado =
                    dialog.ShowDialog();


                if (resultado == true)
                {
                    CargarInternamientos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible editar el internamiento.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // CAMBIAR CAMA
        // =========================================================

        private void btnCambiarCama_Click(
     object sender,
     RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.DataContext is not Internamiento internamiento)
            {
                return;
            }


            try
            {
                Internamiento? detalle =
                    dao.ObtenerPorId(
                        internamiento.IdInternamiento);


                if (detalle == null)
                {
                    MessageBox.Show(
                        "No fue posible cargar el internamiento.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                if (detalle.IdEstadoInternamiento != 1)
                {
                    MessageBox.Show(
                        "Solo los internamientos activos pueden cambiar de cama.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                CambiarCamaInternamientoDialog dialog =
                    new CambiarCamaInternamientoDialog(
                        detalle);


                dialog.Owner =
                    Window.GetWindow(this);


                bool? resultado =
                    dialog.ShowDialog();


                if (resultado == true)
                {
                    CargarInternamientos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cambiar la cama.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // DAR DE ALTA
        // =========================================================

        private void btnDarAlta_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.DataContext is not Internamiento internamiento)
            {
                return;
            }


            try
            {
                Internamiento? detalle =
                    dao.ObtenerPorId(
                        internamiento.IdInternamiento);


                if (detalle == null)
                {
                    MessageBox.Show(
                        "No fue posible cargar el internamiento.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                if (detalle.IdEstadoInternamiento != 1)
                {
                    MessageBox.Show(
                        "Este internamiento ya no se encuentra activo.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                AltaInternamientoDialog dialog =
                    new AltaInternamientoDialog(
                        detalle);


                dialog.Owner =
                    Window.GetWindow(this);


                bool? resultado =
                    dialog.ShowDialog();


                if (resultado == true)
                {
                    CargarInternamientos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible procesar el alta médica.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}