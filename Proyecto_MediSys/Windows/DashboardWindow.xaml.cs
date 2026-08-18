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

        private DispatcherTimer reloj =
            new DispatcherTimer();


        public DashboardWindow()
        {
            InitializeComponent();


            // =====================================================
            // MENSAJE TEMPORAL DE SESIÓN
            // Puedes quitarlo cuando ya no lo necesites
            // =====================================================

            MessageBox.Show(
                $"Usuario: {SesionActual.Usuario!.NombreCompleto}\n" +
                $"Id Médico: {SesionActual.Usuario.IdMedico}");


            // =====================================================
            // FRAME PRINCIPAL
            // =====================================================

            MainFramePrincipal =
                MainFrame;


            // =====================================================
            // PÁGINA INICIAL
            // =====================================================

            MainFrame.Navigate(
                new DashboardPage());


            // =====================================================
            // ESCUCHAR MENÚ LATERAL
            // =====================================================

            Sidebar.MenuSeleccionado +=
                Sidebar_MenuSeleccionado;


            // =====================================================
            // FECHA
            // =====================================================

            txtFecha.Text =
                DateTime.Now.ToString(
                    "dd/MM/yyyy");


            // =====================================================
            // RELOJ
            // =====================================================

            reloj.Interval =
                TimeSpan.FromSeconds(1);


            reloj.Tick +=
                Reloj_Tick;


            reloj.Start();
        }


        // =========================================================
        // RELOJ
        // =========================================================

        private void Reloj_Tick(
            object? sender,
            EventArgs e)
        {
            txtHora.Text =
                DateTime.Now.ToString(
                    "HH:mm:ss");
        }


        // =========================================================
        // NAVEGACIÓN DEL MENÚ
        // =========================================================

        private void Sidebar_MenuSeleccionado(
            string opcion)
        {
            try
            {
                switch (opcion)
                {
                    // =================================================
                    // DASHBOARD
                    // =================================================

                    case "Dashboard":

                        MainFrame.Navigate(
                            new DashboardPage());

                        break;


                    // =================================================
                    // PACIENTES
                    // =================================================

                    case "Pacientes":

                        MainFrame.Navigate(
                            new PacientesPage());

                        break;


                    // =================================================
                    // USUARIOS
                    // =================================================

                    case "Usuarios":

                        MainFrame.Navigate(
                            new UsuariosPage());

                        break;


                    // =================================================
                    // MÉDICOS
                    // =================================================

                    case "Medicos":

                        MessageBox.Show(
                            "Módulo Médicos en construcción.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        break;


                    // =================================================
                    // ESPECIALIDADES
                    // =================================================

                    case "Especialidades":

                        MessageBox.Show(
                            "Módulo Especialidades en construcción.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        break;


                    // =================================================
                    // MEDICAMENTOS
                    // =================================================

                    case "Medicamentos":

                        MessageBox.Show(
                            "Módulo Medicamentos en construcción.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        break;


                    // =================================================
                    // EMERGENCIAS
                    // =================================================

                    case "Emergencias":

                        MainFrame.Navigate(
                            new EmergenciasPage());

                        break;


                    // =================================================
                    // CITAS
                    // =================================================

                    case "Citas":

                        MainFrame.Navigate(
                            new CitasPage());

                        break;


                    // =================================================
                    // INTERNAMIENTOS
                    // =================================================

                    case "Internamientos":

                        MainFrame.Navigate(
                            new InternamientosPage());

                        break;


                    // =================================================
                    // FACTURACIÓN
                    // =================================================

                    case "Facturacion":

                        MessageBox.Show(
                            "Módulo Facturación en construcción.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        break;


                    // =================================================
                    // LABORATORIO
                    // =================================================

                    case "Laboratorio":

                        MessageBox.Show(
                            "Módulo Laboratorio en construcción.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        break;


                    // =================================================
                    // REPORTES
                    // =================================================

                    case "Reportes":

                        MessageBox.Show(
                            "Módulo Reportes en construcción.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        break;


                    // =================================================
                    // BUSCAR PACIENTE
                    // =================================================

                    case "BuscarPaciente":

                        MessageBox.Show(
                            "Módulo Buscar Paciente en construcción.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        break;


                    // =================================================
                    // ESTADÍSTICAS
                    // =================================================

                    case "Estadisticas":

                        MessageBox.Show(
                            "Módulo Estadísticas en construcción.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        break;


                    // =================================================
                    // CONFIGURACIÓN
                    // =================================================

                    case "Configuracion":

                        MessageBox.Show(
                            "Módulo Configuración en construcción.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        break;


                    // =================================================
                    // OPCIÓN NO RECONOCIDA
                    // =================================================

                    default:

                        MessageBox.Show(
                            $"La opción '{opcion}' no está configurada.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible abrir el módulo seleccionado.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void Sidebar_Loaded(
            object sender,
            RoutedEventArgs e)
        {
        }
    }
}