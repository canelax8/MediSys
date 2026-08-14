using Microsoft.Win32;
using Proyecto_MediSys.Controls;
using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using Proyecto_MediSys.Services.PDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Pages.Procesos
{
    public partial class EmergenciasPage : Page
    {
        private readonly EmergenciaDAO dao = new();

        private List<Emergencia> listaEmergencias = new();

        public EmergenciasPage()
        {
            InitializeComponent();

            CargarEmergencias();
        }


        // ============================================================
        // CARGAR EMERGENCIAS
        // ============================================================

        private void CargarEmergencias()
        {
            try
            {
                listaEmergencias = dao.ObtenerTodos();

                AplicarFiltros();
                CargarEspecialidades();
                ActualizarContadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar las emergencias.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CambiarEstadoEmergencia(Emergencia emergencia)
        {
            CambiarEstadoEmergenciaDialog dialog =
                new CambiarEstadoEmergenciaDialog(emergencia);

            dialog.Owner = Window.GetWindow(this);

            bool? resultado = dialog.ShowDialog();

            if (resultado != true)
                return;

            bool actualizado = dao.ActualizarEstado(
                emergencia.IdEmergencia,
                dialog.IdEstadoSeleccionado);

            if (actualizado)
            {
                MessageBox.Show(
                    "El estado de la emergencia fue actualizado correctamente.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                CargarEmergencias();
            }
            else
            {
                MessageBox.Show(
                    "No fue posible actualizar el estado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void btnCambiarEstado_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;

            if (boton.DataContext is not Emergencia emergencia)
                return;

            CambiarEstadoEmergencia(emergencia);
        }

        private void btnVerEmergencia_Click(object sender,RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;

            if (boton.DataContext is not Emergencia emergencia)
                return;

            try
            {
                EmergenciaDetalleDialog dialog =
                    new EmergenciaDetalleDialog(emergencia);

                dialog.Owner = Window.GetWindow(this);

                dialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible abrir el expediente de emergencia.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ============================================================
        // INICIAR / CONTINUAR ATENCIÓN
        // ============================================================

        private void btnContinuarEmergencia_Click(
     object sender,
     RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;

            if (boton.DataContext is not Emergencia emergencia)
                return;

            try
            {
                // Si está pendiente, iniciar atención
                if (emergencia.IdEstadoEmergencia == 1)
                {
                    bool actualizado = dao.ActualizarEstado(
                        emergencia.IdEmergencia,
                        2); // En Atención

                    if (!actualizado)
                    {
                        MessageBox.Show(
                            "No fue posible iniciar la atención.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }
                }

                // Abrir el proceso existente
                EmergenciaDialog dialog =
                    new EmergenciaDialog(emergencia.IdEmergencia);

                dialog.Owner = Window.GetWindow(this);

                bool? resultado = dialog.ShowDialog();

                // Recargar siempre porque el estado pudo cambiar
                CargarEmergencias();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible continuar la emergencia.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // ============================================================
        // NUEVA EMERGENCIA
        // ============================================================

        private void btnNuevaEmergencia_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                EmergenciaDialog dialog = new EmergenciaDialog();

                dialog.Owner = Window.GetWindow(this);

                bool? resultado = dialog.ShowDialog();

                if (resultado == true)
                {
                    CargarEmergencias();

                    MessageBox.Show(
                        "La lista de emergencias fue actualizada.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible abrir el formulario de emergencia.\n\n{ex}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // BUSCAR
        // ============================================================

        private void txtBuscar_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            AplicarFiltros();
        }


        // ============================================================
        // FILTRO ESTADO
        // ============================================================

        private void cmbEstado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
                return;

            AplicarFiltros();
        }


        // ============================================================
        // FILTRO ESPECIALIDAD
        // ============================================================

        private void cmbEspecialidad_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
                return;

            AplicarFiltros();
        }


        // ============================================================
        // APLICAR FILTROS
        // ============================================================

        private void AplicarFiltros()
        {
            if (dgEmergencias == null)
                return;

            IEnumerable<Emergencia> resultado = listaEmergencias;


            // --------------------------------------------------------
            // BUSCADOR
            // --------------------------------------------------------

            string texto = txtBuscar?.Text?.Trim().ToLower() ?? "";

            if (!string.IsNullOrWhiteSpace(texto))
            {
                resultado = resultado.Where(e =>

                    (e.CodigoEmergencia ?? "")
                        .ToLower()
                        .Contains(texto)

                    ||

                    (e.NombrePaciente ?? "")
                        .ToLower()
                        .Contains(texto)

                    ||

                    (e.NombreMedico ?? "")
                        .ToLower()
                        .Contains(texto)

                    ||

                    (e.Especialidad ?? "")
                        .ToLower()
                        .Contains(texto)

                );
            }


            // --------------------------------------------------------
            // ESTADO
            // --------------------------------------------------------

            if (cmbEstado?.SelectedItem is ComboBoxItem estadoItem)
            {
                string estado =
                    estadoItem.Content?.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(estado)
                    && estado != "Todos")
                {
                    resultado = resultado.Where(e =>
                        (e.Estado ?? "")
                            .Equals(
                                estado,
                                StringComparison.OrdinalIgnoreCase));
                }
            }


            // --------------------------------------------------------
            // ESPECIALIDAD
            // --------------------------------------------------------

            if (cmbEspecialidad?.SelectedItem is ComboBoxItem especialidadItem)
            {
                string especialidad =
                    especialidadItem.Content?.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(especialidad)
                    && especialidad != "Todas")
                {
                    resultado = resultado.Where(e =>
                        (e.Especialidad ?? "")
                            .Equals(
                                especialidad,
                                StringComparison.OrdinalIgnoreCase));
                }
            }


            dgEmergencias.ItemsSource =
                resultado.ToList();
        }


        // ============================================================
        // CARGAR ESPECIALIDADES
        // ============================================================

        private void CargarEspecialidades()
        {
            if (cmbEspecialidad == null)
                return;

            cmbEspecialidad.Items.Clear();

            cmbEspecialidad.Items.Add(
                new ComboBoxItem
                {
                    Content = "Todas"
                });


            var especialidades =
                listaEmergencias
                    .Where(e => !string.IsNullOrWhiteSpace(e.Especialidad))
                    .Select(e => e.Especialidad)
                    .Distinct()
                    .OrderBy(e => e)
                    .ToList();


            foreach (string especialidad in especialidades)
            {
                cmbEspecialidad.Items.Add(
                    new ComboBoxItem
                    {
                        Content = especialidad
                    });
            }


            cmbEspecialidad.SelectedIndex = 0;
        }


        // ============================================================
        // CONTADORES
        // ============================================================

        private void ActualizarContadores()
        {
            if (listaEmergencias == null)
                return;


            txtTotal.Text =
                listaEmergencias.Count.ToString();


            txtHoy.Text =
                listaEmergencias.Count(e =>
                    e.FechaIngreso.Date == DateTime.Today)
                .ToString();


            txtEnEspera.Text =
                listaEmergencias.Count(e =>
                    EsEstado(e.Estado, "En espera")
                    || EsEstado(e.Estado, "Pendiente"))
                .ToString();


            txtAtendiendo.Text =
                listaEmergencias.Count(e =>
                    EsEstado(e.Estado, "En Atención")
                    || EsEstado(e.Estado, "Atendiendo"))
                .ToString();


            txtCriticos.Text =
                listaEmergencias.Count(e =>
                    EsEstado(e.Estado, "Crítico")
                    || EsEstado(e.Estado, "Critico"))
                .ToString();


            txtAltas.Text =
                listaEmergencias.Count(e =>
                    EsEstado(e.Estado, "Alta")
                    || EsEstado(e.Estado, "Finalizado"))
                .ToString();
        }


        private bool EsEstado(
            string estado,
            string valor)
        {
            return string.Equals(
                estado?.Trim(),
                valor,
                StringComparison.OrdinalIgnoreCase);
        }

        private void btnPDFEmergencia_Click(
    object sender,
    RoutedEventArgs e)
        {
            try
            {
                if (sender is not FrameworkElement elemento ||
                    elemento.DataContext is not Emergencia emergencia)
                {
                    MessageBox.Show(
                        "No fue posible identificar la emergencia seleccionada.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                // =====================================================
                // CARGAR EXPEDIENTE COMPLETO
                // =====================================================

                EmergenciaDAO dao =
                    new EmergenciaDAO();


                var resultado =
                    dao.ObtenerPorId(
                        emergencia.IdEmergencia);


                if (resultado.Emergencia == null ||
                    resultado.Proceso == null)
                {
                    MessageBox.Show(
                        "No fue posible cargar el expediente completo.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);

                    return;
                }


                // =====================================================
                // ELEGIR DÓNDE GUARDAR
                // =====================================================

                SaveFileDialog dialog =
                    new SaveFileDialog
                    {
                        Title =
                            "Guardar expediente de emergencia",

                        Filter =
                            "Documento PDF (*.pdf)|*.pdf",

                        FileName =
                            $"Expediente_{resultado.Emergencia.CodigoEmergencia}.pdf",

                        DefaultExt =
                            ".pdf",

                        AddExtension =
                            true
                    };


                bool? guardar =
                    dialog.ShowDialog();


                if (guardar != true)
                    return;


                // =====================================================
                // GENERAR PDF
                // =====================================================

                EmergenciaPdfService servicio =
                    new EmergenciaPdfService();


                servicio.Generar(
                    resultado.Emergencia,
                    resultado.Proceso,
                    dialog.FileName);


                // =====================================================
                // PREGUNTAR SI DESEA ABRIRLO
                // =====================================================

                MessageBoxResult abrir =
                    MessageBox.Show(
                        "El expediente PDF fue generado correctamente.\n\n¿Desea abrirlo ahora?",
                        "MediSys",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);


                if (abrir ==
                    MessageBoxResult.Yes)
                {
                    Process.Start(
                        new ProcessStartInfo
                        {
                            FileName =
                                dialog.FileName,

                            UseShellExecute =
                                true
                        });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible generar el PDF.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }




        // ============================================================
        // LIMPIAR FILTROS
        // ============================================================

        private void LimpiarFiltros()
        {
            if (txtBuscar != null)
                txtBuscar.Clear();

            if (cmbEstado != null)
                cmbEstado.SelectedIndex = 0;

            if (cmbEspecialidad != null)
                cmbEspecialidad.SelectedIndex = 0;

            AplicarFiltros();
        }
    }
}