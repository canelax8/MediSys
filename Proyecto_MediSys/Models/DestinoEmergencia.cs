namespace Proyecto_MediSys.Models
{
    public class DestinoEmergencia
    {
        public long IdDestino { get; set; }

        public long IdEmergencia { get; set; }

        public string Destino { get; set; } = "";

        public string ObservacionesFinales { get; set; } = "";

        public long IdEstadoEmergenciaResultado { get; set; }
        public DateTime? FechaSalida { get; set; }
    }
}