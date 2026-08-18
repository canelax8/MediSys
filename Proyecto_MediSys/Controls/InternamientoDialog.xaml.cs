using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static Proyecto_MediSys.Models.Cama;
using Proyecto_MediSys.Controls;

namespace Proyecto_MediSys.Controls
{


    public partial class InternamientoDialog : Window
    {
        // =========================================================
        // DAO
        // =========================================================

        private readonly InternamientoDAO internamientoDAO =
            new InternamientoDAO();

        private readonly PacienteDAO pacienteDAO =
            new PacienteDAO();

        private readonly CitaDAO citaDAO =
            new CitaDAO();

        // =========================================================
        // NUEVO INTERNAMIENTO DESDE EMERGENCIA
        // =========================================================

        public InternamientoDialog(
            long idEmergenciaOrigen,
            string tipoSugerido)
        {
            InitializeComponent();


            esEdicion =
                false;

            soloLectura =
                false;


            Inicializar();


            CargarDesdeEmergencia(
                idEmergenciaOrigen,
                tipoSugerido);
        }

        // =========================================================
        // LISTAS
        // =========================================================

        private List<Paciente> pacientes =
            new List<Paciente>();

        private List<TipoInternamiento> tipos =
            new List<TipoInternamiento>();

        private List<AreaHospitalaria> areas =
            new List<AreaHospitalaria>();

        private List<EspecialidadCitaOpcion> especialidades =
            new List<EspecialidadCitaOpcion>();

        private List<MedicoCitaOpcion> medicos =
            new List<MedicoCitaOpcion>();

        private List<EmergenciaInternamientoOpcion> emergencias =
            new List<EmergenciaInternamientoOpcion>();


        // =========================================================
        // MODO
        // =========================================================

        private Internamiento? internamientoActual;

        private readonly bool esEdicion;

        private readonly bool soloLectura;

        private bool cargandoDatos =
            true;


        // =========================================================
        // NUEVO
        // =========================================================

        public InternamientoDialog()
        {
            InitializeComponent();


            esEdicion =
                false;

            soloLectura =
                false;


            Inicializar();
        }

        // =========================================================
        // CARGAR DATOS DESDE EMERGENCIA
        // =========================================================

        private void CargarDesdeEmergencia(
            long idEmergenciaOrigen,
            string tipoSugerido)
        {
            try
            {
                cargandoDatos =
                    false;


                // =====================================================
                // LOCALIZAR EMERGENCIA
                // =====================================================

                EmergenciaInternamientoOpcion? emergenciaOpcion =
                    emergencias
                        .FirstOrDefault(
                            x =>
                                x.IdEmergencia ==
                                idEmergenciaOrigen);


                if (emergenciaOpcion == null)
                {
                    MessageBox.Show(
                        "No fue posible localizar la emergencia para crear el internamiento.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                // =====================================================
                // MARCAR ORIGEN = EMERGENCIA
                // =====================================================

                rbIngresoDirecto.IsChecked =
                    false;


                rbEmergencia.IsChecked =
                    true;


                ActualizarOrigen();


                // =====================================================
                // SELECCIONAR EMERGENCIA
                //
                // Esto ya dispara el código que carga:
                // paciente
                // especialidad
                // médico
                // motivo
                // =====================================================

                cmbEmergencia.SelectedItem =
                    emergenciaOpcion;


                // =====================================================
                // TIPO DE INTERNAMIENTO
                // =====================================================

                TipoInternamiento? tipo =
                    tipos
                        .FirstOrDefault(
                            x =>
                                x.Nombre.Equals(
                                    tipoSugerido,
                                    StringComparison.OrdinalIgnoreCase));


                if (tipo != null)
                {
                    cmbTipo.SelectedItem =
                        tipo;
                }


                // =====================================================
                // TRAER INFORMACIÓN CLÍNICA COMPLETA
                // =====================================================

                EmergenciaDAO emergenciaDAO =
                    new EmergenciaDAO();


                var resultado =
                    emergenciaDAO.ObtenerPorId(
                        idEmergenciaOrigen);


                if (resultado.Proceso != null)
                {
                    // Motivo

                    if (resultado.Proceso.InformacionClinica != null)
                    {
                        txtMotivo.Text =
                            resultado.Proceso
                                .InformacionClinica
                                .MotivoConsulta ?? "";
                    }


                    // Diagnóstico

                    if (resultado.Proceso.Diagnostico != null)
                    {
                        txtDiagnostico.Text =
                            resultado.Proceso
                                .Diagnostico
                                .DiagnosticoPrincipal ?? "";
                    }


                    // Observaciones finales del destino

                    if (resultado.Proceso.Destino != null)
                    {
                        txtObservaciones.Text =
                            resultado.Proceso
                                .Destino
                                .ObservacionesFinales ?? "";
                    }
                }


                // =====================================================
                // FECHA DE INTERNAMIENTO = AHORA
                // =====================================================

                dpFechaIngreso.SelectedDate =
                    DateTime.Today;


                txtHoraIngreso.Text =
                    DateTime.Now.ToString(
                        "HH:mm");


                // =====================================================
                // MENSAJE VISUAL
                // =====================================================

                txtSubtitulo.Text =
                    $"Internamiento generado desde la emergencia {emergenciaOpcion.CodigoEmergencia}.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar los datos de la emergencia.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // =========================================================
        // EDITAR / VER
        // =========================================================

        public InternamientoDialog(
            Internamiento internamiento,
            bool soloLectura = false)
        {
            InitializeComponent();


            internamientoActual =
                internamiento;

            esEdicion =
                true;

            this.soloLectura =
                soloLectura;


            Inicializar();

            CargarInternamientoExistente();

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


                pacientes =
                    pacienteDAO.ObtenerTodos();


                tipos =
                    internamientoDAO
                        .ObtenerTiposInternamiento();


                areas =
                    internamientoDAO
                        .ObtenerAreas();


                especialidades =
                    citaDAO
                        .ObtenerEspecialidades();


                medicos =
                    citaDAO
                        .ObtenerMedicos();


                emergencias =
                    internamientoDAO
                        .ObtenerEmergenciasDisponibles(
                            internamientoActual
                                ?.IdInternamiento);


                cmbPaciente.ItemsSource =
                    pacientes;


                cmbTipo.ItemsSource =
                    tipos;


                cmbArea.ItemsSource =
                    areas;


                cmbEspecialidad.ItemsSource =
                    especialidades;


                cmbEmergencia.ItemsSource =
                    emergencias;


                cmbMedico.ItemsSource =
                    new List<MedicoCitaOpcion>();


                cmbHabitacion.ItemsSource =
                    new List<Habitacion>();


                cmbCama.ItemsSource =
                    new List<Cama>();


                if (!esEdicion)
                {
                    dpFechaIngreso.SelectedDate =
                        DateTime.Today;


                    txtHoraIngreso.Text =
                        DateTime.Now.ToString(
                            "HH:mm");


                    if (tipos.Count > 0)
                    {
                        cmbTipo.SelectedIndex =
                            0;
                    }
                }


                cargandoDatos =
                    false;


                ActualizarOrigen();
            }
            catch (Exception ex)
            {
                cargandoDatos =
                    false;


                MessageBox.Show(
                    $"No fue posible cargar el formulario de internamiento.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);


                Close();
            }
        }


        // =========================================================
        // BUSCAR PACIENTE
        // =========================================================

        private void cmbPaciente_KeyUp(
            object sender,
            KeyEventArgs e)
        {
            if (cargandoDatos)
                return;


            if (e.Key == Key.Up ||
                e.Key == Key.Down ||
                e.Key == Key.Enter ||
                e.Key == Key.Tab ||
                e.Key == Key.Escape)
            {
                return;
            }


            string buscar =
                cmbPaciente.Text
                    ?.Trim()
                    .ToLower()
                ?? "";


            if (string.IsNullOrWhiteSpace(
                buscar))
            {
                cmbPaciente.ItemsSource =
                    pacientes;

                cmbPaciente.IsDropDownOpen =
                    true;

                return;
            }


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


        // =========================================================
        // PACIENTE
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


            if (!cargandoDatos)
            {
                cmbPaciente.IsDropDownOpen =
                    false;
            }
        }


        // =========================================================
        // ORIGEN
        // =========================================================

        private void Origen_Checked(
            object sender,
            RoutedEventArgs e)
        {
            if (cargandoDatos)
                return;


            ActualizarOrigen();
        }


        private void ActualizarOrigen()
        {
            bool desdeEmergencia =
                rbEmergencia.IsChecked ==
                true;


            panelEmergencia.Visibility =
                desdeEmergencia

                    ? Visibility.Visible
                    : Visibility.Collapsed;


            cmbEmergencia.IsEnabled =
                desdeEmergencia;


            cmbPaciente.IsEnabled =
                !desdeEmergencia
                &&
                !soloLectura;


            if (!desdeEmergencia &&
                !cargandoDatos)
            {
                cmbEmergencia.SelectedItem =
                    null;
            }
        }


        // =========================================================
        // EMERGENCIA
        // =========================================================

        private void cmbEmergencia_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoDatos)
                return;


            if (cmbEmergencia.SelectedItem
                is not EmergenciaInternamientoOpcion emergencia)
            {
                return;
            }


            // =====================================================
            // PACIENTE
            // =====================================================

            Paciente? paciente =
                pacientes
                    .FirstOrDefault(
                        x =>
                            x.IdPaciente ==
                            emergencia.IdPaciente);


            cmbPaciente.ItemsSource =
                pacientes;


            cmbPaciente.SelectedItem =
                paciente;


            // =====================================================
            // ESPECIALIDAD
            // =====================================================

            EspecialidadCitaOpcion? especialidad =
                especialidades
                    .FirstOrDefault(
                        x =>
                            x.IdEspecialidad ==
                            emergencia.IdEspecialidad);


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
                            emergencia.IdMedico);


            // =====================================================
            // MOTIVO
            // =====================================================

            if (string.IsNullOrWhiteSpace(
                txtMotivo.Text))
            {
                txtMotivo.Text =
                    emergencia.MotivoConsulta;
            }
        }


        // =========================================================
        // ESPECIALIDAD
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
        }


        private void CargarMedicosPorEspecialidad(
            long idEspecialidad)
        {
            cmbMedico.ItemsSource =
                medicos
                    .Where(
                        x =>
                            x.IdEspecialidad ==
                            idEspecialidad)
                    .OrderBy(
                        x =>
                            x.NombreCompleto)
                    .ToList();
        }


        // =========================================================
        // TIPO
        // =========================================================

        private void cmbTipo_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoDatos)
                return;


            if (cmbTipo.SelectedItem
                is not TipoInternamiento tipo)
            {
                return;
            }


            // Si el tipo seleccionado es UCI,
            // seleccionar automáticamente el área UCI.

            if (tipo.Nombre.Equals(
                "UCI",
                StringComparison.OrdinalIgnoreCase))
            {
                AreaHospitalaria? areaUci =
                    areas
                        .FirstOrDefault(
                            x =>
                                x.Nombre.Equals(
                                    "UCI",
                                    StringComparison.OrdinalIgnoreCase));


                if (areaUci != null)
                {
                    cmbArea.SelectedItem =
                        areaUci;
                }
            }
        }


        // =========================================================
        // ÁREA
        // =========================================================

        private void cmbArea_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoDatos)
                return;


            cmbHabitacion.ItemsSource =
                new List<Habitacion>();


            cmbCama.ItemsSource =
                new List<Cama>();


            MostrarDisponibilidadNeutral();


            if (cmbArea.SelectedItem
                is not AreaHospitalaria area)
            {
                return;
            }


            List<Habitacion> habitaciones =
                internamientoDAO
                    .ObtenerHabitacionesPorArea(
                        area.IdArea);


            cmbHabitacion.ItemsSource =
                habitaciones;
        }


        // =========================================================
        // HABITACIÓN
        // =========================================================

        private void cmbHabitacion_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoDatos)
                return;


            cmbCama.ItemsSource =
                new List<Cama>();


            MostrarDisponibilidadNeutral();


            if (cmbHabitacion.SelectedItem
                is not Habitacion habitacion)
            {
                return;
            }


            long? camaActual =
                esEdicion

                    ? internamientoActual?.IdCama
                    : null;


            List<Cama> camas =
                internamientoDAO
                    .ObtenerCamasDisponibles(
                        habitacion.IdHabitacion,
                        camaActual);


            cmbCama.ItemsSource =
                camas;
        }


        // =========================================================
        // CAMA
        // =========================================================

        private void cmbCama_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (cargandoDatos)
                return;


            if (cmbCama.SelectedItem
                is not Cama cama)
            {
                MostrarDisponibilidadNeutral();

                return;
            }


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
                $"Cama {cama.CodigoCama} disponible para asignación.";
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
                "Seleccione área, habitación y cama.";
        }


        // =========================================================
        // CARGAR INTERNAMIENTO EXISTENTE
        // =========================================================

        private void CargarInternamientoExistente()
        {
            if (internamientoActual == null)
                return;


            cargandoDatos =
                true;


            txtCodigo.Text =
                internamientoActual
                    .CodigoInternamiento;


            txtTitulo.Text =
                soloLectura

                    ? "DETALLE DE INTERNAMIENTO"

                    : "EDITAR INTERNAMIENTO";


            txtSubtitulo.Text =
                soloLectura

                    ? "Información del ingreso hospitalario."

                    : "Modifique los datos del internamiento.";


            // =====================================================
            // PACIENTE
            // =====================================================

            cmbPaciente.ItemsSource =
                pacientes;


            cmbPaciente.SelectedItem =
                pacientes
                    .FirstOrDefault(
                        x =>
                            x.IdPaciente ==
                            internamientoActual.IdPaciente);


            // =====================================================
            // ORIGEN
            // =====================================================

            if (internamientoActual
                .IdEmergenciaOrigen
                .HasValue)
            {
                rbEmergencia.IsChecked =
                    true;


                rbIngresoDirecto.IsChecked =
                    false;


                cmbEmergencia.SelectedItem =
                    emergencias
                        .FirstOrDefault(
                            x =>
                                x.IdEmergencia ==
                                internamientoActual
                                    .IdEmergenciaOrigen
                                    .Value);
            }
            else
            {
                rbIngresoDirecto.IsChecked =
                    true;


                rbEmergencia.IsChecked =
                    false;
            }


            // =====================================================
            // TIPO
            // =====================================================

            cmbTipo.SelectedItem =
                tipos
                    .FirstOrDefault(
                        x =>
                            x.IdTipoInternamiento ==
                            internamientoActual
                                .IdTipoInternamiento);


            // =====================================================
            // ESPECIALIDAD
            // =====================================================

            EspecialidadCitaOpcion? especialidad =
                especialidades
                    .FirstOrDefault(
                        x =>
                            x.IdEspecialidad ==
                            internamientoActual
                                .IdEspecialidad);


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
                            internamientoActual
                                .IdMedicoResponsable);


            // =====================================================
            // FECHA / HORA
            // =====================================================

            dpFechaIngreso.SelectedDate =
                internamientoActual
                    .FechaIngreso.Date;


            txtHoraIngreso.Text =
                internamientoActual
                    .FechaIngreso
                    .ToString(
                        "HH:mm");


            // =====================================================
            // ÁREA
            // =====================================================

            AreaHospitalaria? area =
                areas
                    .FirstOrDefault(
                        x =>
                            x.IdArea ==
                            internamientoActual.IdArea);


            cmbArea.SelectedItem =
                area;


            if (area != null)
            {
                List<Habitacion> habitaciones =
                    internamientoDAO
                        .ObtenerHabitacionesPorArea(
                            area.IdArea);


                cmbHabitacion.ItemsSource =
                    habitaciones;


                Habitacion? habitacion =
                    habitaciones
                        .FirstOrDefault(
                            x =>
                                x.IdHabitacion ==
                                internamientoActual
                                    .IdHabitacion);


                cmbHabitacion.SelectedItem =
                    habitacion;


                if (habitacion != null)
                {
                    List<Cama> camas =
                        internamientoDAO
                            .ObtenerCamasDisponibles(
                                habitacion
                                    .IdHabitacion,
                                internamientoActual
                                    .IdCama);


                    cmbCama.ItemsSource =
                        camas;


                    cmbCama.SelectedItem =
                        camas
                            .FirstOrDefault(
                                x =>
                                    x.IdCama ==
                                    internamientoActual
                                        .IdCama);
                }
            }


            txtMotivo.Text =
                internamientoActual
                    .MotivoInternamiento;


            txtDiagnostico.Text =
                internamientoActual
                    .DiagnosticoIngreso;


            txtObservaciones.Text =
                internamientoActual
                    .ObservacionesIngreso;


            cargandoDatos =
                false;


            ActualizarOrigen();


            if (cmbCama.SelectedItem
                is Cama cama)
            {
                txtDisponibilidad.Text =
                    $"Cama {cama.CodigoCama} asignada actualmente.";
            }
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

                        : "Registrar internamiento";


                return;
            }


            rbIngresoDirecto.IsEnabled =
                false;

            rbEmergencia.IsEnabled =
                false;

            cmbEmergencia.IsEnabled =
                false;

            cmbPaciente.IsEnabled =
                false;

            cmbTipo.IsEnabled =
                false;

            cmbEspecialidad.IsEnabled =
                false;

            cmbMedico.IsEnabled =
                false;

            dpFechaIngreso.IsEnabled =
                false;

            txtHoraIngreso.IsReadOnly =
                true;

            cmbArea.IsEnabled =
                false;

            cmbHabitacion.IsEnabled =
                false;

            cmbCama.IsEnabled =
                false;

            txtMotivo.IsReadOnly =
                true;

            txtDiagnostico.IsReadOnly =
                true;

            txtObservaciones.IsReadOnly =
                true;


            btnGuardar.Visibility =
                Visibility.Collapsed;


            btnCancelar.Content =
                "Cerrar";
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


            if (rbEmergencia.IsChecked == true &&
                cmbEmergencia.SelectedItem
                is not EmergenciaInternamientoOpcion)
            {
                MessageBox.Show(
                    "Debe seleccionar la emergencia de origen.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (cmbTipo.SelectedItem
                is not TipoInternamiento)
            {
                MessageBox.Show(
                    "Debe seleccionar el tipo de internamiento.",
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
                    "Debe seleccionar el médico responsable.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (!dpFechaIngreso.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Debe seleccionar la fecha de ingreso.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (!TimeSpan.TryParse(
                txtHoraIngreso.Text.Trim(),
                out TimeSpan hora))
            {
                MessageBox.Show(
                    "La hora de ingreso no es válida.\n\nUtilice el formato HH:mm, por ejemplo: 16:30.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtHoraIngreso.Focus();

                return false;
            }


            DateTime fechaIngreso =
                dpFechaIngreso
                    .SelectedDate
                    .Value
                    .Date
                    .Add(hora);


            if (fechaIngreso >
                DateTime.Now.AddMinutes(5))
            {
                MessageBox.Show(
                    "La fecha de ingreso no puede estar en el futuro.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (cmbArea.SelectedItem
                is not AreaHospitalaria)
            {
                MessageBox.Show(
                    "Debe seleccionar un área hospitalaria.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (cmbHabitacion.SelectedItem
                is not Habitacion)
            {
                MessageBox.Show(
                    "Debe seleccionar una habitación.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (cmbCama.SelectedItem
                is not Cama)
            {
                MessageBox.Show(
                    "Debe seleccionar una cama disponible.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }


            if (string.IsNullOrWhiteSpace(
                txtMotivo.Text))
            {
                MessageBox.Show(
                    "Debe indicar el motivo del internamiento.",
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


                TipoInternamiento tipo =
                    (TipoInternamiento)
                    cmbTipo.SelectedItem;


                EspecialidadCitaOpcion especialidad =
                    (EspecialidadCitaOpcion)
                    cmbEspecialidad.SelectedItem;


                MedicoCitaOpcion medico =
                    (MedicoCitaOpcion)
                    cmbMedico.SelectedItem;


                Cama cama =
                    (Cama)
                    cmbCama.SelectedItem;


                TimeSpan.TryParse(
                    txtHoraIngreso.Text.Trim(),
                    out TimeSpan hora);


                DateTime fechaIngreso =
                    dpFechaIngreso
                        .SelectedDate!
                        .Value
                        .Date
                        .Add(hora);


                // =================================================
                // VALIDAR PACIENTE
                // =================================================

                if (internamientoDAO
                    .PacienteTieneInternamientoActivo(
                        paciente.IdPaciente,
                        esEdicion
                            ? internamientoActual
                                ?.IdInternamiento
                            : null))
                {
                    MessageBox.Show(
                        "Este paciente ya posee un internamiento activo.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }





                // =================================================
                // CREAR OBJETO
                // =================================================

                Internamiento internamiento =
                    internamientoActual
                    ?? new Internamiento();


                internamiento.IdPaciente =
                    paciente.IdPaciente;


                internamiento.IdEmergenciaOrigen =
                    rbEmergencia.IsChecked == true

                        ? ((EmergenciaInternamientoOpcion)
                            cmbEmergencia.SelectedItem)
                            .IdEmergencia

                        : null;


                internamiento.IdTipoInternamiento =
                    tipo.IdTipoInternamiento;


                internamiento.IdEspecialidad =
                    especialidad.IdEspecialidad;


                internamiento.IdMedicoResponsable =
                    medico.IdMedico;


                internamiento.IdCama =
                    cama.IdCama;


                internamiento.FechaIngreso =
                    fechaIngreso;


                internamiento.MotivoInternamiento =
                    txtMotivo.Text.Trim();


                internamiento.DiagnosticoIngreso =
                    txtDiagnostico.Text.Trim();


                internamiento.ObservacionesIngreso =
                    txtObservaciones.Text.Trim();


                if (!esEdicion)
                {
                    internamiento
                        .IdEstadoInternamiento = 1;
                }


                // =================================================
                // INSERTAR / EDITAR
                // =================================================

                bool resultado =
                    esEdicion

                        ? internamientoDAO
                            .Actualizar(
                                internamiento)

                        : internamientoDAO
                            .Insertar(
                                internamiento);


                if (!resultado)
                {
                    MessageBox.Show(
                        "No fue posible guardar el internamiento.",
                        "MediSys",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }


                MessageBox.Show(
                    esEdicion

                        ? "El internamiento fue actualizado correctamente."

                        : $"Internamiento registrado correctamente.\n\nCódigo: {internamiento.CodigoInternamiento}",

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
                    $"No fue posible guardar el internamiento.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // =========================================================
        // CERRAR
        // =========================================================

        private void btnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Close();
        }
    }
}