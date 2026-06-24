using System;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Controls
{
    public partial class SidebarMenu : UserControl
    {
        // Evento que notificará qué opción del menú fue seleccionada
        public event Action<string>? MenuSeleccionado;

        public SidebarMenu()
        {
            InitializeComponent();
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Dashboard");
        }

        private void btnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Usuarios");
        }

        private void btnPacientes_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Pacientes");
        }

        private void btnMedicos_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Medicos");
        }

        private void btnEspecialidades_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Especialidades");
        }

        private void btnMedicamentos_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Medicamentos");
        }

        private void btnEmergencias_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Emergencias");
        }

        private void btnCitas_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Citas");
        }

        private void btnInternamientos_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Internamientos");
        }

        private void btnFacturacion_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Facturacion");
        }

        private void btnLaboratorio_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Laboratorio");
        }

        private void btnReportes_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Reportes");
        }

        private void btnBuscarPaciente_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("BuscarPaciente");
        }

        private void btnEstadisticas_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Estadisticas");
        }

        private void btnConfiguracion_Click(object sender, RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke("Configuracion");
        }

        private void btnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult respuesta = MessageBox.Show(
                "¿Desea cerrar la sesión?",
                "Cerrar sesión",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (respuesta == MessageBoxResult.Yes)
            {
                LoginWindow login = new LoginWindow();
                login.Show();

                Window.GetWindow(this)?.Close();
            }
        }
    }
}