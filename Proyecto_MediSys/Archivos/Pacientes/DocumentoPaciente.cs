using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto_MediSys.Archivos.Pacientes
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

        // Para mostrar en el DataGrid
        public string FechaRegistro
        {
            get
            {
                return FechaSubida.ToString("dd/MM/yyyy");
            }
        }

        public string Tamano
        {
            get
            {
                return TamanoKB.ToString("N1") + " KB";
            }
        }

        public string Estado
        {
            get
            {
                return Activo ? "Activo" : "Eliminado";
            }
        }
    }
}
