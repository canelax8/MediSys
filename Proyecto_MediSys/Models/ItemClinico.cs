using System;

namespace Proyecto_MediSys.Models
{
    public class ItemClinico
    {
        public long IdItemClinico { get; set; }

        public string Codigo { get; set; } = "";

        public string Nombre { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public int IdTipoItem { get; set; }

        public string TipoItem { get; set; } = "";

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }


        // =========================================
        // Información específica de medicamento
        // =========================================

        public string PrincipioActivo { get; set; } = "";

        public string Concentracion { get; set; } = "";

        public string Presentacion { get; set; } = "";

        public string FormaFarmaceutica { get; set; } = "";


        // =========================================
        // Propiedades para interfaz
        // =========================================

        public string Mostrar
        {
            get
            {
                if (TipoItem == "Medicamento")
                {
                    string detalle = "";

                    if (!string.IsNullOrWhiteSpace(Concentracion))
                        detalle += " " + Concentracion;

                    if (!string.IsNullOrWhiteSpace(Presentacion))
                        detalle += " - " + Presentacion;

                    return $"{Codigo} - {Nombre}{detalle}";
                }

                return $"{Codigo} - {Nombre}";
            }
        }
    }
}