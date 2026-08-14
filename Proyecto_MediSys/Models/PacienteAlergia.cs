namespace Proyecto_MediSys.Models
{
    public class PacienteAlergia
    {
        public long IdPacienteAlergia { get; set; }

        public int IdPaciente { get; set; }

        public long? IdAlergia { get; set; }

        public string NombreAlergia { get; set; } = "";

        public string AlergiaTexto { get; set; } = "";

        public string Observaciones { get; set; } = "";

        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }


        public string AlergiaMostrar
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(NombreAlergia))
                    return NombreAlergia;

                return AlergiaTexto;
            }
        }
    }
}