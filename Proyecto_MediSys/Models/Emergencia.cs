namespace Proyecto_MediSys.Models
{
    public class Emergencia
    {
        public long IdEmergencia { get; set; }

        public string CodigoEmergencia { get; set; } = "";

        public long IdPaciente { get; set; }

        public string NombrePaciente { get; set; } = "";

        public long IdMedico { get; set; }

        public string NombreMedico { get; set; } = "";

        public long IdEspecialidad { get; set; }

        public string Especialidad { get; set; } = "";

        public DateTime FechaIngreso { get; set; }

        public long IdEstadoEmergencia { get; set; }

        public string Estado { get; set; } = "";

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public string MotivoConsulta { get; set; } = "";
    }
}