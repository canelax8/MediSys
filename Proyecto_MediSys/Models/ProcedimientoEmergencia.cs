namespace Proyecto_MediSys.Models
{
    public class ProcedimientoEmergencia
    {
        public long IdProcedimiento { get; set; }

        public long IdEmergencia { get; set; }

        public string Medicamentos { get; set; } = "";

        public string Procedimientos { get; set; } = "";

        public string Laboratorios { get; set; } = "";

        public string Imagenes { get; set; } = "";
    }
}