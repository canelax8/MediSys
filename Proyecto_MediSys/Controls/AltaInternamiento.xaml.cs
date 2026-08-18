using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Windows;

namespace Proyecto_MediSys.Controls
{
    public partial class AltaInternamientoDialog : Window
    {
        private readonly InternamientoDAO dao =
            new InternamientoDAO();


        private readonly Internamiento internamiento;


        public AltaInternamientoDialog(
            Internamiento internamiento)
        {
            InitializeComponent();


            this.internamiento =
                internamiento;


            CargarDatos();
        }


        // =========================================================
        // CARGAR
        // =========================================================

        private void CargarDatos()
        {
            txtPaciente.Text =
                internamiento.NombrePaciente;


            txtCodigo.Text =
                internamiento.CodigoInternamiento;


            txtCama.Text =
                $"Ubicación actual: {internamiento.Area} / " +
                $"Hab. {internamiento.Habitacion} / " +
                $"Cama {internamiento.CodigoCama}\n\n" +
                "Al confirmar el alta, esta cama volverá a estar disponible.";
        }


        // =========================================================
        // CONFIRMAR
        // =========================================================

        private void btnConfirmar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (internamiento.IdEstadoInternamiento != 1)
            {
                MessageBox.Show(
                    "El internamiento ya no se encuentra activo.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            MessageBoxResult respuesta =
                MessageBox.Show(
                    $"¿Confirma el alta médica de este paciente?\n\n" +
                    $"{internamiento.NombrePaciente}\n" +
                    $"{internamiento.CodigoInternamiento}",
                    "Confirmar alta médica",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (respuesta !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                bool resultado =
                    dao.DarAlta(
                        internamiento.IdInternamiento,
                        txtObservaciones.Text.Trim());


                if (!resultado)
                {
                    MessageBox.Show(
                        "No fue posible registrar el alta médica.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                MessageBox.Show(
                    "Alta médica registrada correctamente.\n\n" +
                    "La cama ha sido liberada.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;


                Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible registrar el alta.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // CANCELAR
        // =========================================================

        private void btnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}