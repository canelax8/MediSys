using Microsoft.Win32;
using Proyecto_MediSys.Models;
using Proyecto_MediSys.Services;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;





namespace Proyecto_MediSys.Controls
{
    /// <summary>
    /// Lógica de interacción para PacienteDocumentosDialog.xaml
    /// </summary>
    public partial class PacienteDocumentosDialog : UserControl
    {
        private PacienteDialog formularioPaciente;

        public PacienteDocumentosDialog(PacienteDialog formulario)
        {
            InitializeComponent();

            formularioPaciente = formulario;

            dgDocumentos.ItemsSource = formularioPaciente.Documentos;
        }

        
        //boton agregar documento
        private void btnAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivoSeleccionado))
            {
                MessageBox.Show(
                    "Debe seleccionar un archivo.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (cmbTipoDocumento.SelectedIndex <= 0)
            {
                MessageBox.Show(
                    "Seleccione el tipo de documento.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            FileInfo archivo = new FileInfo(rutaArchivoSeleccionado);

            foreach (var doc in formularioPaciente.Documentos)
            {
                if (doc.RutaArchivo == rutaArchivoSeleccionado)
                {
                    MessageBox.Show(
                        "Ese documento ya fue agregado.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }
            }

                    formularioPaciente.Documentos.Add(
            new DocumentoPaciente
            {
                TipoDocumento = ((ComboBoxItem)cmbTipoDocumento.SelectedItem).Content.ToString(),

                NombreArchivo = archivo.Name,

                RutaArchivo = archivo.FullName,

                FechaRegistro = DateTime.Now,

                Tamano = (archivo.Length / 1024.0).ToString("N1") + " KB",

                Estado = "Activo"
            });

            formularioPaciente.ActualizarEstadoDocumentos();

            MessageBox.Show(
                "Documento agregado correctamente.",
                "MediSys",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            LimpiarFormulario();
        }


        private string rutaArchivoSeleccionado = "";


        //boton examinar
        private void btnExaminar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter =
                "Archivos|*.jpg;*.jpeg;*.png;*.pdf";

            if (dialog.ShowDialog() == true)
            {
                rutaArchivoSeleccionado = dialog.FileName;

                txtRutaArchivo.Text = Path.GetFileName(rutaArchivoSeleccionado);

                string extension = System.IO.Path.GetExtension(rutaArchivoSeleccionado).ToLower();

                if (extension == ".jpg" ||
                    extension == ".jpeg" ||
                    extension == ".png")
                {
                    BitmapImage imagen = new BitmapImage();

                    imagen.BeginInit();

                    imagen.UriSource = new Uri(rutaArchivoSeleccionado);

                    imagen.CacheOption = BitmapCacheOption.OnLoad;

                    imagen.EndInit();

                    imgVistaPrevia.Source = imagen;

                    imgVistaPrevia.Visibility = Visibility.Visible;

                    panelSinDocumento.Visibility = Visibility.Collapsed;
                }
                else
                {
                    imgVistaPrevia.Source = null;

                    imgVistaPrevia.Visibility = Visibility.Collapsed;

                    panelSinDocumento.Visibility = Visibility.Visible;
                }
            }
        }



        //boton ver
        private void btnVer_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocumentos.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione un documento.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            DocumentoPaciente documento =
                (DocumentoPaciente)dgDocumentos.SelectedItem;

            if (!File.Exists(documento.RutaArchivo))
            {
                MessageBox.Show(
                    "El archivo ya no existe.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            Process.Start(new ProcessStartInfo()
            {
                FileName = documento.RutaArchivo,
                UseShellExecute = true
            });
        }

        //boton descargar
        private void btnDescargar_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocumentos.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione un documento.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            DocumentoPaciente documento =
                (DocumentoPaciente)dgDocumentos.SelectedItem;

            SaveFileDialog guardar = new SaveFileDialog();

            guardar.FileName = documento.NombreArchivo;

            guardar.Filter = "Todos los archivos|*.*";

            if (guardar.ShowDialog() == true)
            {
                File.Copy(
                    documento.RutaArchivo,
                    guardar.FileName,
                    true);

                MessageBox.Show(
                    "Documento descargado correctamente.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        //boton eliminar
        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocumentos.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione un documento.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            DocumentoPaciente documento =
                (DocumentoPaciente)dgDocumentos.SelectedItem;

            MessageBoxResult respuesta =
                MessageBox.Show(
                    $"¿Desea eliminar el documento:\n\n{documento.NombreArchivo}?",
                    "Eliminar documento",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (respuesta == MessageBoxResult.Yes)
            {
                formularioPaciente.Documentos.Remove(documento);

                formularioPaciente.ActualizarEstadoDocumentos();

                MessageBox.Show(
                    "Documento eliminado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        //boton imprimir
        private void btnImprimir_Click(object sender, RoutedEventArgs e)
        {
            if (dgDocumentos.SelectedItem == null)
            {
                MessageBox.Show(
                    "Seleccione un documento.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            DocumentoPaciente documento =
                (DocumentoPaciente)dgDocumentos.SelectedItem;

            if (!File.Exists(documento.RutaArchivo))
            {
                MessageBox.Show(
                    "El archivo no existe.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            try
            {
                ProcessStartInfo info = new ProcessStartInfo(documento.RutaArchivo);

                info.Verb = "print";

                info.UseShellExecute = true;

                Process.Start(info);
            }
            catch
            {
                MessageBox.Show(
                    "No fue posible imprimir el documento.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        //boton cerrar
        private async void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            formularioPaciente.ActualizarEstadoDocumentos();

            MaterialDesignThemes.Wpf.DialogHost.CloseDialogCommand.Execute(null, this);

            await Task.Delay(150);

            await DialogService.Mostrar(formularioPaciente);
        }

        //metodo limpiar formulario
        private void LimpiarFormulario()
        {
            cmbTipoDocumento.SelectedIndex = 0;

            txtRutaArchivo.Clear();

            rutaArchivoSeleccionado = "";

            imgVistaPrevia.Source = null;

            imgVistaPrevia.Visibility = Visibility.Collapsed;

            panelSinDocumento.Visibility = Visibility.Visible;
        }


    }


}
