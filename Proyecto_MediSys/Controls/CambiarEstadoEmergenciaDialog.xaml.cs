using Proyecto_MediSys.Models;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Controls
{
    public partial class CambiarEstadoEmergenciaDialog : Window
    {
        private readonly Emergencia emergencia;

        public long IdEstadoSeleccionado { get; private set; }

        public CambiarEstadoEmergenciaDialog(Emergencia emergencia)
        {
            InitializeComponent();

            this.emergencia = emergencia;

            CargarInformacion();
        }

        private void CargarInformacion()
        {
            txtCodigo.Text = emergencia.CodigoEmergencia;

            txtPaciente.Text = emergencia.NombrePaciente;

            txtEstadoActual.Text = emergencia.Estado;

            // Seleccionar el estado actual
            foreach (ComboBoxItem item in cmbEstado.Items)
            {
                if (long.TryParse(item.Tag?.ToString(), out long id))
                {
                    if (id == emergencia.IdEstadoEmergencia)
                    {
                        cmbEstado.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEstado.SelectedItem is not ComboBoxItem item)
            {
                MessageBox.Show(
                    "Seleccione un estado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!long.TryParse(
                    item.Tag?.ToString(),
                    out long idEstado))
            {
                MessageBox.Show(
                    "El estado seleccionado no es válido.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            IdEstadoSeleccionado = idEstado;

            DialogResult = true;
            Close();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}