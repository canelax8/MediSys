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
    public partial class PasoInformacionClinica : UserControl
    {
        // ============================================================
        // DATOS
        // ============================================================

        private InformacionClinica informacion = new();

        private Paciente? pacienteActual;

        private readonly AlergiaDAO alergiaDAO = new();

        private List<Alergia> catalogoAlergias = new();

        private ObservableCollection<PacienteAlergia> alergiasPaciente = new();


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public PasoInformacionClinica()
        {
            InitializeComponent();

            CargarCatalogoAlergias();

            dgAlergiasPaciente.ItemsSource = alergiasPaciente;
        }


        // ============================================================
        // RECIBIR PACIENTE DESDE EMERGENCIA
        // ============================================================

        public void CargarPaciente(Paciente paciente)
        {
            pacienteActual = paciente;

            CargarAlergiasPaciente();
        }


        // ============================================================
        // CARGAR INFORMACIÓN CLÍNICA EXISTENTE
        // ============================================================

        public void CargarInformacion(
            InformacionClinica informacionExistente)
        {
            if (informacionExistente == null)
                return;


            informacion =
                informacionExistente;


            txtMotivoConsulta.Text =
                informacionExistente.MotivoConsulta ?? "";


            chkDiabetes.IsChecked =
                informacionExistente.Diabetes;


            chkHipertension.IsChecked =
                informacionExistente.Hipertension;


            chkAsma.IsChecked =
                informacionExistente.Asma;


            chkCardiopatia.IsChecked =
                informacionExistente.Cardiopatia;


            chkEmbarazo.IsChecked =
                informacionExistente.Embarazo;


            chkNinguno.IsChecked =
                informacionExistente.Ninguno;


            txtMedicamentos.Text =
                informacionExistente.MedicamentosActuales ?? "";


            txtObservaciones.Text =
                informacionExistente.Observaciones ?? "";
        }

        // ============================================================
        // CARGAR CATÁLOGO GENERAL
        // ============================================================

        private void CargarCatalogoAlergias()
        {
            try
            {
                catalogoAlergias = alergiaDAO.ObtenerTodas();

                lstResultadosAlergias.ItemsSource = catalogoAlergias;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar el catálogo de alergias.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // CARGAR ALERGIAS DEL PACIENTE
        // ============================================================

        private void CargarAlergiasPaciente()
        {
            if (pacienteActual == null)
                return;

            try
            {
                List<PacienteAlergia> lista =
                    alergiaDAO.ObtenerPorPaciente(
                        pacienteActual.IdPaciente);

                alergiasPaciente.Clear();

                foreach (PacienteAlergia alergia in lista)
                {
                    alergiasPaciente.Add(alergia);
                }

                ActualizarAlertaAlergias();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar las alergias del paciente.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // ALERTA VISUAL
        // ============================================================

        private void ActualizarAlertaAlergias()
        {
            int cantidad = alergiasPaciente.Count;

            if (cantidad > 0)
            {
                panelAlertaAlergia.Visibility =
                    Visibility.Visible;

                txtContadorAlergias.Text =
                    cantidad == 1
                        ? "1 alergia registrada"
                        : $"{cantidad} alergias registradas";
            }
            else
            {
                panelAlertaAlergia.Visibility =
                    Visibility.Collapsed;

                txtContadorAlergias.Text =
                    "Sin alergias registradas";
            }
        }


        // ============================================================
        // BUSCAR ALERGIA
        // ============================================================

        private void txtBuscarAlergia_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            string texto =
                txtBuscarAlergia.Text
                    .Trim()
                    .ToLower();

            if (string.IsNullOrWhiteSpace(texto))
            {
                lstResultadosAlergias.ItemsSource =
                    catalogoAlergias;

                return;
            }


            List<Alergia> resultado =
                catalogoAlergias
                    .Where(a =>
                        a.Nombre
                            .ToLower()
                            .Contains(texto)
                        ||
                        a.Descripcion
                            .ToLower()
                            .Contains(texto))
                    .ToList();


            lstResultadosAlergias.ItemsSource =
                resultado;
        }


        // ============================================================
        // AGREGAR DESDE CATÁLOGO
        // ============================================================

        private void btnAgregarAlergiaCatalogo_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (pacienteActual == null)
            {
                MessageBox.Show(
                    "No hay un paciente seleccionado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (lstResultadosAlergias.SelectedItem
                is not Alergia alergia)
            {
                MessageBox.Show(
                    "Seleccione una alergia del catálogo.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            try
            {
                bool agregado =
                    alergiaDAO.AgregarAlPaciente(
                        pacienteActual.IdPaciente,
                        alergia.IdAlergia);


                if (!agregado)
                {
                    MessageBox.Show(
                        "Esta alergia ya está registrada para el paciente.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }


                CargarAlergiasPaciente();

                txtBuscarAlergia.Clear();

                lstResultadosAlergias.SelectedItem =
                    null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible registrar la alergia.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // AGREGAR ALERGIA MANUAL
        // ============================================================

        private void btnAgregarAlergiaManual_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (pacienteActual == null)
            {
                MessageBox.Show(
                    "No hay un paciente seleccionado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            string alergia =
                txtAlergiaManual.Text.Trim();


            if (string.IsNullOrWhiteSpace(alergia))
            {
                MessageBox.Show(
                    "Escriba el nombre de la alergia.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtAlergiaManual.Focus();

                return;
            }


            try
            {
                bool agregado =
                    alergiaDAO.AgregarManual(
                        pacienteActual.IdPaciente,
                        alergia);


                if (!agregado)
                {
                    MessageBox.Show(
                        "La alergia ya está registrada o no fue posible agregarla.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    return;
                }


                txtAlergiaManual.Clear();

                CargarAlergiasPaciente();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible registrar la alergia.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // QUITAR ALERGIA
        // ============================================================

        private void btnQuitarAlergia_Click(
            object sender,
            RoutedEventArgs e)
        {
            Button boton = (Button)sender;

            if (boton.DataContext
                is not PacienteAlergia alergia)
            {
                return;
            }


            MessageBoxResult respuesta =
                MessageBox.Show(
                    $"¿Desea quitar la alergia '{alergia.AlergiaMostrar}' del paciente?",
                    "MediSys",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (respuesta != MessageBoxResult.Yes)
                return;


            try
            {
                if (alergiaDAO.QuitarDelPaciente(
                    alergia.IdPacienteAlergia))
                {
                    CargarAlergiasPaciente();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible quitar la alergia.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // VALIDAR PASO
        // ============================================================

        public bool Validar()
        {
            if (string.IsNullOrWhiteSpace(
                txtMotivoConsulta.Text))
            {
                MessageBox.Show(
                    "Debe escribir el motivo de consulta.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtMotivoConsulta.Focus();

                return false;
            }

            return true;
        }


        // ============================================================
        // OBTENER INFORMACIÓN CLÍNICA
        // ============================================================

        public InformacionClinica ObtenerInformacion()
        {
            informacion.MotivoConsulta =
                txtMotivoConsulta.Text.Trim();

            informacion.Diabetes =
                chkDiabetes.IsChecked == true;

            informacion.Hipertension =
                chkHipertension.IsChecked == true;

            informacion.Asma =
                chkAsma.IsChecked == true;

            informacion.Cardiopatia =
                chkCardiopatia.IsChecked == true;

            informacion.Embarazo =
                chkEmbarazo.IsChecked == true;

            informacion.Ninguno =
                chkNinguno.IsChecked == true;


            // ========================================================
            // COMPATIBILIDAD CON EL MODELO ACTUAL
            // ========================================================
            //
            // Aunque las alergias reales ya quedan almacenadas
            // en tbPacienteAlergias, seguimos llenando este campo
            // para no romper el guardado actual de Emergencia.
            //

            informacion.Alergias =
                string.Join(
                    ", ",
                    alergiasPaciente.Select(
                        a => a.AlergiaMostrar));


            informacion.MedicamentosActuales =
                txtMedicamentos.Text.Trim();

            informacion.Observaciones =
                txtObservaciones.Text.Trim();


            return informacion;
        }
    }
}