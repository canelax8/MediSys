using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Proyecto_MediSys.Controls
{
    public partial class EmergenciaDialog : Window
    {
        // =========================================================
        // PASOS DEL WIZARD
        // =========================================================

        private PasoPaciente? pasoPaciente;

        private PasoEvaluacion? pasoEvaluacion;

        private PasoInformacionClinica? pasoInformacion;

        private PasoDiagnostico? pasoDiagnostico;

        private PasoProcedimientos? pasoProcedimientos;

        private PasoDestino? pasoDestino;

        private PasoResumen? pasoResumen;


        // =========================================================
        // PROCESO DE EMERGENCIA
        // =========================================================

        private Paciente? pacienteSeleccionado;

        private ProcesoEmergencia proceso =
            new ProcesoEmergencia();


        // =========================================================
        // NAVEGACIÓN
        // =========================================================

        private int pasoActual = 1;


        // =========================================================
        // MODO DEL FORMULARIO
        // =========================================================

        private bool esEdicion = false;

        private long idEmergenciaActual = 0;

        private Emergencia? emergenciaActual;

        // =========================================================
        // EMERGENCIA PROCESADA
        // =========================================================

        public long IdEmergenciaProcesada
        {
            get;
            private set;
        }


        // =========================================================
        // CONSTRUCTOR
        // NUEVA EMERGENCIA
        // =========================================================

        public EmergenciaDialog()
        {
            InitializeComponent();


            esEdicion =
                false;


            pasoActual =
                1;


            Title =
                "Nueva Emergencia";


            MostrarPaso();
        }


        // =========================================================
        // CONSTRUCTOR
        // ATENDER / CONTINUAR EMERGENCIA EXISTENTE
        // =========================================================

        public EmergenciaDialog(
            long idEmergencia)
        {
            InitializeComponent();


            esEdicion =
                true;


            idEmergenciaActual =
                idEmergencia;

            IdEmergenciaProcesada =
                idEmergencia;


            // =====================================================
            // CUANDO ATENDEMOS UNA EMERGENCIA EXISTENTE
            // EL PACIENTE YA ESTÁ SELECCIONADO.
            // EMPEZAMOS EN EVALUACIÓN.
            // =====================================================

            pasoActual =
                2;


            CargarEmergenciaExistente();


            if (emergenciaActual != null)
            {
                Title =
                    $"Atención de Emergencia - " +
                    $"{emergenciaActual.CodigoEmergencia}";
            }
            else
            {
                Title =
                    "Atención de Emergencia";
            }


            MostrarPaso();
        }


        // =========================================================
        // CARGAR EMERGENCIA EXISTENTE
        // =========================================================

        private void CargarEmergenciaExistente()
        {
            try
            {
                EmergenciaDAO dao =
                    new EmergenciaDAO();


                var resultado =
                    dao.ObtenerPorId(
                        idEmergenciaActual);


                if (resultado.Emergencia == null ||
                    resultado.Proceso == null)
                {
                    MessageBox.Show(
                        "No fue posible encontrar la emergencia seleccionada.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);


                    Close();

                    return;
                }


                emergenciaActual =
                    resultado.Emergencia;


                proceso =
                    resultado.Proceso;


                pacienteSeleccionado =
                    proceso.Paciente;


                if (pacienteSeleccionado == null)
                {
                    MessageBox.Show(
                        "No fue posible cargar el paciente asociado a esta emergencia.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);


                    Close();

                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar la emergencia.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);


                Close();
            }
        }


        // =========================================================
        // ACTUALIZAR SIDEBAR
        // =========================================================

        private void ActualizarSidebar()
        {
            Border[] tarjetas =
            {
                Paso1Card,
                Paso2Card,
                Paso3Card,
                Paso4Card,
                Paso5Card,
                Paso6Card,
                Paso7Card
            };


            TextBlock[] textos =
            {
                txtPaso1,
                txtPaso2,
                txtPaso3,
                txtPaso4,
                txtPaso5,
                txtPaso6,
                txtPaso7
            };


            for (int i = 0;
                 i < tarjetas.Length;
                 i++)
            {
                // =================================================
                // EN EDICIÓN EL PASO 1 YA ESTÁ COMPLETADO
                // =================================================

                if (esEdicion &&
                    i == 0)
                {
                    tarjetas[i].Background =
                        new SolidColorBrush(
                            (Color)
                            ColorConverter.ConvertFromString(
                                "#DCFCE7"));


                    textos[i].Foreground =
                        new SolidColorBrush(
                            (Color)
                            ColorConverter.ConvertFromString(
                                "#15803D"));


                    textos[i].Text =
                        "✔ 1";


                    continue;
                }


                // =================================================
                // PASOS COMPLETADOS
                // =================================================

                if (i + 1 < pasoActual)
                {
                    tarjetas[i].Background =
                        new SolidColorBrush(
                            (Color)
                            ColorConverter.ConvertFromString(
                                "#DCFCE7"));


                    textos[i].Foreground =
                        new SolidColorBrush(
                            (Color)
                            ColorConverter.ConvertFromString(
                                "#15803D"));


                    textos[i].Text =
                        $"✔ {i + 1}";
                }

                // =================================================
                // PASO ACTUAL
                // =================================================

                else if (
                    i + 1 == pasoActual)
                {
                    tarjetas[i].Background =
                        new SolidColorBrush(
                            (Color)
                            ColorConverter.ConvertFromString(
                                "#2563EB"));


                    textos[i].Foreground =
                        Brushes.White;
                }

                // =================================================
                // PASOS PENDIENTES
                // =================================================

                else
                {
                    tarjetas[i].Background =
                        Brushes.White;


                    textos[i].Foreground =
                        new SolidColorBrush(
                            (Color)
                            ColorConverter.ConvertFromString(
                                "#64748B"));
                }
            }
        }


        // =========================================================
        // MOSTRAR PASO
        // =========================================================

        private void MostrarPaso()
        {
            switch (pasoActual)
            {
                // =================================================
                // PASO 1
                // PACIENTE
                // =================================================

                case 1:
                    {
                        if (pasoPaciente == null)
                        {
                            pasoPaciente =
                                new PasoPaciente();


                            pasoPaciente
                                .PacienteSeleccionadoChanged +=
                                PasoPaciente_PacienteSeleccionadoChanged;
                        }


                        contenedorPaso.Content =
                            pasoPaciente;


                        btnSiguiente.Content =
                            "Siguiente ➜";


                        btnSiguiente.IsEnabled =
                            pacienteSeleccionado != null;


                        break;
                    }


                // =================================================
                // PASO 2
                // EVALUACIÓN
                // =================================================

                case 2:
                    {
                        if (pasoEvaluacion == null)
                        {
                            pasoEvaluacion =
                                new PasoEvaluacion();


                            if (pacienteSeleccionado != null)
                            {
                                pasoEvaluacion.CargarPaciente(
                                    pacienteSeleccionado);
                            }


                            // =========================================
                            // PRECARGAR EVALUACIÓN EXISTENTE
                            // =========================================

                            if (esEdicion &&
                                proceso.Evaluacion != null)
                            {
                                pasoEvaluacion.CargarEvaluacion(
                                    proceso.Evaluacion);
                            }
                        }


                        contenedorPaso.Content =
                            pasoEvaluacion;


                        btnSiguiente.Content =
                            "Siguiente ➜";


                        btnSiguiente.IsEnabled =
                            true;


                        break;
                    }


                // =================================================
                // PASO 3
                // INFORMACIÓN CLÍNICA
                // =================================================

                case 3:
                    {
                        if (pasoInformacion == null)
                        {
                            pasoInformacion =
                                new PasoInformacionClinica();


                            if (pacienteSeleccionado != null)
                            {
                                pasoInformacion.CargarPaciente(
                                    pacienteSeleccionado);
                            }


                            // =========================================
                            // PRECARGAR INFORMACIÓN EXISTENTE
                            // =========================================

                            if (esEdicion &&
                                proceso.InformacionClinica != null)
                            {
                                pasoInformacion.CargarInformacion(
                                    proceso.InformacionClinica);
                            }
                        }


                        contenedorPaso.Content =
                            pasoInformacion;


                        btnSiguiente.Content =
                            "Siguiente ➜";


                        btnSiguiente.IsEnabled =
                            true;


                        break;
                    }


                // =================================================
                // PASO 4
                // DIAGNÓSTICO
                // =================================================

                case 4:
                    {
                        if (pasoDiagnostico == null)
                        {
                            pasoDiagnostico =
                                new PasoDiagnostico();


                            // =========================================
                            // PRECARGAR DIAGNÓSTICOS EXISTENTES
                            // =========================================

                            if (esEdicion)
                            {
                                pasoDiagnostico.CargarDiagnosticos(
                                    proceso.Diagnostico,
                                    proceso.DiagnosticosSeleccionados,
                                    proceso.DiagnosticoPrincipalCIE10,
                                    proceso.DiagnosticosManuales);
                            }
                        }


                        contenedorPaso.Content =
                            pasoDiagnostico;


                        btnSiguiente.Content =
                            "Siguiente ➜";


                        btnSiguiente.IsEnabled =
                            true;


                        break;
                    }


                // =================================================
                // PASO 5
                // PROCEDIMIENTOS / TRATAMIENTO
                // =================================================

                case 5:
                    {
                        if (pasoProcedimientos == null)
                        {
                            pasoProcedimientos =
                                new PasoProcedimientos();


                            if (pacienteSeleccionado != null)
                            {
                                pasoProcedimientos.CargarPaciente(
                                    pacienteSeleccionado);
                            }


                            // =========================================
                            // PRECARGAR ITEMS EXISTENTES
                            // =========================================

                            if (esEdicion &&
                                proceso.ItemsClinicos != null)
                            {
                                pasoProcedimientos.CargarItems(
                                    proceso.ItemsClinicos);
                            }
                        }


                        contenedorPaso.Content =
                            pasoProcedimientos;


                        btnSiguiente.Content =
                            "Siguiente ➜";


                        btnSiguiente.IsEnabled =
                            true;


                        break;
                    }


                // =================================================
                // PASO 6
                // DESTINO
                // =================================================

                case 6:
                    {
                        if (pasoDestino == null)
                        {
                            pasoDestino =
                                new PasoDestino();


                            // =========================================
                            // PRECARGAR DESTINO EXISTENTE
                            // =========================================

                            if (esEdicion &&
                                proceso.Destino != null &&
                                !string.IsNullOrWhiteSpace(
                                    proceso.Destino.Destino))
                            {
                                pasoDestino.CargarDestino(
                                    proceso.Destino);
                            }
                        }


                        contenedorPaso.Content =
                            pasoDestino;


                        btnSiguiente.Content =
                            "Resumen ➜";


                        btnSiguiente.IsEnabled =
                            true;


                        break;
                    }


                // =================================================
                // PASO 7
                // RESUMEN
                // =================================================

                case 7:
                    {
                        pasoResumen =
                            new PasoResumen(
                                proceso);


                        contenedorPaso.Content =
                            pasoResumen;


                        btnSiguiente.Content =
                            esEdicion
                                ? "Guardar cambios"
                                : "Registrar Emergencia";


                        btnSiguiente.IsEnabled =
                            true;


                        break;
                    }
            }


            // =====================================================
            // BOTÓN ANTERIOR / CANCELAR
            // =====================================================

            if (!esEdicion &&
                pasoActual == 1)
            {
                btnCancelar.Content =
                    "Cancelar";
            }

            else if (
                esEdicion &&
                pasoActual == 2)
            {
                btnCancelar.Content =
                    "Cerrar";
            }

            else
            {
                btnCancelar.Content =
                    "← Anterior";
            }


            ActualizarSidebar();
        }


        // =========================================================
        // PACIENTE SELECCIONADO
        // =========================================================

        private void PasoPaciente_PacienteSeleccionadoChanged(
            Paciente paciente)
        {
            pacienteSeleccionado =
                paciente;


            proceso.Paciente =
                paciente;


            btnSiguiente.IsEnabled =
                true;
        }


        // =========================================================
        // BOTÓN SIGUIENTE
        // =========================================================

        private void btnSiguiente_Click(
            object sender,
            RoutedEventArgs e)
        {
            try
            {
                switch (pasoActual)
                {
                    // =============================================
                    // PASO 1
                    // =============================================

                    case 1:
                        {
                            if (pacienteSeleccionado == null)
                            {
                                MessageBox.Show(
                                    "Seleccione un paciente antes de continuar.",
                                    "MediSys",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);


                                return;
                            }


                            proceso.Paciente =
                                pacienteSeleccionado;


                            break;
                        }


                    // =============================================
                    // PASO 2
                    // =============================================

                    case 2:
                        {
                            if (pasoEvaluacion == null)
                            {
                                MessageBox.Show(
                                    "No fue posible cargar la evaluación inicial.",
                                    "MediSys",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);


                                return;
                            }


                            proceso.Evaluacion =
                                pasoEvaluacion
                                    .ObtenerEvaluacion();


                            break;
                        }


                    // =============================================
                    // PASO 3
                    // =============================================

                    case 3:
                        {
                            if (pasoInformacion == null)
                            {
                                MessageBox.Show(
                                    "No fue posible cargar la información clínica.",
                                    "MediSys",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);


                                return;
                            }


                            if (!pasoInformacion.Validar())
                            {
                                return;
                            }


                            proceso.InformacionClinica =
                                pasoInformacion
                                    .ObtenerInformacion();


                            break;
                        }


                    // =============================================
                    // PASO 4
                    // =============================================

                    case 4:
                        {
                            if (pasoDiagnostico == null)
                            {
                                MessageBox.Show(
                                    "No fue posible cargar el diagnóstico.",
                                    "MediSys",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);


                                return;
                            }


                            if (!pasoDiagnostico.Validar())
                            {
                                return;
                            }


                            // =========================================
                            // DIAGNÓSTICO TEXTUAL
                            // =========================================

                            proceso.Diagnostico =
                                pasoDiagnostico
                                    .ObtenerDiagnostico();


                            // =========================================
                            // DIAGNÓSTICOS CIE-10
                            // =========================================

                            proceso.DiagnosticosSeleccionados =
                                pasoDiagnostico
                                    .ObtenerDiagnosticosSeleccionados();


                            // =========================================
                            // PRINCIPAL
                            // =========================================

                            proceso.DiagnosticoPrincipalCIE10 =
                                pasoDiagnostico
                                    .ObtenerDiagnosticoPrincipalCIE10();


                            // =========================================
                            // MANUALES
                            // =========================================

                            proceso.DiagnosticosManuales =
                                pasoDiagnostico
                                    .ObtenerDiagnosticosManuales();


                            break;
                        }


                    // =============================================
                    // PASO 5
                    // =============================================

                    case 5:
                        {
                            if (pasoProcedimientos == null)
                            {
                                MessageBox.Show(
                                    "No fue posible cargar el paso de tratamiento.",
                                    "MediSys",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);


                                return;
                            }


                            if (!pasoProcedimientos.Validar())
                            {
                                return;
                            }


                            proceso.Procedimientos =
                                pasoProcedimientos
                                    .ObtenerProcedimientos();


                            proceso.ItemsClinicos =
                                pasoProcedimientos
                                    .ObtenerItems();


                            break;
                        }


                    // =============================================
                    // PASO 6
                    // =============================================

                    case 6:
                        {
                            if (pasoDestino == null)
                            {
                                MessageBox.Show(
                                    "No fue posible cargar el destino del paciente.",
                                    "MediSys",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);


                                return;
                            }


                            if (!pasoDestino.Validar())
                            {
                                return;
                            }


                            proceso.Destino =
                                pasoDestino
                                    .ObtenerDestino();


                            break;
                        }


                    // =============================================
                    // PASO 7
                    // =============================================

                    case 7:
                        {
                            GuardarProceso();

                            return;
                        }
                }


                // =================================================
                // AVANZAR
                // =================================================

                if (pasoActual < 7)
                {
                    pasoActual++;


                    MostrarPaso();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible continuar al siguiente paso.\n\n" +
                    $"{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // GUARDAR PROCESO
        // =========================================================

        // =========================================================
        // GUARDAR PROCESO
        // =========================================================

        private void GuardarProceso()
        {
            try
            {
                EmergenciaDAO dao =
                    new EmergenciaDAO();


                // =====================================================
                // NUEVA EMERGENCIA
                // =====================================================

                if (!esEdicion)
                {
                    bool guardado =
                        dao.GuardarEmergenciaCompleta(
                            proceso);


                    if (!guardado)
                    {
                        MessageBox.Show(
                            "No fue posible registrar la emergencia.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }

                    IdEmergenciaProcesada =
                            dao.UltimoIdEmergenciaGuardada;

                    MessageBox.Show(
                        "La emergencia fue registrada correctamente.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);


                    DialogResult =
                        true;


                    Close();


                    return;
                }


                // =====================================================
                // ACTUALIZAR EMERGENCIA EXISTENTE
                // =====================================================

                bool actualizado =
                    dao.ActualizarEmergenciaCompleta(
                        idEmergenciaActual,
                        proceso);


                if (!actualizado)
                {
                    MessageBox.Show(
                        "No fue posible guardar los cambios de la atención.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                MessageBox.Show(
                    "La atención de emergencia fue actualizada correctamente.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                DialogResult =
                    true;


                Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible guardar la atención.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // BOTÓN ANTERIOR / CERRAR
        // =========================================================

        private void btnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            // =====================================================
            // NUEVA EMERGENCIA
            // =====================================================

            if (!esEdicion &&
                pasoActual == 1)
            {
                Close();

                return;
            }


            // =====================================================
            // EDITANDO:
            // NO VOLVER A SELECCIONAR PACIENTE
            // =====================================================

            if (esEdicion &&
                pasoActual == 2)
            {
                Close();

                return;
            }


            if (pasoActual > 1)
            {
                pasoActual--;


                MostrarPaso();
            }
        }
    }
}