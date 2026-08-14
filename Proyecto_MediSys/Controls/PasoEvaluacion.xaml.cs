using Proyecto_MediSys.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Proyecto_MediSys.Controls
{
    public partial class PasoEvaluacion : UserControl
    {
        private int nivelTriage = 0;


        public PasoEvaluacion()
        {
            InitializeComponent();

            ConfigurarSignosVitales();

        }

        private void ConfigurarSignosVitales()
        {
            cardTemperatura.Titulo = "🌡 Temperatura";
            cardTemperatura.Descripcion = "Temperatura corporal";
            cardTemperatura.Unidad = "°C";
            cardTemperatura.Estado = "🟢 Normal";
            cardTemperatura.Tipo = TipoSignoVital.Temperatura;


            cardPresion.Titulo = "🩸 Presión arterial";
            cardPresion.Descripcion = "Presión sistólica / diastólica";
            cardPresion.Unidad = "mmHg";
            cardPresion.Estado = "🟢 Normal";
            cardPresion.Tipo = TipoSignoVital.Presion;

            cardFrecuenciaCardiaca.Titulo = "❤️ Frecuencia Cardíaca";
            cardFrecuenciaCardiaca.Descripcion = "Latidos por minuto";
            cardFrecuenciaCardiaca.Unidad = "lpm";
            cardFrecuenciaCardiaca.Estado = "🟢 Normal";
            cardFrecuenciaCardiaca.Tipo = TipoSignoVital.FrecuenciaCardiaca;

            cardFrecuenciaRespiratoria.Titulo = "🫁 Frecuencia Respiratoria";
            cardFrecuenciaRespiratoria.Descripcion = "Respiraciones por minuto";
            cardFrecuenciaRespiratoria.Unidad = "rpm";
            cardFrecuenciaRespiratoria.Estado = "🟢 Normal";
            cardFrecuenciaRespiratoria.Tipo = TipoSignoVital.FrecuenciaRespiratoria;

            cardSaturacion.Titulo = "🫧 Saturación O₂";
            cardSaturacion.Descripcion = "Oxígeno en sangre";
            cardSaturacion.Unidad = "%";
            cardSaturacion.Estado = "🟢 Normal";
            cardSaturacion.Tipo = TipoSignoVital.Saturacion;

            cardGlucemia.Titulo = "🩸 Glucemia";
            cardGlucemia.Descripcion = "Nivel de glucosa";
            cardGlucemia.Unidad = "mg/dL";
            cardGlucemia.Estado = "🟢 Normal";
            cardGlucemia.Tipo = TipoSignoVital.Glucemia;

            cardPeso.Titulo = "⚖ Peso";
            cardPeso.Descripcion = "Peso del paciente";
            cardPeso.Unidad = "kg";
            cardPeso.Estado = "";
            cardPeso.Tipo = TipoSignoVital.Peso;

            cardTalla.Titulo = "📏 Talla";
            cardTalla.Descripcion = "Estatura";
            cardTalla.Unidad = "cm";
            cardTalla.Estado = "";
            cardTalla.Tipo = TipoSignoVital.Talla;
        }




        //-------------------------------------------------
        // Cargar información del paciente
        //-------------------------------------------------

        public void CargarPaciente(Paciente paciente)
        {
            txtNombrePaciente.Text = paciente.NombreCompleto;
            txtDocumento.Text = paciente.DocumentoMostrar;
            txtEdad.Text = paciente.Edad;
            txtSeguro.Text = paciente.NombreSeguro;
        }

        public void CargarEvaluacion(EvaluacionInicial evaluacion)
        {
            if (evaluacion == null)
                return;

            // =========================
            // TRIAGE
            // =========================

            switch (evaluacion.NivelTriage)
            {
                case 1:
                    SeleccionarTriage(
                        cardTriage1,
                        1,
                        "Nivel I - Reanimación");
                    break;

                case 2:
                    SeleccionarTriage(
                        cardTriage2,
                        2,
                        "Nivel II - Emergencia");
                    break;

                case 3:
                    SeleccionarTriage(
                        cardTriage3,
                        3,
                        "Nivel III - Urgencia");
                    break;

                case 4:
                    SeleccionarTriage(
                        cardTriage4,
                        4,
                        "Nivel IV - Menor urgencia");
                    break;

                case 5:
                    SeleccionarTriage(
                        cardTriage5,
                        5,
                        "Nivel V - No urgente");
                    break;
            }

            // =========================
            // SIGNOS VITALES
            // =========================

            cardTemperatura.EstablecerValor(
                evaluacion.Temperatura?.ToString() ?? "");

            cardPresion.EstablecerValor(
                evaluacion.PresionArterial ?? "");

            cardFrecuenciaCardiaca.EstablecerValor(
                evaluacion.FrecuenciaCardiaca?.ToString() ?? "");

            cardFrecuenciaRespiratoria.EstablecerValor(
                evaluacion.FrecuenciaRespiratoria?.ToString() ?? "");

            cardSaturacion.EstablecerValor(
                evaluacion.Saturacion?.ToString() ?? "");

            cardGlucemia.EstablecerValor(
                evaluacion.Glucemia?.ToString() ?? "");

            cardPeso.EstablecerValor(
                evaluacion.Peso?.ToString() ?? "");

            cardTalla.EstablecerValor(
                evaluacion.Talla?.ToString() ?? "");
        }

        private void LimpiarSeleccion()
        {
            cardTriage1.BorderThickness = new Thickness(0);
            cardTriage2.BorderThickness = new Thickness(0);
            cardTriage3.BorderThickness = new Thickness(0);
            cardTriage4.BorderThickness = new Thickness(0);
            cardTriage5.BorderThickness = new Thickness(0);

            cardTriage1.RenderTransform = null;
            cardTriage2.RenderTransform = null;
            cardTriage3.RenderTransform = null;
            cardTriage4.RenderTransform = null;
            cardTriage5.RenderTransform = null;
        }

        private void SeleccionarTriage(Border tarjeta, int nivel, string texto)
        {
            LimpiarSeleccion();

            nivelTriage = nivel;

            tarjeta.BorderBrush = Brushes.White;
            tarjeta.BorderThickness = new Thickness(3);

            tarjeta.RenderTransformOrigin = new Point(0.5, 0.5);
            tarjeta.RenderTransform = new ScaleTransform(1.08, 1.08);

            txtTriageSeleccionado.Text = texto;
        }

        private void cardTriage1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SeleccionarTriage(cardTriage1, 1, "Nivel I - Reanimación");
        }

        private void cardTriage2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SeleccionarTriage(cardTriage2, 2, "Nivel II - Emergencia");
        }

        private void cardTriage3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SeleccionarTriage(cardTriage3, 3, "Nivel III - Urgencia");
        }

        private void cardTriage4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SeleccionarTriage(cardTriage4, 4, "Nivel IV - Menor urgencia");
        }

        private void cardTriage5_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SeleccionarTriage(cardTriage5, 5, "Nivel V - No urgente");
        }

        public EvaluacionInicial ObtenerEvaluacion()
        {
            EvaluacionInicial evaluacion = new();

            evaluacion.NivelTriage = nivelTriage;

            if (decimal.TryParse(cardTemperatura.ObtenerValor(), out decimal temperatura))
                evaluacion.Temperatura = temperatura;

            evaluacion.PresionArterial = cardPresion.ObtenerValor();

            if (int.TryParse(cardFrecuenciaCardiaca.ObtenerValor(), out int fc))
                evaluacion.FrecuenciaCardiaca = fc;

            if (int.TryParse(cardFrecuenciaRespiratoria.ObtenerValor(), out int fr))
                evaluacion.FrecuenciaRespiratoria = fr;

            if (int.TryParse(cardSaturacion.ObtenerValor(), out int sat))
                evaluacion.Saturacion = sat;

            if (decimal.TryParse(cardGlucemia.ObtenerValor(), out decimal glucemia))
                evaluacion.Glucemia = glucemia;

            if (decimal.TryParse(cardPeso.ObtenerValor(), out decimal peso))
                evaluacion.Peso = peso;

            if (decimal.TryParse(cardTalla.ObtenerValor(), out decimal talla))
                evaluacion.Talla = talla;

            return evaluacion;
        }
    }
    
}