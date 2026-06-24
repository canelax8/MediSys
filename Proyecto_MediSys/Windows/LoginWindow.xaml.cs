using Proyecto_MediSys.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Proyecto_MediSys
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void themeToggle_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("Debe ingresar el usuario.",
                                "Campo requerido",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                txtUsuario.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                MessageBox.Show("Debe ingresar la contraseña.",
                                "Campo requerido",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                txtPassword.Focus();
                return;
            }

            string usuario = txtUsuario.Text;
            string clave = txtPassword.Password;

            UsuarioDAO usuarioDAO = new UsuarioDAO();

            bool acceso = usuarioDAO.ValidarLogin(usuario, clave);

            if (acceso)
            {

                DashboardWindow dashboard = new DashboardWindow();
                dashboard.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos");
            }
        }

        private void txtUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void txtPassword_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}