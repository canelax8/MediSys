using Proyecto_MediSys.Controls;
using Proyecto_MediSys.Data;
using Proyecto_MediSys.Services;
using System.Windows;
using System.Windows.Controls;
using Proyecto_MediSys.Models;
using System.Collections.Generic;
using System.Linq;

namespace Proyecto_MediSys.Pages.Mantenimientos
{

    public partial class PacientesPage : Page
    {


        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            BuscarPacientes(txtBuscar.Text);
        }


        private readonly PacienteDAO dao = new PacienteDAO();

        private List<Paciente> listaPacientes = new();

        // Constructor
        public PacientesPage()
        {
            InitializeComponent();
            CargarPacientes();
        }

        private async void btnNuevoPaciente_Click(object sender, RoutedEventArgs e)
        {
            PacienteDialog dialog = new PacienteDialog();

            dialog.PacienteGuardado += () =>
            {
                CargarPacientes();
            };

            await DialogService.Mostrar(dialog);
        }

      

        private void CargarPacientes()
        {
            listaPacientes = dao.ObtenerTodos();

            dgPacientes.ItemsSource = listaPacientes;

            ActualizarContadores();
        }

        private void ActualizarContadores()
        {
            txtTotalPacientes.Text = listaPacientes.Count.ToString();
        }

        private void BuscarPacientes(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                dgPacientes.ItemsSource = listaPacientes;
                return;
            }

            texto = texto.ToLower().Trim();

            var resultado = listaPacientes.Where(p =>

                p.CodigoPaciente.ToLower().Contains(texto)

                || p.Nombre.ToLower().Contains(texto)

                || p.Apellido.ToLower().Contains(texto)

                || p.NombreCompleto.ToLower().Contains(texto)

                || p.NumeroDocumento.ToLower().Contains(texto)

                || p.CodigoTemporal.ToLower().Contains(texto)

            ).ToList();

            dgPacientes.ItemsSource = resultado;
        }

        private async void btnVer_Click(object sender, RoutedEventArgs e)
        {
            Button boton = (Button)sender;

            Paciente paciente = (Paciente)boton.DataContext;

            PacienteDialog dialog = new PacienteDialog(
                paciente,
                ModoFormulario.Ver);

            await DialogService.Mostrar(dialog);
        }

        private async void btnEditar_Click(object sender, RoutedEventArgs e)
        {
        

            Button boton = (Button)sender;

            Paciente paciente = (Paciente)boton.DataContext;

            PacienteDialog dialog = new PacienteDialog(
                paciente,
                ModoFormulario.Editar);

            dialog.PacienteGuardado += () =>
            {
                CargarPacientes();
            };

            await DialogService.Mostrar(dialog);
        }

        //*******boton documentos********//
        private async void btnDocumentos_Click(object sender, RoutedEventArgs e)
        {
            Button boton = (Button)sender;

            Paciente paciente = (Paciente)boton.DataContext;

            PacienteDialog dialog =
                new PacienteDialog(
                    paciente,
                    ModoFormulario.Editar);

            await DialogService.Mostrar(
                new PacienteDocumentosDialog(dialog));
        }
        //*******boton elimminar paciente********//
        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            Button boton = (Button)sender;

            Paciente paciente = (Paciente)boton.DataContext;

            MessageBoxResult respuesta = MessageBox.Show(
                $"¿Desea eliminar el paciente?\n\n{paciente.NombreCompleto}",
                "Eliminar paciente",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
                return;

            if (dao.Eliminar(paciente.IdPaciente))
            {
                MessageBox.Show(
                    "Paciente eliminado correctamente.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                CargarPacientes();
            }
            else
            {
                MessageBox.Show(
                    "No fue posible eliminar el paciente.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
    }
}