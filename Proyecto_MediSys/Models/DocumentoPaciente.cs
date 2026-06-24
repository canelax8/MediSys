using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto_MediSys.Models
{

    public class DocumentoPaciente
    {
        public int IdDocumento { get; set; }

        public int IdPaciente { get; set; }

        public string TipoDocumento { get; set; } = "";

        public string NombreArchivo { get; set; } = "";

        public string RutaArchivo { get; set; } = "";

        public DateTime FechaRegistro { get; set; }

        public string Tamano { get; set; } = "";

        public string Estado { get; set; } = "";
    }
}
