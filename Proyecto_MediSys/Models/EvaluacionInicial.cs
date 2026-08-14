namespace Proyecto_MediSys.Models
{
    public class EvaluacionInicial
    {
        public long IdEvaluacion { get; set; }

        public long IdEmergencia { get; set; }

        public int NivelTriage { get; set; }

        public decimal? Temperatura { get; set; }

        public string PresionArterial { get; set; } = "";

        public int? FrecuenciaCardiaca { get; set; }

        public int? FrecuenciaRespiratoria { get; set; }

        public int? Saturacion { get; set; }

        public decimal? Glucemia { get; set; }

        public decimal? Peso { get; set; }

        public decimal? Talla { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}