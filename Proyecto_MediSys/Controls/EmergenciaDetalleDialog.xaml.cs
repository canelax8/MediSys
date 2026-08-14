using Microsoft.Win32;
using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using Proyecto_MediSys.Services.PDF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Controls
{
    public partial class EmergenciaDetalleDialog : Window
    {
        private Emergencia emergencia;

        private ProcesoEmergencia proceso;


        public EmergenciaDetalleDialog(
            Emergencia emergencia)
        {
            InitializeComponent();

            this.emergencia =
                emergencia;


            EmergenciaDAO dao =
                new EmergenciaDAO();


            var resultado =
                dao.ObtenerPorId(
                    emergencia.IdEmergencia);


            if (resultado.Emergencia == null ||
                resultado.Proceso == null)
            {
                MessageBox.Show(
                    "No fue posible cargar la información completa de la emergencia.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Close();

                return;
            }


            this.emergencia =
                resultado.Emergencia;


            this.proceso =
                resultado.Proceso;


            CargarDatos();
        }


        // =========================================================
        // CARGAR TODO EL EXPEDIENTE
        // =========================================================

        private void CargarDatos()
        {
            CargarEncabezado();

            CargarPaciente();

            CargarEvaluacion();

            CargarInformacionClinica();

            CargarDiagnosticos();

            CargarItemsClinicos();

            CargarDestino();

            CargarMedico();
        }


        // =========================================================
        // ENCABEZADO
        // =========================================================

        private void CargarEncabezado()
        {
            txtCodigo.Text =
                emergencia.CodigoEmergencia;


            txtPacienteHeader.Text =
                emergencia.NombrePaciente;


            txtEstado.Text =
                emergencia.Estado;
        }


        // =========================================================
        // PACIENTE
        // =========================================================

        private void CargarPaciente()
        {
            Paciente paciente =
                proceso.Paciente;


            txtNombrePaciente.Text =
                paciente.NombreCompleto;


            txtDocumento.Text =
                paciente.DocumentoMostrar;


            txtEdad.Text =
                paciente.Edad;


            txtSexo.Text =
                Texto(
                    paciente.Sexo);


            txtTelefono.Text =
                Texto(
                    paciente.Telefono);


            txtDireccion.Text =
                Texto(
                    paciente.Direccion);


            txtSeguro.Text =
                string.IsNullOrWhiteSpace(
                    paciente.NombreSeguro)

                    ? "Sin seguro"

                    : paciente.NombreSeguro;
        }


        // =========================================================
        // EVALUACIÓN INICIAL
        // =========================================================

        private void CargarEvaluacion()
        {
            EvaluacionInicial evaluacion =
                proceso.Evaluacion;


            txtTriage.Text =
                ObtenerTriage(
                    evaluacion.NivelTriage);


            txtTemperatura.Text =
                evaluacion.Temperatura.HasValue

                    ? $"{evaluacion.Temperatura.Value:N1} °C"

                    : "—";


            txtPresion.Text =
                Texto(
                    evaluacion.PresionArterial);


            txtFrecuenciaCardiaca.Text =
                evaluacion.FrecuenciaCardiaca.HasValue

                    ? $"{evaluacion.FrecuenciaCardiaca.Value} lpm"

                    : "—";


            txtFrecuenciaRespiratoria.Text =
                evaluacion.FrecuenciaRespiratoria.HasValue

                    ? $"{evaluacion.FrecuenciaRespiratoria.Value} rpm"

                    : "—";


            txtSaturacion.Text =
                evaluacion.Saturacion.HasValue

                    ? $"{evaluacion.Saturacion.Value}%"

                    : "—";


            txtGlucemia.Text =
                evaluacion.Glucemia.HasValue

                    ? $"{evaluacion.Glucemia.Value:N1} mg/dL"

                    : "—";


            txtPeso.Text =
                evaluacion.Peso.HasValue

                    ? $"{evaluacion.Peso.Value:N1} kg"

                    : "—";


            txtTalla.Text =
                evaluacion.Talla.HasValue

                    ? $"{evaluacion.Talla.Value:N1} cm"

                    : "—";
        }


        // =========================================================
        // INFORMACIÓN CLÍNICA
        // =========================================================

        private void CargarInformacionClinica()
        {
            InformacionClinica informacion =
                proceso.InformacionClinica;


            txtMotivo.Text =
                Texto(
                    informacion.MotivoConsulta);


            txtAntecedentes.Text =
                ConstruirAntecedentes(
                    informacion);


            txtAlergias.Text =
                Texto(
                    informacion.Alergias);


            txtMedicamentos.Text =
                Texto(
                    informacion.MedicamentosActuales);


            txtObservacionesClinicas.Text =
                Texto(
                    informacion.Observaciones);
        }


        // =========================================================
        // DIAGNÓSTICOS
        // =========================================================

        private void CargarDiagnosticos()
        {
            DiagnosticoEmergencia diagnostico =
                proceso.Diagnostico;


            // =====================================================
            // PRINCIPAL
            // =====================================================

            if (proceso.DiagnosticoPrincipalCIE10
                != null)
            {
                txtDiagnosticoPrincipal.Text =
                    proceso
                    .DiagnosticoPrincipalCIE10
                    .Mostrar;
            }
            else
            {
                txtDiagnosticoPrincipal.Text =
                    Texto(
                        diagnostico
                        .DiagnosticoPrincipal);
            }


            // =====================================================
            // SECUNDARIOS CIE-10
            // =====================================================

            List<CIE10> secundarios =
                new List<CIE10>();


            if (proceso.DiagnosticosSeleccionados
                != null)
            {
                secundarios =
                    proceso
                    .DiagnosticosSeleccionados
                    .Where(cie =>
                        proceso
                        .DiagnosticoPrincipalCIE10
                        == null

                        ||

                        cie.IdCIE10 !=
                        proceso
                        .DiagnosticoPrincipalCIE10
                        .IdCIE10)
                    .ToList();
            }


            txtDiagnosticoSecundario.Text =
                secundarios.Count > 0

                    ? string.Join(
                        "\n",
                        secundarios.Select(
                            x => "• " + x.Mostrar))

                    : "Sin diagnósticos secundarios.";


            // =====================================================
            // MANUALES
            // =====================================================

            if (proceso.DiagnosticosManuales != null &&
                proceso.DiagnosticosManuales.Count > 0)
            {
                txtDiagnosticosManuales.Text =
                    string.Join(
                        "\n",
                        proceso.DiagnosticosManuales
                        .Where(x =>
                            !string.IsNullOrWhiteSpace(x))
                        .Select(x =>
                            "• " + x));
            }
            else
            {
                txtDiagnosticosManuales.Text =
                    "Ninguno.";
            }


            // =====================================================
            // IMPRESIÓN
            // =====================================================

            txtImpresionClinica.Text =
                Texto(
                    diagnostico.ImpresionClinica);


            // =====================================================
            // OBSERVACIONES MÉDICAS
            // =====================================================

            txtObservacionesMedicas.Text =
                Texto(
                    diagnostico.Observaciones);
        }


        // =========================================================
        // ITEMS CLÍNICOS / SERVICIOS
        // =========================================================

        private void CargarItemsClinicos()
        {
            if (proceso.ItemsClinicos == null ||
                proceso.ItemsClinicos.Count == 0)
            {
                dgItemsClinicos.ItemsSource =
                    null;


                txtPlanTarifario.Text =
                    "No determinado";


                txtSubtotal.Text =
                    "RD$ 0.00";


                return;
            }


            // =====================================================
            // TABLA
            // =====================================================

            dgItemsClinicos.ItemsSource =
                proceso.ItemsClinicos;


            // =====================================================
            // SUBTOTAL
            // =====================================================

            decimal subtotal =
                proceso.ItemsClinicos.Sum(
                    x => x.Cantidad *
                         x.PrecioUnitarioAplicado);


            txtSubtotal.Text =
                $"RD$ {subtotal:N2}";


            // =====================================================
            // PLAN TARIFARIO
            // =====================================================

            string? plan =
                proceso.ItemsClinicos
                    .Where(x =>
                        !string.IsNullOrWhiteSpace(
                            x.NombrePlanTarifario))
                    .Select(x =>
                        x.NombrePlanTarifario)
                    .FirstOrDefault();


            txtPlanTarifario.Text =
                string.IsNullOrWhiteSpace(plan)

                    ? "No determinado"

                    : plan;
        }


        // =========================================================
        // DESTINO
        // =========================================================

        private void CargarDestino()
        {
            DestinoEmergencia destino =
                proceso.Destino;


            txtDestino.Text =
                Texto(
                    destino.Destino);


            txtFechaSalida.Text =
                destino.FechaSalida.HasValue

                    ? destino.FechaSalida
                        .Value
                        .ToString(
                            "dd/MM/yyyy HH:mm")

                    : "Pendiente";


            txtObservacionesFinales.Text =
                Texto(
                    destino
                    .ObservacionesFinales);
        }


        // =========================================================
        // MÉDICO
        // =========================================================

        private void CargarMedico()
        {
            txtMedico.Text =
                Texto(
                    emergencia.NombreMedico);


            txtEspecialidad.Text =
                Texto(
                    emergencia.Especialidad);
        }


        // =========================================================
        // CONSTRUIR ANTECEDENTES
        // =========================================================

        private string ConstruirAntecedentes(
            InformacionClinica informacion)
        {
            if (informacion.Ninguno)
            {
                return
                    "Sin antecedentes conocidos";
            }


            List<string> antecedentes =
                new List<string>();


            if (informacion.Diabetes)
                antecedentes.Add(
                    "Diabetes");


            if (informacion.Hipertension)
                antecedentes.Add(
                    "Hipertensión");


            if (informacion.Asma)
                antecedentes.Add(
                    "Asma");


            if (informacion.Cardiopatia)
                antecedentes.Add(
                    "Cardiopatía");


            if (informacion.Embarazo)
                antecedentes.Add(
                    "Embarazo");


            return antecedentes.Count > 0

                ? string.Join(
                    ", ",
                    antecedentes)

                : "No especificados";
        }


        // =========================================================
        // TRIAGE
        // =========================================================

        private string ObtenerTriage(
            int nivel)
        {
            return nivel switch
            {
                1 =>
                    "Nivel I - Reanimación",

                2 =>
                    "Nivel II - Emergencia",

                3 =>
                    "Nivel III - Urgencia",

                4 =>
                    "Nivel IV - Menor urgencia",

                5 =>
                    "Nivel V - No urgente",

                _ =>
                    "No registrado"
            };
        }


        private void btnImprimir_Click(
    object sender,
    RoutedEventArgs e)
        {
            try
            {
                // =====================================================
                // 1. CREAR PDF TEMPORAL
                // =====================================================

                string carpetaTemporal =
                    Path.Combine(
                        Path.GetTempPath(),
                        "MediSys");

                Directory.CreateDirectory(
                    carpetaTemporal);


                string rutaPdf =
                    Path.Combine(
                        carpetaTemporal,
                        $"Expediente_{emergencia.CodigoEmergencia}.pdf");


                EmergenciaPdfService servicio =
                    new EmergenciaPdfService();


                servicio.Generar(
                    emergencia,
                    proceso,
                    rutaPdf);


                // =====================================================
                // 2. ABRIR DIÁLOGO DE IMPRESIÓN
                // =====================================================

                PrintDialog dialog =
                    new PrintDialog();


                bool? resultado =
                    dialog.ShowDialog();


                if (resultado != true)
                    return;


                // =====================================================
                // 3. ABRIR EL PDF PARA IMPRIMIR
                // =====================================================

                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = rutaPdf,

                        Verb = "print",

                        UseShellExecute = true,

                        CreateNoWindow = true,

                        WindowStyle =
                            ProcessWindowStyle.Hidden
                    });


                MessageBox.Show(
                    "El expediente fue enviado al sistema de impresión.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible imprimir el expediente.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // TEXTO SEGURO
        // =========================================================

        private string Texto(
            string? valor)
        {
            return string.IsNullOrWhiteSpace(
                valor)

                ? "No especificado"

                : valor;
        }


        // =========================================================
        // CERRAR
        // =========================================================

        private void btnCerrar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }


        // =========================================================
        // PDF
        // =========================================================

        private void btnPDF_Click(
     object sender,
     RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog dialog =
                    new SaveFileDialog
                    {
                        Title =
                            "Guardar expediente de emergencia",

                        Filter =
                            "Documento PDF (*.pdf)|*.pdf",

                        FileName =
                            $"Expediente_{emergencia.CodigoEmergencia}.pdf",

                        DefaultExt =
                            ".pdf",

                        AddExtension =
                            true
                    };


                bool? resultado =
                    dialog.ShowDialog();


                if (resultado != true)
                    return;


                EmergenciaPdfService servicio =
                    new EmergenciaPdfService();


                servicio.Generar(
                    emergencia,
                    proceso,
                    dialog.FileName);


                MessageBoxResult abrir =
                    MessageBox.Show(
                        "El expediente PDF fue generado correctamente.\n\n¿Desea abrirlo ahora?",
                        "MediSys",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Information);


                if (abrir ==
                    MessageBoxResult.Yes)
                {
                    Process.Start(
                        new ProcessStartInfo
                        {
                            FileName =
                                dialog.FileName,

                            UseShellExecute =
                                true
                        });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible generar el PDF.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}