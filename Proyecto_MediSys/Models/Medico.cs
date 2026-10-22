using System;

namespace Proyecto_MediSys.Models
{
    public class Medico
    {
        public long IdMedico { get; set; }

        public string CodigoMedico { get; set; } = "";

        public string Nombre { get; set; } = "";

        public string Apellido { get; set; } = "";

        public string Cedula { get; set; } = "";

        public string Exequatur { get; set; } = "";

        public string Telefono { get; set; } = "";

        public string Correo { get; set; } = "";

        public long? IdEspecialidad { get; set; }

        public string NombreEspecialidad { get; set; } = "";

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }


        // ============================================================
        // PROPIEDADES PARA MOSTRAR
        // ============================================================

        public string NombreCompleto =>
            $"{Nombre} {Apellido}".Trim();


        public string Estado =>
            Activo
                ? "Activo"
                : "Inactivo";


        public string AccionEstado =>
            Activo
                ? "Desactivar"
                : "Activar";
    }


    // ================================================================
    // CATÁLOGO DE ESPECIALIDADES
    // ================================================================

    public class EspecialidadMedicoOpcion
    {
        public long IdEspecialidad { get; set; }

        public string CodigoEspecialidad { get; set; } = "";

        public string Nombre { get; set; } = "";

        public string Descripcion { get; set; } = "";
    }
}