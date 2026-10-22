using System;

namespace Proyecto_MediSys.Models
{
    public class InternamientoItem
    {
        public long IdInternamientoItem { get; set; }

        public long IdInternamiento { get; set; }

        public long IdItemClinico { get; set; }


        // ============================================================
        // DATOS DEL ITEM
        // ============================================================

        public string Codigo { get; set; } = "";

        public string Nombre { get; set; } = "";

        public string TipoItem { get; set; } = "";


        // ============================================================
        // CONSUMO
        // ============================================================

        public decimal Cantidad { get; set; }

        public decimal PrecioUnitarioAplicado { get; set; }

        public int? IdPlanTarifarioAplicado { get; set; }

        public string NombrePlanTarifario { get; set; } = "";


        // ============================================================
        // ESTADO
        // ============================================================

        public string Estado { get; set; } = "Registrado";

        public string Observaciones { get; set; } = "";

        public DateTime FechaRegistro { get; set; }

        public bool Activo { get; set; }


        // ============================================================
        // CALCULADOS
        // ============================================================

        public decimal Total =>
            Cantidad * PrecioUnitarioAplicado;


        public string PrecioTexto =>
            $"RD$ {PrecioUnitarioAplicado:N2}";


        public string TotalTexto =>
            $"RD$ {Total:N2}";


        public string CantidadTexto =>
            Cantidad.ToString("N2");


        public string FechaMostrar =>
            FechaRegistro.ToString(
                "dd/MM/yyyy HH:mm");
    }
}