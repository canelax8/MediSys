using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto_MediSys.Models
{
    public class Usuario
    {
        public long IdUsuario { get; set; }

        public string CodigoUsuario { get; set; } = "";

        public string Nombre { get; set; } = "";

        public string Apellido { get; set; } = "";

        public string? SegundoApellido { get; set; }

        public string UsuarioLogin { get; set; } = "";

        public string ClaveHash { get; set; } = "";

        public string Correo { get; set; } = "";

        public string? Telefono { get; set; }

        public long IdRol { get; set; }
        public long? IdMedico { get; set; }

        public long? IdEspecialidad { get; set; }
        public string NombreMedico { get; set; } = "";

        // Este campo es para mostrar el nombre del rol
        // cuando hagamos el JOIN con tbRoles.
        public string NombreRol { get; set; } = "";

        public bool Activo { get; set; }

        public bool DebeCambiarClave { get; set; }

        public short IntentosFallidos { get; set; }

        public DateTime? UltimoAcceso { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public string NombreCompleto
        {
            get
            {
                return $"{Nombre} {Apellido}";
            }
        }

        public string Iniciales
        {
            get
            {
                string ini = "";

                if (!string.IsNullOrWhiteSpace(Nombre))
                    ini += Nombre.Substring(0, 1).ToUpper();

                if (!string.IsNullOrWhiteSpace(Apellido))
                    ini += Apellido.Substring(0, 1).ToUpper();

                return ini;
            }
        }

        public string EstadoTexto
        {
            get
            {
                return Activo ? "Activo" : "Inactivo";
            }
        }
    }

}
