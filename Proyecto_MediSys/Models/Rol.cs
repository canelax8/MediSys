namespace Proyecto_MediSys.Models
{
    public class Rol
    {
        public long IdRol { get; set; }

        public string CodigoRol { get; set; } = "";

        public string Nombre { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public bool Activo { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}