    namespace Proyecto_MediSys.Models
    {
        public class InformacionClinica
        {
            public long IdInformacionClinica { get; set; }

            public long IdEmergencia { get; set; }

            public string MotivoConsulta { get; set; } = "";

            public bool Diabetes { get; set; }

            public bool Hipertension { get; set; }

            public bool Asma { get; set; }

            public bool Cardiopatia { get; set; }

            public bool Embarazo { get; set; }

            public bool Ninguno { get; set; }

            public string Alergias { get; set; } = "";

            public string MedicamentosActuales { get; set; } = "";

            public string Observaciones { get; set; } = "";
        }
    }