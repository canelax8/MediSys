namespace Proyecto_MediSys.Models
{
    public class EstadoCita
    {
        public int IdEstadoCita { get; set; }

        public string Nombre { get; set; } = "";

        public bool Activo { get; set; }


        public override string ToString()
        {
            return Nombre;
        }
    }
}