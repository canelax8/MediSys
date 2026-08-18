using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Proyecto_MediSys.Controls
{
    public partial class CambiarCamaInternamientoDialog : Window
    {
        private readonly InternamientoDAO dao =
            new InternamientoDAO();


        private readonly Internamiento internamiento;


        private List<AreaHospitalaria> areas =
            new List<AreaHospitalaria>();


        private bool cargandoDatos =
            false;


        public CambiarCamaInternamientoDialog(
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
            try
            {
                cargandoDatos =
                    true;


                txtPaciente.Text =
                    internamiento.NombrePaciente;


                txtCamaActual.Text =
                    $"{internamiento.Area} / " +
                    $"Hab. {internamiento.Habitacion} / " +
                    $"Cama {internamiento.CodigoCama}";


                areas =
                    dao.ObtenerAreas();


                cmbArea.ItemsSource =
                    areas;


                cmbHabitacion.ItemsSource =
                    new List<Habitacion>();


                cmbCama.ItemsSource =
                    new List<Cama>();


                cargandoDatos =
                    false;
            }
            catch (Exception ex)
            {
                cargandoDatos =
                    false;


                MessageBox.Show(
                    $"No fue posible cargar las camas disponibles.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);


                Close();
            }
        }


        // =========================================================
        // ÁREA
        // =========================================================

        private void cmbArea_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoDatos)
                return;


            cmbHabitacion.ItemsSource =
                new List<Habitacion>();


            cmbCama.ItemsSource =
                new List<Cama>();


            MostrarNeutral();


            if (cmbArea.SelectedItem
                is not AreaHospitalaria area)
            {
                return;
            }


            try
            {
                cmbHabitacion.ItemsSource =
                    dao.ObtenerHabitacionesPorArea(
                        area.IdArea);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar las habitaciones.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // HABITACIÓN
        // =========================================================

        private void cmbHabitacion_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoDatos)
                return;


            cmbCama.ItemsSource =
                new List<Cama>();


            MostrarNeutral();


            if (cmbHabitacion.SelectedItem
                is not Habitacion habitacion)
            {
                return;
            }


            try
            {
                // No incluimos la cama actual.
                // Queremos realmente seleccionar otra cama.

                List<Cama> camas =
                    dao.ObtenerCamasDisponibles(
                        habitacion.IdHabitacion);


                camas =
                    camas
                        .Where(
                            x =>
                                x.IdCama !=
                                internamiento.IdCama)
                        .ToList();


                cmbCama.ItemsSource =
                    camas;


                if (camas.Count == 0)
                {
                    txtDisponibilidad.Text =
                        "No hay camas disponibles en esta habitación.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar las camas.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // CAMA
        // =========================================================

        private void cmbCama_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cmbCama.SelectedItem
                is not Cama cama)
            {
                MostrarNeutral();

                return;
            }


            panelDisponibilidad.Background =
                new SolidColorBrush(
                    (Color)
                    ColorConverter.ConvertFromString(
                        "#E8F7EF"));


            panelDisponibilidad.BorderBrush =
                new SolidColorBrush(
                    (Color)
                    ColorConverter.ConvertFromString(
                        "#CDE8D7"));


            txtDisponibilidad.Foreground =
                new SolidColorBrush(
                    (Color)
                    ColorConverter.ConvertFromString(
                        "#287A4B"));


            txtDisponibilidad.Text =
                $"Cama {cama.CodigoCama} disponible.";
        }


        private void MostrarNeutral()
        {
            panelDisponibilidad.Background =
                new SolidColorBrush(
                    (Color)
                    ColorConverter.ConvertFromString(
                        "#F8FAFC"));


            panelDisponibilidad.BorderBrush =
                new SolidColorBrush(
                    (Color)
                    ColorConverter.ConvertFromString(
                        "#E2E8F0"));


            txtDisponibilidad.Foreground =
                new SolidColorBrush(
                    (Color)
                    ColorConverter.ConvertFromString(
                        "#64748B"));


            txtDisponibilidad.Text =
                "Seleccione una nueva cama.";
        }


        // =========================================================
        // CAMBIAR
        // =========================================================

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (cmbArea.SelectedItem
                is not AreaHospitalaria)
            {
                MessageBox.Show(
                    "Debe seleccionar el área.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (cmbHabitacion.SelectedItem
                is not Habitacion)
            {
                MessageBox.Show(
                    "Debe seleccionar la habitación.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (cmbCama.SelectedItem
                is not Cama cama)
            {
                MessageBox.Show(
                    "Debe seleccionar una cama disponible.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            MessageBoxResult confirmacion =
                MessageBox.Show(
                    $"¿Desea cambiar al paciente de cama?\n\n" +
                    $"Paciente: {internamiento.NombrePaciente}\n" +
                    $"Cama actual: {internamiento.CodigoCama}\n" +
                    $"Nueva cama: {cama.CodigoCama}",
                    "Confirmar cambio de cama",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (confirmacion !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                bool resultado =
                    dao.CambiarCama(
                        internamiento.IdInternamiento,
                        cama.IdCama);


                if (!resultado)
                {
                    MessageBox.Show(
                        "No fue posible cambiar la cama.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                MessageBox.Show(
                    $"Cambio realizado correctamente.\n\n" +
                    $"Nueva cama: {cama.CodigoCama}",
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
                    $"No fue posible cambiar la cama.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        private void btnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}