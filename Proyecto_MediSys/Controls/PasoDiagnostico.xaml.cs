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
    public partial class PasoDiagnostico : UserControl
    {
        private readonly CIE10DAO cie10DAO = new();

        private DiagnosticoEmergencia diagnostico = new();

        private List<CIE10> catalogoCIE10 = new();

        private ObservableCollection<CIE10> diagnosticosSeleccionados = new();

        private CIE10? diagnosticoPrincipal;

        private List<string> diagnosticosManuales = new();


        public PasoDiagnostico()
        {
            InitializeComponent();

            CargarCatalogoCIE10();

            dgDiagnosticosSeleccionados.ItemsSource =
                diagnosticosSeleccionados;
        }


        // ============================================================
        // CARGAR CATÁLOGO CIE-10
        // ============================================================

        private void CargarCatalogoCIE10()
        {
            try
            {
                catalogoCIE10 =
                    cie10DAO.ObtenerTodos();

                lstResultadosCIE10.ItemsSource =
                    catalogoCIE10;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar el catálogo CIE-10.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // BUSCAR
        // ============================================================

        private void txtBuscarCIE10_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            string texto =
                txtBuscarCIE10.Text
                    .Trim()
                    .ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                lstResultadosCIE10.ItemsSource =
                    catalogoCIE10;

                return;
            }

            var resultado =
                catalogoCIE10
                    .Where(c =>
                        c.Codigo
                            .ToLower()
                            .Contains(texto)
                        ||
                        c.Descripcion
                            .ToLower()
                            .Contains(texto)
                        ||
                        c.Categoria
                            .ToLower()
                            .Contains(texto))
                    .ToList();

            lstResultadosCIE10.ItemsSource =
                resultado;
        }


        // ============================================================
        // AGREGAR DIAGNÓSTICO DEL CATÁLOGO
        // ============================================================

        private void btnAgregarDiagnostico_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (lstResultadosCIE10.SelectedItem
                is not CIE10 cie10)
            {
                MessageBox.Show(
                    "Seleccione un diagnóstico del catálogo CIE-10.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            bool existe =
                diagnosticosSeleccionados
                    .Any(d =>
                        d.IdCIE10 == cie10.IdCIE10);

            if (existe)
            {
                MessageBox.Show(
                    "Este diagnóstico ya fue agregado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            diagnosticosSeleccionados.Add(cie10);

            // Si es el primero, lo establecemos como principal
            if (diagnosticoPrincipal == null)
            {
                diagnosticoPrincipal = cie10;
            }

            ActualizarDiagnosticoPrincipal();

            lstResultadosCIE10.SelectedItem = null;
        }


        // ============================================================
        // ESTABLECER COMO PRINCIPAL
        // ============================================================

        private void btnPrincipal_Click(
            object sender,
            RoutedEventArgs e)
        {
            Button boton = (Button)sender;

            if (boton.DataContext
                is not CIE10 cie10)
            {
                return;
            }

            diagnosticoPrincipal = cie10;

            ActualizarDiagnosticoPrincipal();

            dgDiagnosticosSeleccionados.Items.Refresh();
        }


        // ============================================================
        // QUITAR DIAGNÓSTICO
        // ============================================================

        private void btnQuitarDiagnostico_Click(
            object sender,
            RoutedEventArgs e)
        {
            Button boton = (Button)sender;

            if (boton.DataContext
                is not CIE10 cie10)
            {
                return;
            }

            diagnosticosSeleccionados.Remove(cie10);

            if (diagnosticoPrincipal != null &&
                diagnosticoPrincipal.IdCIE10 == cie10.IdCIE10)
            {
                diagnosticoPrincipal =
                    diagnosticosSeleccionados
                        .FirstOrDefault();
            }

            ActualizarDiagnosticoPrincipal();
        }


        // ============================================================
        // DIAGNÓSTICO MANUAL
        // ============================================================

        private void btnAgregarDiagnosticoManual_Click(
            object sender,
            RoutedEventArgs e)
        {
            string texto =
                txtDiagnosticoManual.Text.Trim();

            if (string.IsNullOrWhiteSpace(texto))
            {
                MessageBox.Show(
                    "Escriba el diagnóstico que desea agregar.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtDiagnosticoManual.Focus();

                return;
            }

            bool existe =
                diagnosticosManuales
                    .Any(d =>
                        d.Equals(
                            texto,
                            StringComparison.OrdinalIgnoreCase));

            if (existe)
            {
                MessageBox.Show(
                    "Ese diagnóstico manual ya fue agregado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            diagnosticosManuales.Add(texto);

            lstDiagnosticosManuales.ItemsSource = null;
            lstDiagnosticosManuales.ItemsSource =
                diagnosticosManuales;

            txtDiagnosticoManual.Clear();
        }


        // ============================================================
        // ACTUALIZAR PRINCIPAL
        // ============================================================

        private void ActualizarDiagnosticoPrincipal()
        {
            if (diagnosticoPrincipal == null)
            {
                txtDiagnosticoPrincipalSeleccionado.Text =
                    "Ninguno seleccionado";

                return;
            }

            txtDiagnosticoPrincipalSeleccionado.Text =
                diagnosticoPrincipal.Mostrar;
        }


        // ============================================================
        // VALIDAR
        // ============================================================

        public bool Validar()
        {
            if (diagnosticoPrincipal == null &&
                diagnosticosManuales.Count == 0)
            {
                MessageBox.Show(
                    "Debe registrar al menos un diagnóstico.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }


        // ============================================================
        // OBTENER DIAGNÓSTICO ACTUAL
        // ============================================================

        public DiagnosticoEmergencia ObtenerDiagnostico()
        {
            // Principal
            if (diagnosticoPrincipal != null)
            {
                diagnostico.DiagnosticoPrincipal =
                    diagnosticoPrincipal.Mostrar;
            }
            else if (diagnosticosManuales.Count > 0)
            {
                diagnostico.DiagnosticoPrincipal =
                    diagnosticosManuales[0];
            }


            // Secundarios provenientes de CIE-10
            List<string> secundarios = new();

            foreach (CIE10 item in diagnosticosSeleccionados)
            {
                if (diagnosticoPrincipal != null &&
                    item.IdCIE10 ==
                    diagnosticoPrincipal.IdCIE10)
                {
                    continue;
                }

                secundarios.Add(item.Mostrar);
            }


            // Agregar diagnósticos manuales
            foreach (string manual in diagnosticosManuales)
            {
                if (manual ==
                    diagnostico.DiagnosticoPrincipal)
                {
                    continue;
                }

                secundarios.Add(manual);
            }


            diagnostico.DiagnosticoSecundario =
                string.Join(", ", secundarios);


            diagnostico.ImpresionClinica =
                txtImpresionClinica.Text.Trim();


            diagnostico.Observaciones =
                txtObservacionesMedicas.Text.Trim();


            return diagnostico;
        }


        // ============================================================
        // EXPONER DATOS AL EMERGENCIA DIALOG
        // ============================================================

        public List<CIE10> ObtenerDiagnosticosSeleccionados()
        {
            return diagnosticosSeleccionados.ToList();
        }


        public CIE10? ObtenerDiagnosticoPrincipalCIE10()
        {
            return diagnosticoPrincipal;
        }


        public List<string> ObtenerDiagnosticosManuales()
        {
            return diagnosticosManuales.ToList();
        }
    }
}