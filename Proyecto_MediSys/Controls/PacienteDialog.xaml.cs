using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using Proyecto_MediSys.Services;
using System;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;




namespace Proyecto_MediSys.Controls
{

    public partial class PacienteDialog : UserControl
    {
        private readonly PacienteDAO dao = new PacienteDAO();

        private readonly TipoPacienteDAO tipoPacienteDAO = new TipoPacienteDAO();

        private readonly SeguroDAO seguroDAO = new SeguroDAO();

        private readonly EstadoPacienteDAO estadoPacienteDAO = new EstadoPacienteDAO();


        private Paciente? pacienteActual;
        public event Action? PacienteGuardado;
        private readonly ModoFormulario modo;
        public ObservableCollection<DocumentoPaciente> 
        Documentos { get; set; } = new ObservableCollection<DocumentoPaciente>();


        public PacienteDialog(Paciente paciente, ModoFormulario modoFormulario)
        {
            InitializeComponent();

            CargarCatalogos();

            pacienteActual = paciente;

            modo = modoFormulario;

            CargarPaciente(paciente);

            ConfigurarModoFormulario();
        }

        public PacienteDialog()
        {
            InitializeComponent();

            CargarCatalogos();

            modo = ModoFormulario.Nuevo;
        }

        private void CargarCatalogos()
        {
            cmbTipoPaciente.ItemsSource = tipoPacienteDAO.ObtenerTodos();
            
            cmbSeguro.ItemsSource = seguroDAO.ObtenerTodos();

        }



        private void ConfigurarModoFormulario()
        {
            if (modo == ModoFormulario.Nuevo)
                return;

            bool soloLectura = modo == ModoFormulario.Ver;

            txtNombre.IsEnabled = !soloLectura;
            txtApellido.IsEnabled = !soloLectura;
            txtTelefono.IsEnabled = !soloLectura;
            txtDireccion.IsEnabled = !soloLectura;
            txtDocumento.IsEnabled = !soloLectura;
            cmbSexo.IsEnabled = !soloLectura;
            cmbTipoPaciente.IsEnabled = !soloLectura;
            cmbTipoDocumento.IsEnabled = !soloLectura;
            cmbSeguro.IsEnabled = !soloLectura;
            dpNacimiento.IsEnabled = !soloLectura;
            chkIndocumentado.IsEnabled = !soloLectura;
            btnGuardar.Visibility = soloLectura ? Visibility.Collapsed : Visibility.Visible;
            btnCancelar.Content = soloLectura ? "Cerrar": "Cancelar";
        }

        // Cargar los datos del paciente en el formulario
        private void CargarPaciente(Paciente paciente)
        {
            txtNombre.Text = paciente.Nombre;
            txtApellido.Text = paciente.Apellido;
            txtTelefono.Text = paciente.Telefono;
            txtDireccion.Text = paciente.Direccion;
            txtDocumento.Text = paciente.NumeroDocumento;
            dpNacimiento.SelectedDate = paciente.FechaNacimiento;
            txtCodigoTemporal.Text = paciente.CodigoTemporal;
            chkIndocumentado.IsChecked = paciente.Indocumentado;

            SeleccionarCombo(cmbSexo, paciente.Sexo);
            SeleccionarCombo(cmbTipoPaciente, paciente.NombreTipoPaciente);
            SeleccionarCombo(cmbTipoDocumento, paciente.TipoDocumento);
            SeleccionarCombo(cmbSeguro, paciente.NombreSeguro);
        }// Cargar los datos del paciente en el formulario

        // Seleccionar un valor en un ComboBox por su contenido
        private void SeleccionarCombo(ComboBox combo, string valor)
        {
            foreach (ComboBoxItem item in combo.Items)
            {
                if (item.Content.ToString() == valor)
                {
                    combo.SelectedItem = item;
                    break;
                }
            }
        }// Seleccionar un valor en un ComboBox por su contenido

        private readonly Regex soloNumeros = new Regex("[^0-9]+");
        private readonly Regex soloLetras = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$");

        //metodo para telefono
        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = soloNumeros.IsMatch(e.Text);
        }


        //metodo para las letras
        private void SoloLetras_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !soloLetras.IsMatch(e.Text);
        }

        //metodo para cargar la foto del paciente
        /*private void btnFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog abrir = new OpenFileDialog();

            abrir.Title = "Seleccione una fotografía";

            abrir.Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

            if (abrir.ShowDialog() == true)
            {
                rutaFoto = abrir.FileName;

                BitmapImage imagen = new BitmapImage();

                imagen.BeginInit();
                imagen.UriSource = new Uri(rutaFoto);
                imagen.CacheOption = BitmapCacheOption.OnLoad;
                imagen.EndInit();

                //imgPaciente.Source = imagen;

               // txtIcono.Visibility = Visibility.Collapsed;
            }
        }*/

        //Metodo para btnCancelar
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            string mensaje = modo == ModoFormulario.Nuevo ? "¿Desea cancelar el registro del paciente?": "¿Desea cerrar esta ventana?";
            var respuesta = MessageBox.Show(mensaje,"MediSys",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (respuesta == MessageBoxResult.Yes)
            {
                MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, this);
            }
        }

        //Metodo para btnGuardar    
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (Documentos.Count == 0)
            {
                var r = MessageBox.Show("El paciente no tiene documentos adjuntos.\n\n¿Desea continuar?","MediSys",MessageBoxButton.YesNo,MessageBoxImage.Question);
                if (r == MessageBoxResult.No)
                    return;
            }


            if (!ValidarFormulario())
                return;

            try
            {
                Paciente paciente = pacienteActual ?? new Paciente();


                if (modo == ModoFormulario.Nuevo)
                {
                    // El IdPaciente y CodigoPaciente
                    // los genera SQL Server automáticamente.
                }
                paciente.Activo = true;
                paciente.Nombre = txtNombre.Text.Trim();
                paciente.Apellido = txtApellido.Text.Trim();
                paciente.Telefono = txtTelefono.Text.Trim();
                paciente.Direccion = txtDireccion.Text.Trim();

                if (chkIndocumentado.IsChecked == true)
                {
                    paciente.IdTipoPaciente = Convert.ToInt32(cmbTipoPaciente.SelectedValue);

                    paciente.TipoDocumento = string.Empty;

                    paciente.NumeroDocumento = string.Empty;

                    paciente.CodigoTemporal = txtCodigoTemporal.Text;
                }
                else
                {
                    paciente.IdTipoPaciente = Convert.ToInt32(cmbTipoPaciente.SelectedValue);

                    paciente.TipoDocumento = ((ComboBoxItem)cmbTipoDocumento.SelectedItem).Content.ToString();

                    paciente.NumeroDocumento = txtDocumento.Text.Trim();

                    paciente.CodigoTemporal = string.Empty;
                }

                paciente.Indocumentado = chkIndocumentado.IsChecked == true;

                paciente.FechaNacimiento = dpNacimiento.SelectedDate ?? DateTime.Today;

                paciente.Sexo = ((ComboBoxItem)cmbSexo.SelectedItem).Content.ToString();

                paciente.IdSeguro = Convert.ToInt32(cmbSeguro.SelectedValue);

                paciente.IdEstadoPaciente = 1;

                bool guardado = false;

                if (modo == ModoFormulario.Nuevo)
                {
                    guardado = dao.Insertar(paciente);
                }
                else if (modo == ModoFormulario.Editar)
                {
                    guardado = dao.Actualizar(paciente);
                }

                if (guardado)
                {
                    MessageBox.Show(
                        modo == ModoFormulario.Nuevo
                            ? "Paciente registrado correctamente."
                            : "Paciente actualizado correctamente.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    PacienteGuardado?.Invoke();

                    MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, this);
                }
                else
                {
                    MessageBox.Show(
                        modo == ModoFormulario.Nuevo
                            ? "No se pudo registrar el pacienteluid."
                            : "No se pudo actualizar el paciente.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }//btnGuardar_Click


        //Metodo para validar los campos del formulario
        private bool ValidarFormulario()
        {
            //nombre
            string nombre = txtNombre.Text.Trim();

            if (nombre.Length < 2)
            {
                MessageBox.Show("El nombre debe contener al menos 2 letras.");
                txtNombre.Focus();
                return false;
            }

            if (nombre.Length > 15)
            {
                MessageBox.Show("El nombre no puede tener más de 15 caracteres.");
                txtNombre.Focus();
                return false;
            }

            //apellido
            string apellido = txtApellido.Text.Trim();

            if (apellido.Length < 2)
            {
                MessageBox.Show("El apellido debe contener al menos 2 letras.");
                txtNombre.Focus();
                return false;
            }

            if (apellido.Length > 15)
            {
                MessageBox.Show("El apellido no puede tener más de 15 caracteres.");
                txtNombre.Focus();
                return false;
            }

            // Validar documentos solamente si NO es indocumentado

            if (chkIndocumentado.IsChecked == false)
            {
                if (cmbTipoPaciente.SelectedIndex <= 0)
                {
                    MessageBox.Show("Seleccione el tipo de paciente.");
                    cmbTipoPaciente.Focus();
                    return false;
                }

                if (cmbTipoDocumento.SelectedIndex <= 0)
                {
                    MessageBox.Show("Seleccione el tipo de documento.");
                    cmbTipoDocumento.Focus();
                    return false;
                }

                if (txtDocumento.Text.Trim().Length < 5)
                {
                    MessageBox.Show("Digite el número del documento.");
                    txtDocumento.Focus();
                    return false;
                }
            }

            //sexo
            if (cmbSexo.SelectedIndex <= 0)
            {
                MessageBox.Show("Seleccione el sexo.");
                cmbSexo.Focus();
                return false;
            }
            //nacimiento
            if (dpNacimiento.SelectedDate == null)
            {
                MessageBox.Show("Seleccione la fecha de nacimiento.");
                dpNacimiento.Focus();
                return false;
            }

            if (dpNacimiento.SelectedDate > DateTime.Today)
            {
                MessageBox.Show("La fecha de nacimiento no puede ser futura.");
                dpNacimiento.Focus();
                return false;
            }

            if (dpNacimiento.SelectedDate < DateTime.Today.AddYears(-120))
            {
                MessageBox.Show("La fecha de nacimiento no es válida.");
                dpNacimiento.Focus();
                return false;
            }

            //telefono
            if (string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Digite el teléfono.");
                txtTelefono.Focus();
                return false;
            }

            if (!Regex.IsMatch(txtTelefono.Text, "^[0-9]{10}$"))
            {
                MessageBox.Show("El teléfono debe tener 10 dígitos.");
                txtTelefono.Focus();
                return false;
            }

            //direccion
            string direccion = txtDireccion.Text.Trim();

            if (direccion.Length < 5)
            {
                MessageBox.Show("Ingrese una dirección válida.");
                txtDireccion.Focus();
                return false;
            }

            //seguro
            if (cmbSeguro.SelectedIndex <= 0)
            {
                MessageBox.Show("Seleccione un seguro.");
                cmbSeguro.Focus();
                return false;
            }

            return true;
        }

        // ------------------------------------------------------------------//
        private void cmbTipoDocumento_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTipoDocumento.SelectedItem == null)
                return;

            ComboBoxItem item = (ComboBoxItem)cmbTipoDocumento.SelectedItem;

            string tipo = item.Content.ToString();

            txtDocumento.Clear();

            if (tipo == "Cédula")
            {
                txtDocumento.MaxLength = 11;
            }
            else if (tipo == "Pasaporte")
            {
                txtDocumento.MaxLength = 6;
            }
        }
        //------------------------------------------------------------------//
        private void chkIndocumentado_Checked(object sender, RoutedEventArgs e)
        {
            // Bloquear controles
            cmbTipoPaciente.IsEnabled = false;
            cmbTipoDocumento.IsEnabled = false;
            txtDocumento.IsEnabled = false;

            // Limpiar controles
            cmbTipoPaciente.SelectedIndex = 0;
            cmbTipoDocumento.SelectedIndex = 0;
            txtDocumento.Clear();

            // Mostrar código temporal
            panelTemporal.Visibility = Visibility.Visible;
            txtCodigoTemporal.Text = GenerarCodigoTemporal();

        }

            // Generar código temporal
            private string GenerarCodigoTemporal()
                    {
                        return "PAC-TEMP-" +
                               Guid.NewGuid()
                                   .ToString("N")
                                   .Substring(0, 8)
                                   .ToUpper();
                    }    

        

        //--------------------------------------------------------------------//
        private void chkIndocumentado_Unchecked(object sender, RoutedEventArgs e)
        {
            // Habilitar nuevamente
            cmbTipoPaciente.IsEnabled = true;
            cmbTipoDocumento.IsEnabled = true;
            txtDocumento.IsEnabled = true;

            // Ocultar código temporal
            panelTemporal.Visibility = Visibility.Collapsed;
        }
        //------------------------------------------------------------------//
        private void txtDocumento_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (cmbTipoDocumento.SelectedItem == null)
                return;

            ComboBoxItem item = (ComboBoxItem)cmbTipoDocumento.SelectedItem;

            string tipo = item.Content.ToString();

            if (tipo == "Cédula")
            {
                e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[0-9]");
            }
            else if (tipo == "Pasaporte")
            {
                e.Handled = !System.Text.RegularExpressions.Regex.IsMatch(e.Text, "[a-zA-Z0-9]");
            }
        }

        //------------------------------------------------------------------//
        private async void btnDocumentos_Click(object sender, RoutedEventArgs e)
        {
            //Cerrar el dialogo actual
            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, this);

            //Esperar un instante
            await Task.Delay(150);

            //Abrir el siguiente dialogo
            await DialogService.Mostrar(new PacienteDocumentosDialog(this));
        }
        // ------------------------------------------------------------------//
        public void ActualizarEstadoDocumentos()
        {
            txtEstadoIdentidad.Text = "Sin documento";
            txtEstadoSeguro.Text = "Sin documento";
            txtOtros.Text = "0 archivos";

            int otros = 0;

            foreach (var doc in Documentos)
            {
                if (doc.TipoDocumento.Contains("Cédula") ||
                    doc.TipoDocumento.Contains("Pasaporte"))
                {
                    txtEstadoIdentidad.Text = "Documento cargado";
                }
                else if (doc.TipoDocumento.Contains("Seguro"))
                {
                    txtEstadoSeguro.Text = "Documento cargado";
                }
                else
                {
                    otros++;
                }
            }

            
            txtOtros.Text = otros + " archivo(s)";
        }


        // ----------------------------------------------------------------//
        public string CodigoTemporalPaciente
        {
            get
            {
                return txtCodigoTemporal.Text;
            }
        }
    }

}
