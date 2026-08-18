using System;

namespace Proyecto_MediSys.Models
{
    public class Internamiento
    {
        // =========================================================
        // IDENTIFICACIÓN
        // =========================================================

        public long IdInternamiento { get; set; }

        public string CodigoInternamiento { get; set; } = "";


        // =========================================================
        // PACIENTE
        // =========================================================

        public int IdPaciente { get; set; }

        public string NombrePaciente { get; set; } = "";

        public string CodigoPaciente { get; set; } = "";

        public string DocumentoPaciente { get; set; } = "";

        public string TelefonoPaciente { get; set; } = "";

        public string SeguroPaciente { get; set; } = "";


        // =========================================================
        // ORIGEN
        // =========================================================

        public long? IdEmergenciaOrigen { get; set; }

        public string CodigoEmergenciaOrigen { get; set; } = "";

        public bool VieneDeEmergencia =>
            IdEmergenciaOrigen.HasValue;


        public string OrigenMostrar =>
            VieneDeEmergencia
                ? $"Emergencia {CodigoEmergenciaOrigen}"
                : "Ingreso directo";


        // =========================================================
        // MÉDICO
        // =========================================================

        public long IdMedicoResponsable { get; set; }

        public string NombreMedico { get; set; } = "";


        public long IdEspecialidad { get; set; }

        public string Especialidad { get; set; } = "";


        // =========================================================
        // TIPO / ESTADO
        // =========================================================

        public int IdTipoInternamiento { get; set; }

        public string TipoInternamiento { get; set; } = "";


        public int IdEstadoInternamiento { get; set; }

        public string Estado { get; set; } = "";


        // =========================================================
        // UBICACIÓN
        // =========================================================

        public long IdCama { get; set; }

        public string CodigoCama { get; set; } = "";


        public long IdHabitacion { get; set; }

        public string Habitacion { get; set; } = "";


        public long IdArea { get; set; }

        public string Area { get; set; } = "";

        public int? Piso { get; set; }


        // =========================================================
        // INGRESO
        // =========================================================

        public DateTime FechaIngreso { get; set; }

        public string MotivoInternamiento { get; set; } = "";

        public string DiagnosticoIngreso { get; set; } = "";

        public string ObservacionesIngreso { get; set; } = "";


        // =========================================================
        // ALTA
        // =========================================================

        public DateTime? FechaAlta { get; set; }

        public string ObservacionesAlta { get; set; } = "";


        // =========================================================
        // AUDITORÍA
        // =========================================================

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }


        // =========================================================
        // PROPIEDADES PARA MOSTRAR
        // =========================================================

        public string FechaIngresoMostrar =>
            FechaIngreso.ToString(
                "dd/MM/yyyy hh:mm tt");


        public string FechaAltaMostrar =>
            FechaAlta.HasValue
                ? FechaAlta.Value.ToString(
                    "dd/MM/yyyy hh:mm tt")
                : "—";


        public string UbicacionMostrar =>
            $"{Area} / Hab. {Habitacion} / Cama {CodigoCama}";
    }
}