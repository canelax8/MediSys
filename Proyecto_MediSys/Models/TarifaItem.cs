using System;

namespace Proyecto_MediSys.Models
{
    public class TarifaItem
    {
        public long IdTarifa { get; set; }

        public long IdItemClinico { get; set; }

        public int IdPlanTarifario { get; set; }

        public string NombreItem { get; set; } = "";

        public string NombrePlan { get; set; } = "";

        public decimal Precio { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }


        public string PrecioTexto
        {
            get
            {
                return $"RD$ {Precio:N2}";
            }
        }
    }
}