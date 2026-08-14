using System;
using System.Collections.Generic;

namespace Proyecto_MediSys.Models
{
    public class ProcesoEmergencia
    {
        public Paciente Paciente { get; set; }

        public EvaluacionInicial Evaluacion { get; set; }

        public InformacionClinica InformacionClinica { get; set; }

        public DiagnosticoEmergencia Diagnostico { get; set; }
        public List<CIE10> DiagnosticosSeleccionados { get; set; }
                         = new List<CIE10>();

        public List<EmergenciaItem> ItemsClinicos { get; set; }
    = new List<EmergenciaItem>();

        public CIE10? DiagnosticoPrincipalCIE10 { get; set; }

        public List<string> DiagnosticosManuales { get; set; }
            = new List<string>();

        public ProcedimientoEmergencia Procedimientos { get; set; }

        public DestinoEmergencia Destino { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}