using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Proyecto_MediSys.Controls;

namespace Proyecto_MediSys.Pages.Procesos
{
    public partial class CitasPage : Page
    {
        // =========================================================
        // DAO
        // =========================================================

        private readonly CitaDAO dao =
            new CitaDAO();


        // =========================================================
        // DATOS
        // =========================================================

        private List<Cita> listaCitas =
            new List<Cita>();


        private bool cargandoFiltros =
            false;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public CitasPage()
        {
            InitializeComponent();


            CargarCitas();
        }


        // =========================================================
        // CARGAR CITAS
        // =========================================================

        private void CargarCitas()
        {
            try
            {
                listaCitas =
                    dao.ObtenerTodos();


                CargarFiltros();


                AplicarFiltros();


                ActualizarContadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar las citas.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // CARGAR FILTROS
        // =========================================================

        private void CargarFiltros()
        {
            cargandoFiltros =
                true;


            // =====================================================
            // CONSERVAR SELECCIONES
            // =====================================================

            string estadoAnterior =
                cmbEstado.SelectedItem
                    ?.ToString()
                ?? "Todos";


            string especialidadAnterior =
                cmbEspecialidad.SelectedItem
                    ?.ToString()
                ?? "Todas";


            // =====================================================
            // ESTADOS
            // =====================================================

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


            if (estados.Contains(
                estadoAnterior))
            {
                cmbEstado.SelectedItem =
                    estadoAnterior;
            }
            else
            {
                cmbEstado.SelectedIndex =
                    0;
            }


            // =====================================================
            // ESPECIALIDADES
            // =====================================================

            List<string> especialidades =
                new List<string>
                {
                    "Todas"
                };


            especialidades.AddRange(
                listaCitas
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.Especialidad))
                    .Select(x =>
                        x.Especialidad)
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x));


            cmbEspecialidad.ItemsSource =
                especialidades;


            if (especialidades.Contains(
                especialidadAnterior))
            {
                cmbEspecialidad.SelectedItem =
                    especialidadAnterior;
            }
            else
            {
                cmbEspecialidad.SelectedIndex =
                    0;
            }


            cargandoFiltros =
                false;
        }


        // =========================================================
        // APLICAR FILTROS
        // =========================================================

        private void AplicarFiltros()
        {
            if (dgCitas == null)
                return;


            IEnumerable<Cita> resultado =
                listaCitas;


            // =====================================================
            // BUSCADOR
            // =====================================================

            string buscar =
                txtBuscar?.Text
                    ?.Trim()
                    .ToLower()
                ?? "";


            if (!string.IsNullOrWhiteSpace(
                buscar))
            {
                resultado =
                    resultado.Where(
                        c =>
                            (c.CodigoCita ?? "")
                                .ToLower()
                                .Contains(buscar)

                            ||

                            (c.NombrePaciente ?? "")
                                .ToLower()
                                .Contains(buscar)

                            ||

                            (c.NombreMedico ?? "")
                                .ToLower()
                                .Contains(buscar)

                            ||

                            (c.Especialidad ?? "")
                                .ToLower()
                                .Contains(buscar)

                            ||

                            (c.Motivo ?? "")
                                .ToLower()
                                .Contains(buscar));
            }


            // =====================================================
            // ESTADO
            // =====================================================

            string estado =
                cmbEstado?.SelectedItem
                    ?.ToString()
                ?? "Todos";


            if (estado != "Todos")
            {
                resultado =
                    resultado.Where(
                        c =>
                            c.Estado.Equals(
                                estado,
                                StringComparison
                                    .OrdinalIgnoreCase));
            }


            // =====================================================
            // ESPECIALIDAD
            // =====================================================

            string especialidad =
                cmbEspecialidad
                    ?.SelectedItem
                    ?.ToString()
                ?? "Todas";


            if (especialidad != "Todas")
            {
                resultado =
                    resultado.Where(
                        c =>
                            c.Especialidad.Equals(
                                especialidad,
                                StringComparison
                                    .OrdinalIgnoreCase));
            }


            // =====================================================
            // ORDEN
            //
            // Mostrar primero las citas más próximas.
            // =====================================================

            List<Cita> listaFiltrada =
                resultado
                    .OrderBy(c =>
                        c.FechaCita)
                    .ThenBy(c =>
                        c.HoraCita)
                    .ToList();


            dgCitas.ItemsSource =
                listaFiltrada;


            txtResultados.Text =
                listaFiltrada.Count == 1
                    ? "1 cita"
                    : $"{listaFiltrada.Count} citas";
        }


        // =========================================================
        // CONTADORES
        // =========================================================

        private void ActualizarContadores()
        {
            txtPendientes.Text =
                listaCitas.Count(
                    x =>
                    x.Estado.Equals(
                        "Pendiente",
                        StringComparison
                            .OrdinalIgnoreCase))
                .ToString();


            txtConfirmadas.Text =
                listaCitas.Count(
                    x =>
                    x.Estado.Equals(
                        "Confirmada",
                        StringComparison
                            .OrdinalIgnoreCase))
                .ToString();


            txtAtendidas.Text =
                listaCitas.Count(
                    x =>
                    x.Estado.Equals(
                        "Atendida",
                        StringComparison
                            .OrdinalIgnoreCase))
                .ToString();


            txtCanceladas.Text =
                listaCitas.Count(
                    x =>
                    x.Estado.Equals(
                        "Cancelada",
                        StringComparison
                            .OrdinalIgnoreCase))
                .ToString();


            txtHoy.Text =
                listaCitas.Count(
                    x =>
                    x.FechaCita.Date ==
                    DateTime.Today)
                .ToString();


            txtTotal.Text =
                listaCitas.Count
                    .ToString();
        }


        // =========================================================
        // BUSCAR
        // =========================================================

        private void txtBuscar_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            AplicarFiltros();
        }


        // =========================================================
        // FILTRO ESTADO
        // =========================================================

        private void cmbEstado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoFiltros)
                return;


            AplicarFiltros();
        }


        // =========================================================
        // FILTRO ESPECIALIDAD
        // =========================================================

        private void cmbEspecialidad_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoFiltros)
                return;


            AplicarFiltros();
        }


        // =========================================================
        // NUEVA CITA
        // =========================================================

        private void btnNuevaCita_Click( object sender,
     RoutedEventArgs e)
        {
            try
            {
                CitaDialog dialog =
                    new CitaDialog();


                dialog.Owner =
                    Window.GetWindow(this);


                bool? resultado =
                    dialog.ShowDialog();


                if (resultado == true)
                {
                    CargarCitas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible abrir el formulario de cita.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // VER
        // =========================================================

        private void btnVerCita_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.DataContext is not Cita cita)
            {
                return;
            }


            try
            {
                Cita? citaCompleta =
                    dao.ObtenerPorId(
                        cita.IdCita);


                if (citaCompleta == null)
                {
                    MessageBox.Show(
                        "No fue posible cargar la cita seleccionada.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                CitaDialog dialog =
                    new CitaDialog(
                        citaCompleta,
                        true);


                dialog.Owner =
                    Window.GetWindow(this);


                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible consultar la cita.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // EDITAR
        // =========================================================

        private void btnEditarCita_Click(
    object sender,
    RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.DataContext is not Cita cita)
            {
                return;
            }


            try
            {
                // Estados finales:
                // no permitimos editar.

                if (cita.Estado.Equals(
                        "Atendida",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    cita.Estado.Equals(
                        "Cancelada",
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    cita.Estado.Equals(
                        "No asistió",
                        StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Esta cita se encuentra en un estado final y ya no puede modificarse.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                Cita? citaCompleta =
                    dao.ObtenerPorId(
                        cita.IdCita);


                if (citaCompleta == null)
                {
                    MessageBox.Show(
                        "No fue posible cargar la cita seleccionada.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                CitaDialog dialog =
                    new CitaDialog(
                        citaCompleta);


                dialog.Owner =
                    Window.GetWindow(this);


                bool? resultado =
                    dialog.ShowDialog();


                if (resultado == true)
                {
                    CargarCitas();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible editar la cita.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // CONFIRMAR
        // =========================================================

        private void btnConfirmarCita_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.DataContext is not Cita cita)
            {
                return;
            }


            try
            {
                if (!cita.Estado.Equals(
                    "Pendiente",
                    StringComparison
                        .OrdinalIgnoreCase))
                {
                    return;
                }


                MessageBoxResult respuesta =
                    MessageBox.Show(
                        $"¿Desea confirmar la cita {cita.CodigoCita}?\n\n" +
                        $"Paciente: {cita.NombrePaciente}\n" +
                        $"Fecha: {cita.FechaHoraMostrar}",
                        "Confirmar Cita",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);


                if (respuesta !=
                    MessageBoxResult.Yes)
                {
                    return;
                }


                bool actualizado =
                    dao.ActualizarEstado(
                        cita.IdCita,
                        2);


                if (!actualizado)
                {
                    MessageBox.Show(
                        "No fue posible confirmar la cita.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                CargarCitas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible confirmar la cita.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // MARCAR COMO ATENDIDA
        // =========================================================

        private void btnAtendidaCita_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.DataContext is not Cita cita)
            {
                return;
            }


            try
            {
                if (!cita.Estado.Equals(
                    "Confirmada",
                    StringComparison
                        .OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "La cita debe estar confirmada antes de marcarla como atendida.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                MessageBoxResult respuesta =
                    MessageBox.Show(
                        $"¿Confirma que el paciente fue atendido?\n\n" +
                        $"{cita.NombrePaciente}",
                        "Marcar Cita como Atendida",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);


                if (respuesta !=
                    MessageBoxResult.Yes)
                {
                    return;
                }


                bool actualizado =
                    dao.ActualizarEstado(
                        cita.IdCita,
                        3);


                if (!actualizado)
                {
                    MessageBox.Show(
                        "No fue posible actualizar la cita.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                CargarCitas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible actualizar la cita.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // CANCELAR
        // =========================================================

        private void btnCancelarCita_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button boton ||
                boton.DataContext is not Cita cita)
            {
                return;
            }


            try
            {
                if (cita.Estado.Equals(
                        "Atendida",
                        StringComparison
                            .OrdinalIgnoreCase)
                    ||
                    cita.Estado.Equals(
                        "Cancelada",
                        StringComparison
                            .OrdinalIgnoreCase)
                    ||
                    cita.Estado.Equals(
                        "No asistió",
                        StringComparison
                            .OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Esta cita ya se encuentra en un estado final y no puede cancelarse.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                MessageBoxResult respuesta =
                    MessageBox.Show(
                        $"¿Está seguro de cancelar la cita?\n\n" +
                        $"Código: {cita.CodigoCita}\n" +
                        $"Paciente: {cita.NombrePaciente}\n" +
                        $"Fecha: {cita.FechaHoraMostrar}",
                        "Cancelar Cita",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Warning);


                if (respuesta !=
                    MessageBoxResult.Yes)
                {
                    return;
                }


                bool actualizado =
                    dao.ActualizarEstado(
                        cita.IdCita,
                        4);


                if (!actualizado)
                {
                    MessageBox.Show(
                        "No fue posible cancelar la cita.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                CargarCitas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cancelar la cita.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}