using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto_MediSys.Models
{
    public class Seguro
    {
        public int IdSeguro { get; set; }

        public string CodigoSeguro { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public bool Activo { get; set; }
    }
}