using Proyecto_MediSys.Controls;
using Proyecto_MediSys.Helpers;
using Proyecto_MediSys.Pages.Dashboard;
using Proyecto_MediSys.Pages.Mantenimientos;
using Proyecto_MediSys.Pages.Procesos;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Proyecto_MediSys
{
    public partial class DashboardWindow : Window
    {
        public static Frame? MainFramePrincipal { get; private set; }

        private DispatcherTimer reloj = new DispatcherTimer();

        public DashboardWindow()
        {
            InitializeComponent();

            MessageBox.Show(
                            $"Usuario: {SesionActual.Usuario!.NombreCompleto}\n" +
                            $"Id Médico: {SesionActual.Usuario.IdMedico}");

            MainFramePrincipal = MainFrame;

            // Página inicial
            MainFrame.Navigate(new DashboardPage());

            // Escuchar el menú lateral
            Sidebar.MenuSeleccionado += Sidebar_MenuSeleccionado;

            // Fecha
            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            // Hora
            reloj.Interval = TimeSpan.FromSeconds(1);
            reloj.Tick += Reloj_Tick;
            reloj.Start();
        }

        private void Reloj_Tick(object? sender, EventArgs e)
        {
            txtHora.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void Sidebar_MenuSeleccionado(string opcion)
        {
            switch (opcion)
            {
                case "Dashboard":
                    MainFrame.Navigate(new DashboardPage());
                    break;

                case "Pacientes":
                    MainFrame.Navigate(new PacientesPage());
                    break;

                case "Usuarios":
                    MainFrame.Navigate(new UsuariosPage());
                    break;

                case "Medicos":
                    MessageBox.Show("Módulo Médicos en construcción.");
                    break;

                case "Especialidades":
                    MessageBox.Show("Módulo Especialidades en construcción.");
                    break;

                case "Medicamentos":
                    MessageBox.Show("Módulo Medicamentos en construcción.");
                    break;

                case "Emergencias":
                    MainFrame.Navigate(new EmergenciasPage());
                    break;

                case "Citas":
                    MessageBox.Show("Módulo Citas en construcción.");
                    break;

                case "Internamientos":
                    MessageBox.Show("Módulo Internamientos en construcción.");
                    break;

                case "Facturacion":
                    MessageBox.Show("Módulo Facturación en construcción.");
                    break;

                case "Laboratorio":
                    MessageBox.Show("Módulo Laboratorio en construcción.");
                    break;

                case "Reportes":
                    MessageBox.Show("Módulo Reportes en construcción.");
                    break;

                case "BuscarPaciente":
                    MessageBox.Show("Módulo Buscar Paciente en construcción.");
                    break;

                case "Estadisticas":
                    MessageBox.Show("Módulo Estadísticas en construcción.");
                    break;

                case "Configuracion":
                    MessageBox.Show("Módulo Configuración en construcción.");
                    break;
            }
        }

        private void Sidebar_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}