namespace Proyecto_MediSys.Models
{
    public class EstadoInternamiento
    {
        public int IdEstadoInternamiento { get; set; }
        public string Nombre { get; set; } = "";
        public bool Activo { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }


    public class TipoInternamiento
    {
        public int IdTipoInternamiento { get; set; }
        public string Nombre { get; set; } = "";
        public bool Activo { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }


    public class AreaHospitalaria
    {
        public long IdArea { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public bool Activo { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }


    public class Habitacion
    {
        public long IdHabitacion { get; set; }

        public string CodigoHabitacion { get; set; } = "";

        public string NumeroHabitacion { get; set; } = "";

        public long IdArea { get; set; }

        public string Area { get; set; } = "";

        public int? Piso { get; set; }

        public string Descripcion { get; set; } = "";

        public bool Activo { get; set; }


        public string Mostrar =>
            string.IsNullOrWhiteSpace(NumeroHabitacion)
                ? CodigoHabitacion
                : $"Habitación {NumeroHabitacion}";


        public override string ToString()
        {
            return Mostrar;
        }
    }


    public class EstadoCama
    {
        public int IdEstadoCama { get; set; }

        public string Nombre { get; set; } = "";

        public bool Activo { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }


    public class Cama
    {
        public long IdCama { get; set; }

        public string CodigoCama { get; set; } = "";

        public long IdHabitacion { get; set; }

        public string Habitacion { get; set; } = "";

        public long IdArea { get; set; }

        public string Area { get; set; } = "";

        public int IdEstadoCama { get; set; }

        public string Estado { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public bool Activo { get; set; }


        public override string ToString()
        {
            return CodigoCama;
        }

        public class EmergenciaInternamientoOpcion
        {
            public long IdEmergencia { get; set; }

            public string CodigoEmergencia { get; set; } = "";

            public int IdPaciente { get; set; }

            public string NombrePaciente { get; set; } = "";

            public long IdMedico { get; set; }

            public long IdEspecialidad { get; set; }

            public string MotivoConsulta { get; set; } = "";


            public string Mostrar =>
                $"{CodigoEmergencia} - {NombrePaciente}";


            public override string ToString()
            {
                return Mostrar;
            }
        }
    }
}