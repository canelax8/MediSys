using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Net.Mail;
using System.Windows;

namespace Proyecto_MediSys.Controls
{
    public partial class MedicoDialog : Window
    {
        // ============================================================
        // DAO
        // ============================================================

        private readonly MedicoDAO medicoDAO =
            new MedicoDAO();


        // ============================================================
        // DATOS
        // ============================================================

        private Medico? medicoActual;

        private readonly bool esEdicion;

        private readonly bool soloLectura;


        // ============================================================
        // NUEVO
        // ============================================================

        public MedicoDialog()
        {
            InitializeComponent();


            esEdicion =
                false;


            soloLectura =
                false;


            CargarEspecialidades();


            chkActivo.IsChecked =
                true;


            txtTitulo.Text =
                "Nuevo Médico";


            txtSubtitulo.Text =
                "Complete la información profesional del médico.";


            txtCodigo.Text =
                "NUEVO";
        }


        // ============================================================
        // EDITAR / VER
        // ============================================================

        public MedicoDialog(
            Medico medico,
            bool soloLectura = false)
        {
            InitializeComponent();


            medicoActual =
                medico;


            esEdicion =
                true;


            this.soloLectura =
                soloLectura;


            CargarEspecialidades();

            CargarMedico();

            ConfigurarModo();
        }


        // ============================================================
        // ESPECIALIDADES
        // ============================================================

        private void CargarEspecialidades()
        {
            cmbEspecialidad.ItemsSource =
                medicoDAO
                    .ObtenerEspecialidades();
        }


        // ============================================================
        // CARGAR MÉDICO
        // ============================================================

        private void CargarMedico()
        {
            if (medicoActual == null)
                return;


            txtNombre.Text =
                medicoActual.Nombre;


            txtApellido.Text =
                medicoActual.Apellido;


            txtCedula.Text =
                medicoActual.Cedula;


            txtExequatur.Text =
                medicoActual.Exequatur;


            txtTelefono.Text =
                medicoActual.Telefono;


            txtCorreo.Text =
                medicoActual.Correo;


            cmbEspecialidad.SelectedValue =
                medicoActual.IdEspecialidad;


            chkActivo.IsChecked =
                medicoActual.Activo;


            txtCodigo.Text =
                medicoActual.CodigoMedico;
        }


        // ============================================================
        // MODO
        // ============================================================

        private void ConfigurarModo()
        {
            if (!esEdicion)
                return;


            if (soloLectura)
            {
                txtTitulo.Text =
                    "Detalle del Médico";


                txtSubtitulo.Text =
                    "Información profesional registrada en MediSys.";


                txtNombre.IsEnabled = false;

                txtApellido.IsEnabled = false;

                txtCedula.IsEnabled = false;

                txtExequatur.IsEnabled = false;

                txtTelefono.IsEnabled = false;

                txtCorreo.IsEnabled = false;

                cmbEspecialidad.IsEnabled = false;

                chkActivo.IsEnabled = false;


                btnGuardar.Visibility =
                    Visibility.Collapsed;


                btnCancelar.Content =
                    "Cerrar";
            }
            else
            {
                txtTitulo.Text =
                    "Editar Médico";


                txtSubtitulo.Text =
                    "Actualice la información profesional del médico.";


                btnCancelar.Content =
                    "✖  Cancelar";
            }
        }


        // ============================================================
        // VALIDAR
        // ============================================================

        private bool Validar()
        {
            string nombre =
                txtNombre.Text.Trim();


            string apellido =
                txtApellido.Text.Trim();


            string cedula =
                txtCedula.Text.Trim();


            string exequatur =
                txtExequatur.Text.Trim();


            string correo =
                txtCorreo.Text.Trim();


            if (nombre.Length < 2)
            {
                MessageBox.Show(
                    "Digite el nombre del médico.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtNombre.Focus();

                return false;
            }


            if (apellido.Length < 2)
            {
                MessageBox.Show(
                    "Digite el apellido del médico.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtApellido.Focus();

                return false;
            }


            if (string.IsNullOrWhiteSpace(
                cedula))
            {
                MessageBox.Show(
                    "Digite la cédula del médico.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtCedula.Focus();

                return false;
            }


            if (medicoDAO.ExisteCedula(
                cedula,
                medicoActual?.IdMedico))
            {
                MessageBox.Show(
                    "Ya existe un médico registrado con esta cédula.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtCedula.Focus();

                return false;
            }


            if (string.IsNullOrWhiteSpace(
                exequatur))
            {
                MessageBox.Show(
                    "Digite el exequátur del médico.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtExequatur.Focus();

                return false;
            }


            if (medicoDAO.ExisteExequatur(
                exequatur,
                medicoActual?.IdMedico))
            {
                MessageBox.Show(
                    "Ya existe un médico registrado con este exequátur.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtExequatur.Focus();

                return false;
            }


            if (cmbEspecialidad.SelectedValue ==
                null)
            {
                MessageBox.Show(
                    "Seleccione la especialidad del médico.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                cmbEspecialidad.Focus();

                return false;
            }


            if (!string.IsNullOrWhiteSpace(
                correo))
            {
                if (!MailAddress.TryCreate(
                    correo,
                    out _))
                {
                    MessageBox.Show(
                        "Digite un correo electrónico válido.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    txtCorreo.Focus();

                    return false;
                }
            }


            return true;
        }


        // ============================================================
        // GUARDAR
        // ============================================================

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (soloLectura)
                return;


            if (!Validar())
                return;


            try
            {
                Medico medico =
                    medicoActual
                    ?? new Medico();


                medico.Nombre =
                    txtNombre.Text.Trim();


                medico.Apellido =
                    txtApellido.Text.Trim();


                medico.Cedula =
                    txtCedula.Text.Trim();


                medico.Exequatur =
                    txtExequatur.Text.Trim();


                medico.Telefono =
                    txtTelefono.Text.Trim();


                medico.Correo =
                    txtCorreo.Text.Trim();


                medico.IdEspecialidad =
                    Convert.ToInt64(
                        cmbEspecialidad.SelectedValue);


                medico.Activo =
                    chkActivo.IsChecked == true;


                bool guardado;


                if (esEdicion)
                {
                    guardado =
                        medicoDAO
                            .Actualizar(
                                medico);
                }
                else
                {
                    guardado =
                        medicoDAO
                            .Insertar(
                                medico);
                }


                if (!guardado)
                {
                    MessageBox.Show(
                        "No fue posible guardar el médico.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                MessageBox.Show(
                    esEdicion
                        ? "Médico actualizado correctamente."
                        : "Médico registrado correctamente.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;


                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible guardar el médico.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // CANCELAR
        // ============================================================

        private void btnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (soloLectura)
            {
                Close();

                return;
            }


            MessageBoxResult respuesta =
                MessageBox.Show(
                    esEdicion
                        ? "¿Desea cerrar sin guardar los cambios?"
                        : "¿Desea cancelar el registro del médico?",
                    "MediSys",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (respuesta ==
                MessageBoxResult.Yes)
            {
                Close();
            }
        }
    }
}