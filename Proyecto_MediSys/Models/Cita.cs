using System;

namespace Proyecto_MediSys.Models
{
    public class Cita
    {
        // =========================================================
        // IDENTIFICACIÓN
        // =========================================================

        public long IdCita { get; set; }

        public string CodigoCita { get; set; } = "";


        // =========================================================
        // PACIENTE
        // =========================================================

        public int IdPaciente { get; set; }

        public string NombrePaciente { get; set; } = "";


        // =========================================================
        // MÉDICO
        // =========================================================

        public long IdMedico { get; set; }

        public string NombreMedico { get; set; } = "";


        // =========================================================
        // ESPECIALIDAD
        // =========================================================

        public long IdEspecialidad { get; set; }

        public string Especialidad { get; set; } = "";


        // =========================================================
        // ESTADO
        // =========================================================

        public int IdEstadoCita { get; set; }

        public string Estado { get; set; } = "";


        // =========================================================
        // FECHA / HORA
        // =========================================================

        public DateTime FechaCita { get; set; }

        public TimeSpan HoraCita { get; set; }


        // =========================================================
        // DATOS CLÍNICOS / ADMINISTRATIVOS
        // =========================================================

        public string Motivo { get; set; } = "";

        public string Observaciones { get; set; } = "";


        // =========================================================
        // AUDITORÍA
        // =========================================================

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }


        // =========================================================
        // PROPIEDADES PARA MOSTRAR
        // =========================================================

        public DateTime FechaHora
        {
            get
            {
                return FechaCita.Date
                    .Add(HoraCita);
            }
        }


        public string FechaMostrar
        {
            get
            {
                return FechaCita
                    .ToString("dd/MM/yyyy");
            }
        }


        public string HoraMostrar
        {
            get
            {
                return DateTime.Today
                    .Add(HoraCita)
                    .ToString("hh:mm tt");
            }
        }


        public string FechaHoraMostrar
        {
            get
            {
                return $"{FechaMostrar} {HoraMostrar}";
            }
        }
    }
}