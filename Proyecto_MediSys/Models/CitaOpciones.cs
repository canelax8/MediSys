namespace Proyecto_MediSys.Models
{
    public class EspecialidadCitaOpcion
    {
        public long IdEspecialidad { get; set; }

        public string Nombre { get; set; } = "";

        public override string ToString()
        {
            return Nombre;
        }
    }


    public class MedicoCitaOpcion
    {
        public long IdMedico { get; set; }

        public long IdEspecialidad { get; set; }

        public string NombreCompleto { get; set; } = "";

        public string Especialidad { get; set; } = "";

        public override string ToString()
        {
            return NombreCompleto;
        }
    }
}