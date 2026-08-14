using Proyecto_MediSys.Models;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media;
using Proyecto_MediSys.Data;

namespace Proyecto_MediSys.Controls
{

    public partial class EmergenciaDialog : Window
    {
        private PasoPaciente pasoPaciente = null!;
        private PasoEvaluacion? pasoEvaluacion;
        private PasoInformacionClinica? pasoInformacion;
        private PasoDiagnostico? pasoDiagnostico;
        private PasoProcedimientos? pasoProcedimientos;
        private PasoDestino? pasoDestino;
        private PasoResumen pasoResumen = null!;
       
        

        private Paciente? pacienteSeleccionado;

        private ProcesoEmergencia proceso = new();
        
        private int pasoActual = 1;

        // =============================================
        // MODO DEL FORMULARIO
        // =============================================

        private bool esEdicion = false;
        private long idEmergenciaActual = 0;

        private Emergencia? emergenciaActual;

        public EmergenciaDialog()
        {
            InitializeComponent();

            MostrarPaso();
        }

        public EmergenciaDialog(long idEmergencia)
        {
            InitializeComponent();

            esEdicion = true;
            idEmergenciaActual = idEmergencia;

            CargarEmergenciaExistente();

            MostrarPaso();
        }

        private void CargarEmergenciaExistente()
        {
            try
            {
                EmergenciaDAO dao = new EmergenciaDAO();

                var resultado = dao.ObtenerPorId(idEmergenciaActual);

                if (resultado.Emergencia == null)
                {
                    MessageBox.Show(
                        "No fue posible encontrar la emergencia seleccionada.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                emergenciaActual = resultado.Emergencia;

                if (resultado.Proceso != null)
                {
                    proceso = resultado.Proceso;

                    pacienteSeleccionado = proceso.Paciente;
                }

                if (pacienteSeleccionado == null)
                {
                    MessageBox.Show(
                        "No fue posible cargar el paciente de esta emergencia.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar la emergencia.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

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

            for (int i = 0; i < tarjetas.Length; i++)
            {
                if (i + 1 < pasoActual)
                {
                    tarjetas[i].Background = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#DCFCE7"));

                    textos[i].Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#15803D"));

                    textos[i].Text = $"✔ {i + 1}";
                }
                else if (i + 1 == pasoActual)
                {
                    tarjetas[i].Background = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#2563EB"));

                    textos[i].Foreground = Brushes.White;
                }
                else
                {
                    tarjetas[i].Background = Brushes.White;

                    textos[i].Foreground = new SolidColorBrush(
                        (Color)ColorConverter.ConvertFromString("#64748B"));
                }
            }
        }

        private void MostrarPaso()
        {
            switch (pasoActual)
            {
                case 1:

                    pasoPaciente = new PasoPaciente();

                    pasoPaciente.PacienteSeleccionadoChanged += PasoPaciente_PacienteSeleccionadoChanged;

                    contenedorPaso.Content = pasoPaciente;

                    btnSiguiente.Content = "Siguiente ➜";

                    btnSiguiente.IsEnabled = pacienteSeleccionado != null;

                break;

                case 2:

                    if (pasoEvaluacion == null)
                    {
                        pasoEvaluacion = new PasoEvaluacion();

                        pasoEvaluacion.CargarPaciente(pacienteSeleccionado!);

                        if (esEdicion && proceso.Evaluacion != null)
                        {
                            pasoEvaluacion.CargarEvaluacion(
                                proceso.Evaluacion);
                        }
                    }

                    contenedorPaso.Content = pasoEvaluacion;

                    btnSiguiente.Content = "Siguiente ➜";
                    btnSiguiente.IsEnabled = true;

                    break;

                case 3:

                    if (pasoInformacion == null)
                    {
                        pasoInformacion =
                            new PasoInformacionClinica();

                        pasoInformacion.CargarPaciente(
                            pacienteSeleccionado!);
                    }

                    contenedorPaso.Content =
                        pasoInformacion;

                    btnSiguiente.Content =
                        "Siguiente ➜";

                    btnSiguiente.IsEnabled = true;

                    break;

                case 4:

                    if (pasoDiagnostico == null)
                    {
                        pasoDiagnostico = new PasoDiagnostico();
                    }

                    contenedorPaso.Content = pasoDiagnostico;

                    btnSiguiente.Content = "Siguiente ➜";

                    btnSiguiente.IsEnabled = true;

                    break;

                case 5:

                    if (pasoProcedimientos == null)
                    {
                        pasoProcedimientos =
                            new PasoProcedimientos();

                        pasoProcedimientos.CargarPaciente(
                            pacienteSeleccionado!);
                    }

                    contenedorPaso.Content =
                        pasoProcedimientos;

                    btnSiguiente.Content =
                        "Siguiente ➜";

                    btnSiguiente.IsEnabled =
                        true;

                    break;

                case 6:

                    if (pasoDestino == null)
                    {
                        pasoDestino = new PasoDestino();
                    }

                    contenedorPaso.Content = pasoDestino;

                    btnSiguiente.Content = "Resumen ➜";

                    btnSiguiente.IsEnabled = true;

                    break;
                case 7:

                    pasoResumen = new PasoResumen(proceso);

                    contenedorPaso.Content = pasoResumen;

                    contenedorPaso.Content = pasoResumen;

                    btnSiguiente.Content = "Registrar Emergencia";

                    btnSiguiente.IsEnabled = true;

                    break;
            }

            if (pasoActual == 1)
            {
                btnCancelar.Content = "Cancelar";
            }
            else
            {
                btnCancelar.Content = "← Anterior";
            }

            ActualizarSidebar();
        }

        private void PasoPaciente_PacienteSeleccionadoChanged(Paciente paciente)
        {
            pacienteSeleccionado = paciente;

            proceso.Paciente = paciente;

            btnSiguiente.IsEnabled = true;
        }

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            switch (pasoActual)
            {
                case 1:

                    if (pacienteSeleccionado == null)
                    {
                        MessageBox.Show(
                            "Seleccione un paciente antes de continuar.",
                            "MediSys",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }

                    break;

                case 2:

                    proceso.Evaluacion = pasoEvaluacion.ObtenerEvaluacion();
                                  
                    break;

                case 3:

                    if (!pasoInformacion!.Validar())
                        return;

                    proceso.InformacionClinica =
                        pasoInformacion.ObtenerInformacion();

                    break;
                case 4:

                    if (!pasoDiagnostico!.Validar())
                        return;

                    proceso.Diagnostico =
                        pasoDiagnostico.ObtenerDiagnostico();

                    break;

                case 5:

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
                        return;


                    proceso.Procedimientos =
                        pasoProcedimientos.ObtenerProcedimientos();


                    proceso.ItemsClinicos =
                        pasoProcedimientos.ObtenerItems();

                    break;

                case 6:

                    if (!pasoDestino!.Validar())
                        return;

                    proceso.Destino =
                        pasoDestino.ObtenerDestino();

                    break;
            }

            if (pasoActual == 7)
            {
                EmergenciaDAO dao = new EmergenciaDAO();

                bool guardado = dao.GuardarEmergenciaCompleta(proceso);

                if (guardado)
                {
                    MessageBox.Show(
                        "La emergencia fue registrada correctamente.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    DialogResult = true;
                    Close();
                }

                return;
            }

            pasoActual++;

            MostrarPaso();
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            if (pasoActual == 1)
            {
                Close();
                return;
            }

            pasoActual--;

            MostrarPaso();
        }
    }
}