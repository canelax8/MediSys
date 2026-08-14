using Proyecto_MediSys.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Controls
{
    public partial class PasoDestino : UserControl
    {
        private DestinoEmergencia destino = new();

        public PasoDestino()
        {
            InitializeComponent();
        }


        // ============================================================
        // VALIDAR
        // ============================================================

        public bool Validar()
        {
            if (rbAlta.IsChecked != true &&
                rbObservacion.IsChecked != true &&
                rbHospitalizacion.IsChecked != true &&
                rbUCI.IsChecked != true &&
                rbQuirofano.IsChecked != true &&
                rbTraslado.IsChecked != true)
            {
                MessageBox.Show(
                    "Debe seleccionar el destino del paciente.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }


        // ============================================================
        // OBTENER DESTINO
        // ============================================================

        public DestinoEmergencia ObtenerDestino()
        {
            destino = new DestinoEmergencia();

            // ========================================================
            // ALTA
            // ========================================================

            if (rbAlta.IsChecked == true)
            {
                destino.Destino = "Alta";

                destino.IdEstadoEmergenciaResultado = 5;

                destino.FechaSalida = DateTime.Now;
            }

            // ========================================================
            // OBSERVACIÓN
            // ========================================================

            else if (rbObservacion.IsChecked == true)
            {
                destino.Destino = "Observación";

                destino.IdEstadoEmergenciaResultado = 3;

                destino.FechaSalida = null;
            }

            // ========================================================
            // HOSPITALIZACIÓN
            // ========================================================

            else if (rbHospitalizacion.IsChecked == true)
            {
                destino.Destino = "Hospitalización";

                destino.IdEstadoEmergenciaResultado = 4;

                destino.FechaSalida = null;
            }

            // ========================================================
            // UCI
            // ========================================================

            else if (rbUCI.IsChecked == true)
            {
                destino.Destino = "UCI";

                // Por ahora UCI se considera hospitalización
                destino.IdEstadoEmergenciaResultado = 4;

                destino.FechaSalida = null;
            }

            // ========================================================
            // QUIRÓFANO
            // ========================================================

            else if (rbQuirofano.IsChecked == true)
            {
                destino.Destino = "Quirófano";

                // Continúa siendo un paciente hospitalizado
                destino.IdEstadoEmergenciaResultado = 4;

                destino.FechaSalida = null;
            }

            // ========================================================
            // TRASLADO
            // ========================================================

            else if (rbTraslado.IsChecked == true)
            {
                destino.Destino = "Traslado";

                destino.IdEstadoEmergenciaResultado = 6;

                destino.FechaSalida = DateTime.Now;
            }


            destino.ObservacionesFinales =
                txtObservacionesFinales.Text.Trim();


            return destino;
        }
    }
}