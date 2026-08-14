using Proyecto_MediSys.Controls;
using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Proyecto_MediSys.Helpers;

namespace Proyecto_MediSys.Pages.Mantenimientos
{
    /// <summary>
    /// Lógica de interacción para UsuariosPage.xaml
    /// </summary>
    public partial class UsuariosPage : Page
    { 
        private readonly UsuarioDAO dao = new UsuarioDAO();

        private List<Usuario> listaUsuarios = new();

        public UsuariosPage()
        {
            InitializeComponent();

            CargarUsuarios();
        }

        /*metodo para cargar los usuarios en el datagrid*/
        private void CargarUsuarios()
        {
            listaUsuarios = dao.ObtenerTodos();

            dgUsuarios.ItemsSource = null;
            dgUsuarios.ItemsSource = listaUsuarios;

            txtTotalUsuarios.Text = listaUsuarios.Count.ToString();
        }/*aqui termina el metodo para cargar */


        /*metodo para buscar los usuarios en el datagrid*/
        private void txtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            listaUsuarios = dao.Buscar(txtBuscar.Text);

            dgUsuarios.ItemsSource = listaUsuarios;

            txtTotalUsuarios.Text = listaUsuarios.Count.ToString();
        }/*aqui termina el metodo para buscar los usuarios en el datagrid*/

        /*metodo para abrir el dialogo de nuevo usuario*/
        private void btnNuevoUsuario_Click(object sender, RoutedEventArgs e)
        {
            Controls.UsuarioDialog dialog = new Controls.UsuarioDialog();

            dialog.Owner = Window.GetWindow(this);

            dialog.ShowDialog();

            if (dialog.UsuarioGuardado)
            {
                CargarUsuarios();
            }
        }/*aqui termina el metodo para abrir el dialogo de nuevo usuario*/

        private void dgUsuarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnVer_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsuarios.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione un usuario.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            Usuario usuario = (Usuario)dgUsuarios.SelectedItem;

            UsuarioDialog dialog =
                new UsuarioDialog(usuario, ModoFormulario.Ver);

            dialog.Owner = Window.GetWindow(this);

            dialog.ShowDialog();
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsuarios.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione un usuario.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            Usuario usuario = (Usuario)dgUsuarios.SelectedItem;

            UsuarioDialog dialog = new UsuarioDialog(usuario);

            dialog.Owner = Window.GetWindow(this);

            dialog.ShowDialog();

            if (dialog.UsuarioGuardado)
            {
                CargarUsuarios();
            }
        }

       

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsuarios.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione un usuario.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            Usuario usuario = (Usuario)dgUsuarios.SelectedItem;

            MessageBoxResult respuesta = MessageBox.Show(
                $"¿Desea desactivar al usuario '{usuario.NombreCompleto}'?",
                "MediSys",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (respuesta != MessageBoxResult.Yes)
                return;

            bool resultado = dao.Eliminar(usuario.IdUsuario);

            if (resultado)
            {
                MessageBox.Show(
                    "Usuario desactivado correctamente.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                CargarUsuarios();
            }
        }




    }
}
