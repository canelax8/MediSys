namespace Proyecto_MediSys.Models
{
    public class EmergenciaMedicamento
    {
        public long IdEmergenciaItem { get; set; }

        public string Dosis { get; set; } = "";

        public string ViaAdministracion { get; set; } = "";

        public string Frecuencia { get; set; } = "";

        public string Indicaciones { get; set; } = "";
    }
}