namespace Proyecto_MediSys.Models
{
    public class Alergia
    {
        public long IdAlergia { get; set; }

        public string Nombre { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}