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
    public partial class CitaDialog : Window
    {
        // =========================================================
        // DAO
        // =========================================================

        private readonly CitaDAO citaDAO =
            new CitaDAO();

        private readonly PacienteDAO pacienteDAO =
            new PacienteDAO();


        // =========================================================
        // DATOS
        // =========================================================

        private List<Paciente> pacientes =
            new List<Paciente>();

        private List<EspecialidadCitaOpcion> especialidades =
            new List<EspecialidadCitaOpcion>();

        private List<MedicoCitaOpcion> medicos =
            new List<MedicoCitaOpcion>();


        private Cita? citaActual;

        private readonly bool esEdicion;

        private readonly bool soloLectura;

        private bool cargandoDatos;


        // =========================================================
        // NUEVA CITA
        // =========================================================

        public CitaDialog()
        {
            InitializeComponent();


            esEdicion =
                false;

            soloLectura =
                false;


            Inicializar();
        }


        // =========================================================
        // EDITAR / VER
        // =========================================================

        public CitaDialog(
            Cita cita,
            bool soloLectura = false)
        {
            InitializeComponent();


            citaActual =
                cita;

            esEdicion =
                true;

            this.soloLectura =
                soloLectura;


            Inicializar();


            CargarCitaExistente();


            ConfigurarModo();
        }


        // =========================================================
        // INICIALIZAR
        // =========================================================

        private void Inicializar()
        {
            try
            {
                cargandoDatos =
                    true;


                CargarPacientes();

                CargarEspecialidades();

                CargarMedicos();

                CargarHoras();


                dpFecha.SelectedDate =
                    DateTime.Today;


                txtTitulo.Text =
                    "NUEVA CITA";

                txtSubtitulo.Text =
                    "Programe una atención médica para el paciente.";

                txtCodigo.Text =
                    "Nueva";


                cargandoDatos =
                    false;
            }
            catch (Exception ex)
            {
                cargandoDatos =
                    false;


                MessageBox.Show(
                    $"No fue posible cargar el formulario de citas.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);


                Close();
            }
        }


        // =========================================================
        // PACIENTES
        // =========================================================

        private void CargarPacientes()
        {
            pacientes =
                pacienteDAO
                    .ObtenerTodos();


            cmbPaciente.ItemsSource =
                pacientes;
        }


        // =========================================================
        // ESPECIALIDADES
        // =========================================================

        private void CargarEspecialidades()
        {
            especialidades =
                citaDAO
                    .ObtenerEspecialidades();


            cmbEspecialidad.ItemsSource =
                especialidades;
        }


        // =========================================================
        // MÉDICOS
        // =========================================================

        private void CargarMedicos()
        {
            medicos =
                citaDAO
                    .ObtenerMedicos();


            cmbMedico.ItemsSource =
                new List<MedicoCitaOpcion>();
        }


        // =========================================================
        // HORARIOS
        // =========================================================

        private void CargarHoras()
        {
            List<HoraOpcion> horas =
                new List<HoraOpcion>();


            // 8:00 AM hasta 5:00 PM
            // intervalos de 30 minutos.

            TimeSpan inicio =
                new TimeSpan(
                    8,
                    0,
                    0);


            TimeSpan fin =
                new TimeSpan(
                    17,
                    0,
                    0);


            TimeSpan actual =
                inicio;


            while (actual <= fin)
            {
                horas.Add(
                    new HoraOpcion
                    {
                        Valor =
                            actual,

                        Texto =
                            DateTime.Today
                                .Add(actual)
                                .ToString(
                                    "hh:mm tt")
                    });


                actual =
                    actual.Add(
                        TimeSpan.FromMinutes(
                            30));
            }


            cmbHora.ItemsSource =
                horas;
        }


        // =========================================================
        // BUSCAR PACIENTE DESDE EL MISMO COMBOBOX
        // =========================================================

        private void cmbPaciente_KeyUp(
            object sender,
            System.Windows.Input.KeyEventArgs e)
        {
            if (cargandoDatos)
                return;


            // No filtrar con teclas de navegación
            if (e.Key == System.Windows.Input.Key.Up ||
                e.Key == System.Windows.Input.Key.Down ||
                e.Key == System.Windows.Input.Key.Enter ||
                e.Key == System.Windows.Input.Key.Tab ||
                e.Key == System.Windows.Input.Key.Escape)
            {
                return;
            }


            try
            {
                string buscar =
                    cmbPaciente.Text
                        ?.Trim()
                        .ToLower()
                    ?? "";


                // =====================================================
                // TEXTO VACÍO
                // =====================================================

                if (string.IsNullOrWhiteSpace(
                    buscar))
                {
                    cmbPaciente.ItemsSource =
                        pacientes;


                    cmbPaciente.IsDropDownOpen =
                        true;


                    return;
                }


                // =====================================================
                // FILTRAR
                // =====================================================

                List<Paciente> resultado =
                    pacientes
                        .Where(
                            p =>
                                (p.NombreCompleto ?? "")
                                    .ToLower()
                                    .Contains(buscar)

                                ||

                                (p.DocumentoMostrar ?? "")
                                    .ToLower()
                                    .Contains(buscar)

                                ||

                                (p.CodigoPaciente ?? "")
                                    .ToLower()
                                    .Contains(buscar))
                        .ToList();


                cmbPaciente.ItemsSource =
                    resultado;


                cmbPaciente.IsDropDownOpen =
                    resultado.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible buscar el paciente.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // =========================================================
        // PACIENTE SELECCIONADO
        // =========================================================

        private void cmbPaciente_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cmbPaciente.SelectedItem
                is not Paciente paciente)
            {
                txtDocumento.Text =
                    "—";

                txtTelefono.Text =
                    "—";

                txtSeguro.Text =
                    "—";

                return;
            }


            txtDocumento.Text =
                string.IsNullOrWhiteSpace(
                    paciente.DocumentoMostrar)

                    ? "—"
                    : paciente.DocumentoMostrar;


            txtTelefono.Text =
                string.IsNullOrWhiteSpace(
                    paciente.Telefono)

                    ? "—"
                    : paciente.Telefono;


            txtSeguro.Text =
                string.IsNullOrWhiteSpace(
                    paciente.NombreSeguro)

                    ? "Sin seguro"
                    : paciente.NombreSeguro;
        }

        // =========================================================
        // ESPECIALIDAD SELECCIONADA
        // =========================================================

        private void cmbEspecialidad_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoDatos)
                return;


            if (cmbEspecialidad.SelectedItem
                is not EspecialidadCitaOpcion especialidad)
            {
                cmbMedico.ItemsSource =
                    new List<MedicoCitaOpcion>();

                return;
            }


            CargarMedicosPorEspecialidad(
                especialidad.IdEspecialidad);


            VerificarDisponibilidad();
        }


        private void CargarMedicosPorEspecialidad(
            long idEspecialidad)
        {
            List<MedicoCitaOpcion> resultado =
                medicos
                    .Where(
                        x =>
                            x.IdEspecialidad ==
                            idEspecialidad)
                    .OrderBy(
                        x =>
                            x.NombreCompleto)
                    .ToList();


            cmbMedico.ItemsSource =
                resultado;
        }


        // =========================================================
        // CAMBIO DE FECHA / HORA / MÉDICO
        // =========================================================

        private void DatosHorario_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoDatos)
                return;


            VerificarDisponibilidad();
        }


        // =========================================================
        // DISPONIBILIDAD
        // =========================================================

        private void VerificarDisponibilidad()
        {
            try
            {
                if (cmbMedico.SelectedItem
                        is not MedicoCitaOpcion medico
                    ||
                    dpFecha.SelectedDate
                        == null
                    ||
                    cmbHora.SelectedItem
                        is not HoraOpcion hora)
                {
                    MostrarDisponibilidadNeutral();

                    return;
                }


                bool ocupado =
                    citaDAO.ExisteHorario(
                        medico.IdMedico,
                        dpFecha.SelectedDate.Value,
                        hora.Valor,
                        esEdicion
                            ? citaActual?.IdCita
                            : null);


                if (ocupado)
                {
                    panelDisponibilidad.Background =
                        new SolidColorBrush(
                            (Color)
                            ColorConverter.ConvertFromString(
                                "#FEECEC"));


                    panelDisponibilidad.BorderBrush =
                        new SolidColorBrush(
                            (Color)
                            ColorConverter.ConvertFromString(
                                "#F5CCCC"));


                    txtDisponibilidad.Foreground =
                        new SolidColorBrush(
                            (Color)
                            ColorConverter.ConvertFromString(
                                "#B42318"));


                    txtDisponibilidad.Text =
                        "Horario no disponible. El médico ya tiene una cita programada.";
                }
                else
                {
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
                        "Horario disponible.";
                }
            }
            catch
            {
                MostrarDisponibilidadNeutral();
            }
        }


        private void MostrarDisponibilidadNeutral()
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
                "Seleccione médico, fecha y hora para verificar disponibilidad.";
        }


        // =========================================================
        // CARGAR CITA EXISTENTE
        // =========================================================

        private void CargarCitaExistente()
        {
            if (citaActual == null)
                return;


            cargandoDatos =
                true;


            txtCodigo.Text =
                citaActual.CodigoCita;


            txtTitulo.Text =
                soloLectura
                    ? "DETALLE DE CITA"
                    : "EDITAR CITA";


            txtSubtitulo.Text =
                soloLectura
                    ? "Información de la cita médica registrada."
                    : "Modifique los datos de la cita seleccionada.";


            // =====================================================
            // PACIENTE
            // =====================================================

            cmbPaciente.SelectedItem =
                pacientes.FirstOrDefault(
                    x =>
                        x.IdPaciente ==
                        citaActual.IdPaciente);


            // =====================================================
            // ESPECIALIDAD
            // =====================================================

            EspecialidadCitaOpcion? especialidad =
                especialidades
                    .FirstOrDefault(
                        x =>
                            x.IdEspecialidad ==
                            citaActual.IdEspecialidad);


            cmbEspecialidad.SelectedItem =
                especialidad;


            if (especialidad != null)
            {
                CargarMedicosPorEspecialidad(
                    especialidad.IdEspecialidad);
            }


            // =====================================================
            // MÉDICO
            // =====================================================

            cmbMedico.SelectedItem =
                medicos
                    .FirstOrDefault(
                        x =>
                            x.IdMedico ==
                            citaActual.IdMedico);


            // =====================================================
            // FECHA
            // =====================================================

            dpFecha.SelectedDate =
                citaActual.FechaCita;


            // =====================================================
            // HORA
            // =====================================================

            if (cmbHora.ItemsSource
                is IEnumerable<HoraOpcion> horas)
            {
                cmbHora.SelectedItem =
                    horas.FirstOrDefault(
                        x =>
                            x.Valor ==
                            citaActual.HoraCita);
            }


            txtMotivo.Text =
                citaActual.Motivo;


            txtObservaciones.Text =
                citaActual.Observaciones;


            cargandoDatos =
                false;


            VerificarDisponibilidad();
        }


        // =========================================================
        // CONFIGURAR MODO
        // =========================================================

        private void ConfigurarModo()
        {
            if (!soloLectura)
            {
                btnGuardar.Content =
                    esEdicion
                        ? "Guardar cambios"
                        : "Guardar cita";

                btnCancelar.Content =
                    "Cancelar";

                return;
            }


            // =====================================================
            // SOLO LECTURA
            // =====================================================

            cmbPaciente.IsEnabled =
                false;

            cmbEspecialidad.IsEnabled =
                false;

            cmbMedico.IsEnabled =
                false;

            dpFecha.IsEnabled =
                false;

            cmbHora.IsEnabled =
                false;

            txtMotivo.IsReadOnly =
                true;

            txtObservaciones.IsReadOnly =
                true;


            btnGuardar.Visibility =
                Visibility.Collapsed;


            btnCancelar.Content =
                "Cerrar";


            btnCancelar.Width =
                120;
        }


        // =========================================================
        // VALIDAR
        // =========================================================

        private bool Validar()
        {
            if (cmbPaciente.SelectedItem
                is not Paciente)
            {
                MessageBox.Show(
                    "Debe seleccionar un paciente.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (cmbEspecialidad.SelectedItem
                is not EspecialidadCitaOpcion)
            {
                MessageBox.Show(
                    "Debe seleccionar una especialidad.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (cmbMedico.SelectedItem
                is not MedicoCitaOpcion)
            {
                MessageBox.Show(
                    "Debe seleccionar un médico.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (!dpFecha.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Debe seleccionar la fecha de la cita.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (dpFecha.SelectedDate.Value.Date <
                DateTime.Today)
            {
                MessageBox.Show(
                    "No puede programar una cita en una fecha pasada.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (cmbHora.SelectedItem
                is not HoraOpcion hora)
            {
                MessageBox.Show(
                    "Debe seleccionar la hora de la cita.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            DateTime fechaHora =
                dpFecha.SelectedDate.Value.Date
                    .Add(hora.Valor);


            if (fechaHora <= DateTime.Now)
            {
                MessageBox.Show(
                    "La fecha y hora de la cita deben ser posteriores a la hora actual.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (string.IsNullOrWhiteSpace(
                txtMotivo.Text))
            {
                MessageBox.Show(
                    "Debe indicar el motivo de la cita.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtMotivo.Focus();

                return false;
            }


            return true;
        }


        // =========================================================
        // GUARDAR
        // =========================================================

        private void btnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (soloLectura)
                return;


            if (!Validar())
                return;


            try
            {
                Paciente paciente =
                    (Paciente)
                    cmbPaciente.SelectedItem;


                EspecialidadCitaOpcion especialidad =
                    (EspecialidadCitaOpcion)
                    cmbEspecialidad.SelectedItem;


                MedicoCitaOpcion medico =
                    (MedicoCitaOpcion)
                    cmbMedico.SelectedItem;


                HoraOpcion hora =
                    (HoraOpcion)
                    cmbHora.SelectedItem;


                // =================================================
                // COMPROBAR DISPONIBILIDAD NUEVAMENTE
                // =================================================

                bool ocupado =
                    citaDAO.ExisteHorario(
                        medico.IdMedico,
                        dpFecha.SelectedDate!.Value,
                        hora.Valor,
                        esEdicion
                            ? citaActual?.IdCita
                            : null);


                if (ocupado)
                {
                    MessageBox.Show(
                        "El médico ya tiene una cita para esa fecha y hora.\n\nSeleccione otro horario.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                // =================================================
                // CREAR OBJETO
                // =================================================

                Cita cita =
                    citaActual
                    ?? new Cita();


                cita.IdPaciente =
                    paciente.IdPaciente;


                cita.IdEspecialidad =
                    especialidad.IdEspecialidad;


                cita.IdMedico =
                    medico.IdMedico;


                cita.FechaCita =
                    dpFecha.SelectedDate.Value.Date;


                cita.HoraCita =
                    hora.Valor;


                cita.Motivo =
                    txtMotivo.Text.Trim();


                cita.Observaciones =
                    txtObservaciones.Text.Trim();


                // =================================================
                // INSERTAR / ACTUALIZAR
                // =================================================

                bool resultado;


                if (!esEdicion)
                {
                    cita.IdEstadoCita =
                        1;


                    resultado =
                        citaDAO.Insertar(
                            cita);
                }
                else
                {
                    resultado =
                        citaDAO.Actualizar(
                            cita);
                }


                if (!resultado)
                {
                    MessageBox.Show(
                        "No fue posible guardar la cita.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                MessageBox.Show(
                    esEdicion
                        ? "La cita fue actualizada correctamente."
                        : "La cita fue registrada correctamente.",
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
                    $"No fue posible guardar la cita.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // CANCELAR / CERRAR
        // =========================================================

        private void btnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }


        // =========================================================
        // CLASE PARA HORAS
        // =========================================================

        private class HoraOpcion
        {
            public TimeSpan Valor { get; set; }

            public string Texto { get; set; } = "";
        }
    }
}