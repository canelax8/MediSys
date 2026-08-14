using Proyecto_MediSys.Models;

namespace Proyecto_MediSys.Helpers
{
    public static class SesionActual
    {
        public static Usuario? Usuario { get; set; }

        public static bool HaySesion
        {
            get
            {
                return Usuario != null;
            }
        }

        public static void CerrarSesion()
        {
            Usuario = null;
        }
    }
}