namespace Proyecto_MediSys.Models
{
    public class DiagnosticoEmergencia
    {
        public long IdDiagnostico { get; set; }

        public long IdEmergencia { get; set; }

        public string DiagnosticoPrincipal { get; set; } = "";

        public string DiagnosticoSecundario { get; set; } = "";

        public string ImpresionClinica { get; set; } = "";

        public string Observaciones { get; set; } = "";
    }
}