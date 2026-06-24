using System;

namespace Proyecto_MediSys.Models
{
    public class Paciente
    {
        public int IdPaciente { get; set; }
        public string CodigoPaciente { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string? SegundoNombre { get; set; }

        public string Apellido { get; set; } = string.Empty;

        public string? SegundoApellido { get; set; }

        public string NombreTipoPaciente { get; set; } = string.Empty;

        public string NombreSeguro { get; set; } = string.Empty;

        public string NombreEstadoPaciente { get; set; } = string.Empty;

        public string TipoDocumento { get; set; }//

        public string NumeroDocumento { get; set; }//

        public string CodigoTemporal { get; set; }//

        public bool Indocumentado { get; set; }//

        public DateTime FechaNacimiento { get; set; }

        public string Sexo { get; set; }//

        public string Telefono { get; set; } = string.Empty;

        public string? Correo { get; set; }

        public string Direccion { get; set; } = string.Empty;
        public int IdTipoPaciente { get; set; }

        public int IdSeguro { get; set; }

        public int IdEstadoPaciente { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public bool Activo { get; set; }

        //======================
        // Propiedades calculadas
        //======================

        public string NombreCompleto
        {
            get
            {
                return $"{Nombre} {SegundoNombre} {Apellido} {SegundoApellido}"
                    .Replace("  ", " ")
                    .Trim();
            }
        }

        public string FechaRegistroTexto
        {
            get { return FechaCreacion.ToString("dd/MM/yyyy"); }
        }

        public string HoraRegistroTexto
        {
            get { return FechaCreacion.ToString("hh:mm tt"); }
        }

        public string EstadoTexto
        {
            get { return Activo ? "Activo" : "Inactivo"; }
        }
    }
}