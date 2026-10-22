using Proyecto_MediSys.Controls;
using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Pages.Mantenimientos
{
    public partial class MedicosPage : Page
    {
        private readonly MedicoDAO medicoDAO =
            new MedicoDAO();


        private List<Medico> medicos =
            new List<Medico>();


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public MedicosPage()
        {
            InitializeComponent();


            CargarEspecialidades();

            CargarMedicos();
        }


        // ============================================================
        // MÉDICOS
        // ============================================================

        private void CargarMedicos()
        {
            try
            {
                medicos =
                    medicoDAO
                        .ObtenerTodos();


                txtTotalMedicos.Text =
                    medicos.Count.ToString();


                txtActivos.Text =
                    medicos
                        .Count(x => x.Activo)
                        .ToString();


                txtInactivos.Text =
                    medicos
                        .Count(x => !x.Activo)
                        .ToString();


                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar los médicos.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // ESPECIALIDADES
        // ============================================================

        private void CargarEspecialidades()
        {
            try
            {
                List<string> especialidades =
                    medicoDAO
                        .ObtenerEspecialidades()
                        .Select(x => x.Nombre)
                        .ToList();


                especialidades.Insert(
                    0,
                    "Todas");


                cmbEspecialidad.ItemsSource =
                    especialidades;


                cmbEspecialidad.SelectedIndex =
                    0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar las especialidades.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // FILTROS
        // ============================================================

        private void AplicarFiltros()
        {
            IEnumerable<Medico> resultado =
                medicos;


            string buscar =
                txtBuscar.Text
                    .Trim()
                    .ToLower();


            if (!string.IsNullOrWhiteSpace(
                buscar))
            {
                resultado =
                    resultado.Where(
                        x =>
                            x.CodigoMedico
                                .ToLower()
                                .Contains(buscar)

                            ||

                            x.NombreCompleto
                                .ToLower()
                                .Contains(buscar)

                            ||

                            x.Cedula
                                .ToLower()
                                .Contains(buscar)

                            ||

                            x.Exequatur
                                .ToLower()
                                .Contains(buscar)

                            ||

                            x.NombreEspecialidad
                                .ToLower()
                                .Contains(buscar));
            }


            string especialidad =
                cmbEspecialidad.SelectedItem
                    ?.ToString() ?? "Todas";


            if (especialidad !=
                "Todas")
            {
                resultado =
                    resultado.Where(
                        x =>
                            x.NombreEspecialidad ==
                            especialidad);
            }


            dgMedicos.ItemsSource =
                resultado.ToList();
        }


        private void txtBuscar_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            AplicarFiltros();
        }


        private void cmbEspecialidad_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
                return;


            AplicarFiltros();
        }


        // ============================================================
        // NUEVO
        // ============================================================

        private void btnNuevoMedico_Click(
            object sender,
            RoutedEventArgs e)
        {
            MedicoDialog dialog =
                new MedicoDialog();


            dialog.Owner =
                Window.GetWindow(this);


            if (dialog.ShowDialog() ==
                true)
            {
                CargarMedicos();
            }
        }


        // ============================================================
        // VER
        // ============================================================

        private void btnVer_Click(
            object sender,
            RoutedEventArgs e)
        {
            if ((sender as Button)
                ?.DataContext
                is not Medico medico)
            {
                return;
            }


            MedicoDialog dialog =
                new MedicoDialog(
                    medico,
                    soloLectura: true);


            dialog.Owner =
                Window.GetWindow(this);


            dialog.ShowDialog();
        }


        // ============================================================
        // EDITAR
        // ============================================================

        private void btnEditar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if ((sender as Button)
                ?.DataContext
                is not Medico medico)
            {
                return;
            }


            MedicoDialog dialog =
                new MedicoDialog(
                    medico);


            dialog.Owner =
                Window.GetWindow(this);


            if (dialog.ShowDialog() ==
                true)
            {
                CargarMedicos();
            }
        }


        // ============================================================
        // ACTIVAR / DESACTIVAR
        // ============================================================

        private void btnCambiarEstado_Click(
            object sender,
            RoutedEventArgs e)
        {
            if ((sender as Button)
                ?.DataContext
                is not Medico medico)
            {
                return;
            }


            bool nuevoEstado =
                !medico.Activo;


            string accion =
                nuevoEstado
                    ? "activar"
                    : "desactivar";


            MessageBoxResult respuesta =
                MessageBox.Show(
                    $"¿Desea {accion} al médico " +
                    $"{medico.NombreCompleto}?",
                    "MediSys",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (respuesta !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                medicoDAO
                    .CambiarEstado(
                        medico.IdMedico,
                        nuevoEstado);


                CargarMedicos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cambiar el estado del médico.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}