using Proyecto_MediSys.Models;
using Proyecto_MediSys.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Proyecto_MediSys.Controls
{
    /// <summary>
    /// Lógica de interacción para VitalSignCard.xaml
    /// </summary>
    public partial class VitalSignCard : UserControl
    {
        public VitalSignCard()
        {
            InitializeComponent();
        }

        private void ActualizarColorTarjeta(Brush color)
        {
            card.BorderBrush = color;
            card.BorderThickness = new Thickness(2);

            card.Background = Brushes.White;
        }

        public void EstablecerValor(string valor)
        {
            txtValor.Text = valor;
        }

        private void txtValor_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9./]");

            e.Handled = regex.IsMatch(e.Text);
        }

        public string Titulo
        {
            get { return txtTitulo.Text; }
            set { txtTitulo.Text = value; }
        }

        public string Descripcion
        {
            get { return txtDescripcion.Text; }
            set { txtDescripcion.Text = value; }
        }

        public string Estado
        {
            get { return txtEstado.Text; }
            set { txtEstado.Text = value; }
        }

        public string Valor
        {
            get { return txtValor.Text; }
            set { txtValor.Text = value; }
        }

        public string Unidad
        {
            get { return txtUnidad.Text; }
            set { txtUnidad.Text = value; }
        }

        public TipoSignoVital Tipo { get; set; }

        public string Placeholder { get; set; } = "";

        private void txtValor_TextChanged(object sender, TextChangedEventArgs e)
        {
            VitalSignResult resultado = VitalSignValidator.Validar(Tipo,txtValor.Text);
            txtEstado.Text = $"Estado: {resultado.Estado}";
            txtEstado.Foreground = resultado.Color;

            if (string.IsNullOrWhiteSpace(resultado.Estado))
            {
                card.BorderBrush = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#D6E4F0"));

                card.BorderThickness = new Thickness(1);
            }
            else
            {
                ActualizarColorTarjeta(resultado.Color);
            }
        }

        public string ObtenerValor()
        {
            return txtValor.Text.Trim();
        }

        




    }
}
