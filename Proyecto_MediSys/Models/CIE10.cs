namespace Proyecto_MediSys.Models
{
    public class CIE10
    {
        public long IdCIE10 { get; set; }

        public string Codigo { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public string Categoria { get; set; } = "";

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }


        public string Mostrar
        {
            get
            {
                return $"{Codigo} - {Descripcion}";
            }
        }
    }
}