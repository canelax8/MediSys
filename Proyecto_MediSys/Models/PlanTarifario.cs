using System;

namespace Proyecto_MediSys.Models
{
    public class PlanTarifario
    {
        public int IdPlanTarifario { get; set; }

        public string Nombre { get; set; } = "";

        public string Tipo { get; set; } = "";

        public int? IdSeguro { get; set; }

        public string NombreSeguro { get; set; } = "";

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }


        public bool EsPrivado
        {
            get
            {
                return Tipo.Equals(
                    "PRIVADO",
                    StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}