using Proyecto_MediSys.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Proyecto_MediSys.Controls
{
    public partial class PasoResumen : UserControl
    {
        private readonly ProcesoEmergencia proceso;


        public PasoResumen(ProcesoEmergencia proceso)
        {
            InitializeComponent();

            this.proceso = proceso;

            CargarResumen();
        }


        // ============================================================
        // CARGAR RESUMEN
        // ============================================================

        private void CargarResumen()
        {
            CargarPaciente();

            CargarEvaluacion();

            CargarInformacionClinica();

            CargarDiagnosticos();

            CargarItemsClinicos();

            CargarDestino();
        }


        // ============================================================
        // PACIENTE
        // ============================================================

        private void CargarPaciente()
        {
            if (proceso.Paciente == null)
                return;


            txtPaciente.Text =
                proceso.Paciente.NombreCompleto;


            txtDocumento.Text =
                proceso.Paciente.DocumentoMostrar;


            txtEdad.Text =
                proceso.Paciente.Edad;


            txtSeguro.Text =
                string.IsNullOrWhiteSpace(
                    proceso.Paciente.NombreSeguro)
                    ? "Sin seguro"
                    : proceso.Paciente.NombreSeguro;
        }


        // ============================================================
        // EVALUACIÓN INICIAL
        // ============================================================

        private void CargarEvaluacion()
        {
            if (proceso.Evaluacion == null)
                return;


            txtTriage.Text =
                ObtenerNombreTriage(
                    proceso.Evaluacion.NivelTriage);


            txtTemperatura.Text =
                proceso.Evaluacion.Temperatura.HasValue
                    ? $"{proceso.Evaluacion.Temperatura.Value:N1} °C"
                    : "No registrada";


            txtPresion.Text =
                string.IsNullOrWhiteSpace(
                    proceso.Evaluacion.PresionArterial)
                    ? "No registrada"
                    : proceso.Evaluacion.PresionArterial;


            txtFrecuenciaCardiaca.Text =
                proceso.Evaluacion.FrecuenciaCardiaca.HasValue
                    ? $"{proceso.Evaluacion.FrecuenciaCardiaca} lpm"
                    : "No registrada";


            txtFrecuenciaRespiratoria.Text =
                proceso.Evaluacion.FrecuenciaRespiratoria.HasValue
                    ? $"{proceso.Evaluacion.FrecuenciaRespiratoria} rpm"
                    : "No registrada";


            txtSaturacion.Text =
                proceso.Evaluacion.Saturacion.HasValue
                    ? $"{proceso.Evaluacion.Saturacion}%"
                    : "No registrada";


            txtGlucemia.Text =
                proceso.Evaluacion.Glucemia.HasValue
                    ? $"{proceso.Evaluacion.Glucemia.Value:N1} mg/dL"
                    : "No registrada";


            txtPeso.Text =
                proceso.Evaluacion.Peso.HasValue
                    ? $"{proceso.Evaluacion.Peso.Value:N1} kg"
                    : "No registrado";


            txtTalla.Text =
                proceso.Evaluacion.Talla.HasValue
                    ? $"{proceso.Evaluacion.Talla.Value:N1} cm"
                    : "No registrada";
        }


        // ============================================================
        // INFORMACIÓN CLÍNICA
        // ============================================================

        private void CargarInformacionClinica()
        {
            if (proceso.InformacionClinica == null)
                return;


            txtMotivo.Text =
                ValorOAlternativa(
                    proceso.InformacionClinica.MotivoConsulta);


            txtAntecedentes.Text =
                ObtenerAntecedentes();


            txtAlergias.Text =
                ValorOAlternativa(
                    proceso.InformacionClinica.Alergias);


            txtMedicamentosHabituales.Text =
                ValorOAlternativa(
                    proceso.InformacionClinica.MedicamentosActuales);


            txtObservacionesClinicas.Text =
                ValorOAlternativa(
                    proceso.InformacionClinica.Observaciones);
        }


        // ============================================================
        // DIAGNÓSTICOS
        // ============================================================

        private void CargarDiagnosticos()
        {
            if (proceso.Diagnostico == null)
                return;


            // Principal CIE-10

            if (proceso.DiagnosticoPrincipalCIE10 != null)
            {
                txtDiagnosticoPrincipal.Text =
                    proceso.DiagnosticoPrincipalCIE10.Mostrar;
            }
            else
            {
                txtDiagnosticoPrincipal.Text =
                    ValorOAlternativa(
                        proceso.Diagnostico.DiagnosticoPrincipal);
            }


            // Otros CIE-10

            if (proceso.DiagnosticosSeleccionados != null &&
                proceso.DiagnosticosSeleccionados.Count > 0)
            {
                List<CIE10> secundarios =
                    proceso.DiagnosticosSeleccionados
                    .Where(c =>
                        proceso.DiagnosticoPrincipalCIE10 == null
                        ||
                        c.IdCIE10 !=
                        proceso.DiagnosticoPrincipalCIE10.IdCIE10)
                    .ToList();


                txtDiagnosticosCIE10.Text =
                    secundarios.Count > 0
                        ? string.Join(
                            "\n",
                            secundarios.Select(
                                c => "• " + c.Mostrar))
                        : "Sin diagnósticos secundarios.";
            }
            else
            {
                txtDiagnosticosCIE10.Text =
                    "Sin diagnósticos secundarios.";
            }


            // Manuales

            if (proceso.DiagnosticosManuales != null &&
                proceso.DiagnosticosManuales.Count > 0)
            {
                txtDiagnosticosManuales.Text =
                    string.Join(
                        "\n",
                        proceso.DiagnosticosManuales
                        .Where(d =>
                            !string.IsNullOrWhiteSpace(d))
                        .Select(d =>
                            "• " + d));
            }
            else
            {
                txtDiagnosticosManuales.Text =
                    "Ninguno.";
            }


            txtImpresionClinica.Text =
                ValorOAlternativa(
                    proceso.Diagnostico.ImpresionClinica);


            txtObservacionesMedicas.Text =
                ValorOAlternativa(
                    proceso.Diagnostico.Observaciones);
        }


        // ============================================================
        // TRATAMIENTO / SERVICIOS
        // ============================================================

        private void CargarItemsClinicos()
        {
            if (proceso.ItemsClinicos == null)
            {
                dgItemsClinicos.ItemsSource = null;

                txtSubtotalResumen.Text =
                    "RD$ 0.00";

                txtPlanTarifarioResumen.Text =
                    "Sin plan";

                return;
            }


            dgItemsClinicos.ItemsSource =
                proceso.ItemsClinicos;


            decimal subtotal =
                proceso.ItemsClinicos.Sum(
                    i => i.Total);


            txtSubtotalResumen.Text =
                $"RD$ {subtotal:N2}";


            string plan =
                proceso.ItemsClinicos
                    .Where(i =>
                        !string.IsNullOrWhiteSpace(
                            i.NombrePlanTarifario))
                    .Select(i =>
                        i.NombrePlanTarifario)
                    .FirstOrDefault()
                ?? "";


            txtPlanTarifarioResumen.Text =
                string.IsNullOrWhiteSpace(plan)
                    ? "No determinado"
                    : plan;
        }


        // ============================================================
        // DESTINO
        // ============================================================

        private void CargarDestino()
        {
            if (proceso.Destino == null)
                return;


            txtDestino.Text =
                ValorOAlternativa(
                    proceso.Destino.Destino);


            txtObservaciones.Text =
                ValorOAlternativa(
                    proceso.Destino.ObservacionesFinales);
        }


        // ============================================================
        // ANTECEDENTES
        // ============================================================

        private string ObtenerAntecedentes()
        {
            List<string> antecedentes =
                new();


            if (proceso.InformacionClinica.Diabetes)
                antecedentes.Add("Diabetes");


            if (proceso.InformacionClinica.Hipertension)
                antecedentes.Add("Hipertensión");


            if (proceso.InformacionClinica.Asma)
                antecedentes.Add("Asma");


            if (proceso.InformacionClinica.Cardiopatia)
                antecedentes.Add("Cardiopatía");


            if (proceso.InformacionClinica.Embarazo)
                antecedentes.Add("Embarazo");


            if (proceso.InformacionClinica.Ninguno)
                antecedentes.Add(
                    "Sin antecedentes conocidos");


            if (antecedentes.Count == 0)
                return "No especificados";


            return string.Join(
                ", ",
                antecedentes);
        }


        // ============================================================
        // TRIAGE
        // ============================================================

        private string ObtenerNombreTriage(
            int nivel)
        {
            return nivel switch
            {
                1 => "Nivel I - Reanimación",
                2 => "Nivel II - Emergencia",
                3 => "Nivel III - Urgencia",
                4 => "Nivel IV - Menor urgencia",
                5 => "Nivel V - No urgente",
                _ => "No seleccionado"
            };
        }


        // ============================================================
        // TEXTO VACÍO
        // ============================================================

        private string ValorOAlternativa(
            string? valor)
        {
            return string.IsNullOrWhiteSpace(valor)
                ? "No especificado"
                : valor;
        }
    }
}