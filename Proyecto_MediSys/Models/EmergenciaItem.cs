using System;

namespace Proyecto_MediSys.Models
{
    public class EmergenciaItem
    {
        public long IdEmergenciaItem { get; set; }

        public long IdEmergencia { get; set; }

        public long IdItemClinico { get; set; }

        public string Codigo { get; set; } = "";

        public string Nombre { get; set; } = "";

        public string TipoItem { get; set; } = "";

        public decimal Cantidad { get; set; } = 1;

        public decimal PrecioUnitarioAplicado { get; set; }

        public int? IdPlanTarifarioAplicado { get; set; }

        public string NombrePlanTarifario { get; set; } = "";

        public string Estado { get; set; } = "Registrado";

        public string Observaciones { get; set; } = "";

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; }


        // =============================================
        // Información para medicamentos
        // =============================================

        public string Dosis { get; set; } = "";

        public string ViaAdministracion { get; set; } = "";

        public string Frecuencia { get; set; } = "";

        public string Indicaciones { get; set; } = "";


        // =============================================
        // Propiedades calculadas
        // =============================================

        public decimal Total
        {
            get
            {
                return Cantidad * PrecioUnitarioAplicado;
            }
        }


        public string PrecioTexto
        {
            get
            {
                return $"RD$ {PrecioUnitarioAplicado:N2}";
            }
        }


        public string TotalTexto
        {
            get
            {
                return $"RD$ {Total:N2}";
            }
        }


        public string Mostrar
        {
            get
            {
                return $"{Codigo} - {Nombre}";
            }
        }
    }
}