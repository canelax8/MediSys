using Proyecto_MediSys.Data;
using Proyecto_MediSys.Helpers;
using Proyecto_MediSys.Models;
using Proyecto_MediSys.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Proyecto_MediSys.Controls
{
    public partial class PasoPaciente : UserControl
    {
        private readonly PacienteDAO pacienteDAO = new();

        private List<Paciente> listaPacientes = new();

        private bool seleccionandoDesdeCodigo = false;


        public Paciente? PacienteSeleccionado { get; private set; }

        public event Action<Paciente>? PacienteSeleccionadoChanged;


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public PasoPaciente()
        {
            InitializeComponent();

            ConfigurarGrid();

            // Cargar pacientes automáticamente al abrir el paso
            CargarPacientes();
        }


        // ============================================================
        // CONFIGURAR GRID
        // ============================================================

        private void ConfigurarGrid()
        {
            dgPacientes.Columns.Clear();


            dgPacientes.Columns.Add(
                new DataGridTextColumn
                {
                    Header = "Código",
                    Width = 130,
                    Binding =
                        new System.Windows.Data.Binding(
                            "CodigoPaciente")
                });


            dgPacientes.Columns.Add(
                new DataGridTextColumn
                {
                    Header = "Paciente",
                    Width = new DataGridLength(
                        2,
                        DataGridLengthUnitType.Star),

                    Binding =
                        new System.Windows.Data.Binding(
                            "NombreCompleto")
                });


            dgPacientes.Columns.Add(
                new DataGridTextColumn
                {
                    Header = "Documento",
                    Width = 170,
                    Binding =
                        new System.Windows.Data.Binding(
                            "DocumentoMostrar")
                });


            dgPacientes.Columns.Add(
                new DataGridTextColumn
                {
                    Header = "Tipo",
                    Width = 140,
                    Binding =
                        new System.Windows.Data.Binding(
                            "NombreTipoPaciente")
                });


            dgPacientes.Columns.Add(
                new DataGridTextColumn
                {
                    Header = "Seguro",
                    Width = 180,
                    Binding =
                        new System.Windows.Data.Binding(
                            "NombreSeguro")
                });
        }


        // ============================================================
        // CARGAR TODOS LOS PACIENTES
        // ============================================================

        private void CargarPacientes()
        {
            try
            {
                listaPacientes =
                    pacienteDAO.ObtenerTodos();

                dgPacientes.ItemsSource = null;
                dgPacientes.ItemsSource = listaPacientes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar los pacientes.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // BUSCAR
        // ============================================================

        private void BuscarPacientes()
        {
            try
            {
                string texto =
                    txtBuscarPaciente.Text.Trim();


                // Si no hay texto, mostrar TODOS
                if (string.IsNullOrWhiteSpace(texto))
                {
                    CargarPacientes();
                    return;
                }


                listaPacientes =
                    pacienteDAO.Buscar(texto);


                dgPacientes.ItemsSource = null;
                dgPacientes.ItemsSource = listaPacientes;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible buscar pacientes.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // BOTÓN BUSCAR
        // ============================================================

        private void btnBuscarPaciente_Click(
            object sender,
            RoutedEventArgs e)
        {
            BuscarPacientes();
        }


        // ============================================================
        // BUSCAR MIENTRAS ESCRIBE
        // ============================================================

        private void txtBuscarPaciente_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            // Evitar ejecutar durante la inicialización
            if (!IsLoaded)
                return;

            BuscarPacientes();
        }


        // ============================================================
        // SELECCIONAR PACIENTE
        // ============================================================

        private void SeleccionarPaciente(
            Paciente paciente)
        {
            if (paciente == null)
                return;


            PacienteSeleccionado = paciente;


            txtPacienteSeleccionado.Text =
                $"{paciente.NombreCompleto}\n" +
                $"Documento: {paciente.DocumentoMostrar}    " +
                $"Edad: {paciente.Edad}\n" +
                $"Tipo: {paciente.NombreTipoPaciente}    " +
                $"Seguro: {paciente.NombreSeguro}\n" +
                $"Teléfono: {paciente.Telefono}";


            PacienteSeleccionadoChanged?.Invoke(
                paciente);
        }


        // ============================================================
        // SELECCIÓN CON UN CLIC
        // ============================================================

        private void dgPacientes_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (seleccionandoDesdeCodigo)
                return;


            if (dgPacientes.SelectedItem
                is not Paciente paciente)
            {
                return;
            }


            SeleccionarPaciente(paciente);
        }


        // ============================================================
        // DOBLE CLIC
        // ============================================================

        private void dgPacientes_MouseDoubleClick(
            object sender,
            MouseButtonEventArgs e)
        {
            if (dgPacientes.SelectedItem
                is not Paciente paciente)
            {
                return;
            }


            SeleccionarPaciente(paciente);
        }


        // ============================================================
        // SELECCIONAR UN PACIENTE DESDE CÓDIGO
        // ============================================================

        public void SeleccionarPacienteExistente(
            Paciente paciente)
        {
            if (paciente == null)
                return;


            // Primero asegurarnos de que el listado esté cargado
            if (listaPacientes.Count == 0)
            {
                CargarPacientes();
            }


            Paciente? encontrado = null;


            foreach (Paciente p in listaPacientes)
            {
                if (p.IdPaciente == paciente.IdPaciente)
                {
                    encontrado = p;
                    break;
                }
            }


            if (encontrado == null)
            {
                // Si por alguna razón no aparece en la lista,
                // utilizar directamente el paciente recibido.

                encontrado = paciente;
            }


            seleccionandoDesdeCodigo = true;

            dgPacientes.SelectedItem = encontrado;
            dgPacientes.ScrollIntoView(encontrado);

            seleccionandoDesdeCodigo = false;


            SeleccionarPaciente(encontrado);
        }


        // ============================================================
        // REGISTRAR PACIENTE PROVISIONAL
        // ============================================================

        private async void btnNuevoPaciente_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                btnPacienteProvisional.IsEnabled = false;


                PacienteDialog dialog =
                    new PacienteDialog();


                dialog.PacienteGuardado += (paciente) =>
                {
                    // Recargar todos los pacientes
                    CargarPacientes();

                    // Seleccionar automáticamente
                    SeleccionarPacienteExistente(
                        paciente);
                };


                await DialogService.Mostrar(
                    dialog);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible registrar el paciente provisional.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                btnPacienteProvisional.IsEnabled = true;
            }
        }
    }
}