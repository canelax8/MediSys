namespace Proyecto_MediSys.Models
{
    public class EmergenciaDiagnostico
    {
        public long IdEmergenciaDiagnostico { get; set; }

        public long IdEmergencia { get; set; }

        public long? IdCIE10 { get; set; }

        public string CodigoCIE10 { get; set; } = "";

        public string DescripcionCIE10 { get; set; } = "";

        public string DiagnosticoTexto { get; set; } = "";

        public bool EsPrincipal { get; set; }

        public string Observaciones { get; set; } = "";

        public bool Activo { get; set; }

        public DateTime FechaRegistro { get; set; }


        public string DiagnosticoMostrar
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CodigoCIE10))
                {
                    return $"{CodigoCIE10} - {DescripcionCIE10}";
                }

                return DiagnosticoTexto;
            }
        }


        public string TipoTexto
        {
            get
            {
                return EsPrincipal
                    ? "Principal"
                    : "Secundario";
            }
        }
    }
}