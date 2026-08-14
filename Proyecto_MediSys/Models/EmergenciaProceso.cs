using System;

namespace Proyecto_MediSys.Models
{
    public class EmergenciaProceso
    {
        //=========================
        // Paciente
        //=========================

        public Paciente? Paciente { get; set; }

        //=========================
        // Evaluación
        //=========================

        public string MotivoConsulta { get; set; } = "";

        public string Antecedentes { get; set; } = "";

        public string Alergias { get; set; } = "";

        public decimal Temperatura { get; set; }

        public string PresionArterial { get; set; } = "";

        public int FrecuenciaCardiaca { get; set; }

        public int FrecuenciaRespiratoria { get; set; }

        public int Saturacion { get; set; }

        public decimal Peso { get; set; }

        public string Prioridad { get; set; } = "";

        //=========================
        // Diagnóstico
        //=========================

        public string Diagnostico { get; set; } = "";

        //=========================
        // Tratamiento
        //=========================

        public string Tratamiento { get; set; } = "";

        public string Observaciones { get; set; } = "";

        //=========================
        // Generales
        //=========================

        public DateTime FechaIngreso { get; set; } = DateTime.Now;
    }
}