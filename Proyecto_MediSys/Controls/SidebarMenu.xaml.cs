using System;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Controls
{
    public partial class SidebarMenu : UserControl
    {
        // =========================================================
        // EVENTO DEL MENÚ
        // =========================================================

        public event Action<string>? MenuSeleccionado;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public SidebarMenu()
        {
            InitializeComponent();
        }


        // =========================================================
        // DASHBOARD
        // =========================================================

        private void btnDashboard_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Dashboard");
        }


        // =========================================================
        // USUARIOS
        // =========================================================

        private void btnUsuarios_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Usuarios");
        }


        // =========================================================
        // PACIENTES
        // =========================================================

        private void btnPacientes_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Pacientes");
        }


        // =========================================================
        // MÉDICOS
        // =========================================================

        private void btnMedicos_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Medicos");
        }


        // =========================================================
        // ESPECIALIDADES
        // =========================================================

        private void btnEspecialidades_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Especialidades");
        }


        // =========================================================
        // MEDICAMENTOS
        // =========================================================

        private void btnMedicamentos_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Medicamentos");
        }


        // =========================================================
        // EMERGENCIAS
        // =========================================================

        private void btnEmergencias_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Emergencias");
        }


        // =========================================================
        // CITAS
        // =========================================================

        private void btnCitas_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Citas");
        }


        // =========================================================
        // INTERNAMIENTOS
        // =========================================================

        private void btnInternamientos_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Internamientos");
        }


        // =========================================================
        // FACTURACIÓN
        // =========================================================

        private void btnFacturacion_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Facturacion");
        }


        // =========================================================
        // LABORATORIO
        // =========================================================

        private void btnLaboratorio_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Laboratorio");
        }


        // =========================================================
        // REPORTES
        // =========================================================

        private void btnReportes_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Reportes");
        }


        // =========================================================
        // BUSCAR PACIENTE
        // =========================================================

        private void btnBuscarPaciente_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "BuscarPaciente");
        }


        // =========================================================
        // ESTADÍSTICAS
        // =========================================================

        private void btnEstadisticas_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Estadisticas");
        }


        // =========================================================
        // CONFIGURACIÓN
        // =========================================================

        private void btnConfiguracion_Click(
            object sender,
            RoutedEventArgs e)
        {
            MenuSeleccionado?.Invoke(
                "Configuracion");
        }


        // =========================================================
        // CERRAR SESIÓN
        // =========================================================

        private void btnCerrarSesion_Click(
            object sender,
            RoutedEventArgs e)
        {
            MessageBoxResult respuesta =
                MessageBox.Show(
                    "¿Desea cerrar la sesión?",
                    "Cerrar sesión",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (respuesta !=
                MessageBoxResult.Yes)
            {
                return;
            }


            LoginWindow login =
                new LoginWindow();


            login.Show();


            Window.GetWindow(this)
                ?.Close();
        }
    }
}