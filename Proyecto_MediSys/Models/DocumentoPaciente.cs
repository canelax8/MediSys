using System;

namespace Proyecto_MediSys.Models
{
    public class DocumentoPaciente
    {
        public int IdDocumento { get; set; }

        public int IdPaciente { get; set; }

        public string TipoDocumento { get; set; } = "";

        public string NombreArchivo { get; set; } = "";

        public string RutaArchivo { get; set; } = "";

        public string Extension { get; set; } = "";

        public decimal TamanoKB { get; set; }

        public DateTime FechaSubida { get; set; }

        public bool Activo { get; set; }
    }
}