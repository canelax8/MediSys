using System.Windows;
using System.Windows.Input;
using Proyecto_MediSys.Helpers;
using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System.Collections.Generic;



namespace Proyecto_MediSys.Controls
{
    public partial class UsuarioDialog : Window
    {

        private readonly UsuarioDAO dao = new UsuarioDAO();
        public bool UsuarioGuardado { get; private set; } = false;

        private Usuario? usuarioActual = null;

        private ModoFormulario modo = ModoFormulario.Nuevo;

        private readonly RolDAO rolDAO = new RolDAO();

        private List<Rol> listaRoles = new();

        /*constructor del dialogo de usuario, carga los roles en el combobox*/
        public UsuarioDialog()
        {
            InitializeComponent();

            modo = ModoFormulario.Nuevo;

            CargarRoles();
        }

        /*constructor del dialogo de usuario para editar un usuario, recibe un objeto Usuario, carga los roles en el combobox y carga los datos del usuario en los campos correspondientes*/
        public UsuarioDialog(Usuario usuario)
        {
            InitializeComponent();

            CargarRoles();

            usuarioActual = usuario;

            modo = ModoFormulario.Editar;

            txtTitulo.Text = "Editar Usuario";

            CargarDatosUsuario();
        }

        public UsuarioDialog(Usuario usuario, ModoFormulario modoFormulario)
        {
            InitializeComponent();

            CargarRoles();

            usuarioActual = usuario;

            modo = modoFormulario;

            CargarDatosUsuario();

            if (modo == ModoFormulario.Ver)
            {
                txtTitulo.Text = "Detalle del Usuario";

                ConfigurarModoSoloLectura();
            }
        }

        /*--------------------------------------------*/
        private void CargarDatosUsuario()
        {
            if (usuarioActual == null)
                return;
           

            txtNombre.Text = usuarioActual.Nombre;

            txtApellido.Text = usuarioActual.Apellido;

            txtCorreo.Text = usuarioActual.Correo;

            txtTelefono.Text = usuarioActual.Telefono;

            txtUsuario.Text = usuarioActual.UsuarioLogin;

            cmbRol.SelectedValue = usuarioActual.IdRol;

            chkActivo.IsChecked = usuarioActual.Activo;

            chkDebeCambiarClave.IsChecked = usuarioActual.DebeCambiarClave;

            // Nunca cargamos la contraseña por seguridad.
            txtClave.Password = "";
            txtConfirmarClave.Password = "";
        }

        private void ConfigurarModoSoloLectura()
        {
            txtNombre.IsEnabled = false;
            txtApellido.IsEnabled = false;
            txtCorreo.IsEnabled = false;
            txtTelefono.IsEnabled = false;

            txtUsuario.IsEnabled = false;

            cmbRol.IsEnabled = false;

            txtClave.IsEnabled = false;
            txtConfirmarClave.IsEnabled = false;

            chkActivo.IsEnabled = false;
            chkDebeCambiarClave.IsEnabled = false;

            btnGuardar.Visibility = Visibility.Collapsed;

            btnCancelar.Content = "Cerrar";
        }


        private void BarraTitulo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }
        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
      
        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /*metodo para guardar el usuario, valida que los campos no esten vacios y que la clave y la confirmacion de clave sean iguales, si todo es correcto guarda el usuario en la base de datos y cierra el dialogo*/
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe escribir el nombre.");
                txtNombre.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Debe escribir el apellido.");
                txtApellido.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("Debe escribir el usuario.");
                txtUsuario.Focus();
                return;
            }

            if (cmbRol.SelectedValue == null)
            {
                MessageBox.Show("Debe seleccionar un rol.");
                cmbRol.Focus();
                return;
            }

            if (modo == ModoFormulario.Nuevo)
            {
                if (string.IsNullOrWhiteSpace(txtClave.Password))
                {
                    MessageBox.Show("Debe escribir una contraseña.");
                    txtClave.Focus();
                    return;
                }

                if (txtClave.Password != txtConfirmarClave.Password)
                {
                    MessageBox.Show("Las contraseñas no coinciden.");
                    txtConfirmarClave.Focus();
                    return;
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(txtClave.Password))
                {
                    if (txtClave.Password != txtConfirmarClave.Password)
                    {
                        MessageBox.Show("Las contraseñas no coinciden.");
                        txtConfirmarClave.Focus();
                        return;
                    }
                }
            }

            Usuario usuario = new Usuario();

            usuario.Nombre = txtNombre.Text.Trim();
            usuario.Apellido = txtApellido.Text.Trim();
            usuario.Correo = txtCorreo.Text.Trim();
            usuario.Telefono = txtTelefono.Text.Trim();

            usuario.UsuarioLogin = txtUsuario.Text.Trim();

            usuario.IdRol = Convert.ToInt64(cmbRol.SelectedValue);

            usuario.Activo = chkActivo.IsChecked == true;

            usuario.DebeCambiarClave = chkDebeCambiarClave.IsChecked == true;

            bool resultado;

            if(modo == ModoFormulario.Editar)
{
                usuario.IdUsuario = usuarioActual!.IdUsuario;

                if (string.IsNullOrWhiteSpace(txtClave.Password))
                {
                    usuario.ClaveHash = usuarioActual.ClaveHash;
                }
                else
                {
                    usuario.ClaveHash =
                        SHA256Helper.Encriptar(txtClave.Password);
                }

                resultado = dao.Actualizar(usuario);
            }
            else
            {
                usuario.ClaveHash =
                    SHA256Helper.Encriptar(txtClave.Password);

                resultado = dao.Insertar(usuario);
            }

            if (resultado)
            {
                UsuarioGuardado = true;

                Close();
            }


        }/*aqui termina el metodo para guardar el usuario*/

        private void CargarRoles()
        {
            listaRoles = rolDAO.ObtenerTodos();

            cmbRol.ItemsSource = listaRoles;

            if (listaRoles.Count > 0)
                cmbRol.SelectedIndex = 0;
        }
    }
}