using MaterialDesignThemes.Wpf;

namespace Proyecto_MediSys.Services
{
    public static class DialogService
    {
        public static async Task Mostrar(object contenido)
        {
            await DialogHost.Show(contenido, "DialogHostPrincipal");
        }
    }
}