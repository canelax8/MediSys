using Proyecto_MediSys.Data;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Proyecto_MediSys.Pages.Procesos
{
    public partial class FacturacionPage : Page
    {
        // ============================================================
        // DAO
        // ============================================================

        private readonly FacturacionDAO facturacionDAO =
            new FacturacionDAO();


        // ============================================================
        // LISTAS
        // ============================================================

        private List<EpisodioFacturable> emergencias =
            new List<EpisodioFacturable>();


        private List<EpisodioFacturable> internamientos =
            new List<EpisodioFacturable>();


        private List<FacturaResumen> historial =
            new List<FacturaResumen>();


        // ============================================================
        // FACTURA ACTUAL
        // ============================================================

        private EpisodioFacturable? episodioSeleccionado;


        private List<DetalleFacturable> detalleActual =
            new List<DetalleFacturable>();


        private string tipoFacturaActual =
            "";


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public FacturacionPage()
        {
            InitializeComponent();

            CargarDatos();

            MostrarInicio();
        }


        // ============================================================
        // CARGAR
        // ============================================================

        private void CargarDatos()
        {
            try
            {
                // ====================================================
                // EMERGENCIAS
                // ====================================================

                emergencias =
                    facturacionDAO
                        .ObtenerEmergenciasPendientes();


                // ====================================================
                // INTERNAMIENTOS
                // ====================================================

                internamientos =
                    facturacionDAO
                        .ObtenerInternamientosPendientes();


                // ====================================================
                // HISTORIAL
                // ====================================================

                historial =
                    facturacionDAO
                        .ObtenerFacturasRecientes(
                            500);


                // ====================================================
                // GRIDS
                // ====================================================

                dgEmergencias.ItemsSource =
                    emergencias;


                dgInternamientosFacturacion.ItemsSource =
                    internamientos;


                dgHistorial.ItemsSource =
                    historial;


                ActualizarIndicadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar Facturación.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // INDICADORES
        // ============================================================

        private void ActualizarIndicadores()
        {
            int facturasHoy =
                historial.Count(
                    x =>
                        x.FechaFactura.Date ==
                        DateTime.Today);


            int pendientes =
                emergencias.Count +
                internamientos.Count;


            decimal montoPaciente =

                emergencias.Sum(
                    x => x.TotalPaciente)

                +

                internamientos.Sum(
                    x => x.TotalPaciente);


            decimal montoARS =

                emergencias.Sum(
                    x => x.TotalSeguro)

                +

                internamientos.Sum(
                    x => x.TotalSeguro);


            txtFacturasHoy.Text =
                facturasHoy.ToString();


            txtPendientes.Text =
                pendientes.ToString();


            txtMontoPaciente.Text =
                $"RD$ {montoPaciente:N2}";


            txtMontoARS.Text =
                $"RD$ {montoARS:N2}";


            txtCantidadEmergencias.Text =
                $"{emergencias.Count} pendientes";


            txtCantidadInternamientos.Text =
                $"{internamientos.Count} pendientes";
        }


        // ============================================================
        // OCULTAR PANELES
        // ============================================================

        private void OcultarPaneles()
        {
            panelInicio.Visibility =
                Visibility.Collapsed;


            panelEmergencias.Visibility =
                Visibility.Collapsed;


            panelInternamientos.Visibility =
                Visibility.Collapsed;


            panelDetalle.Visibility =
                Visibility.Collapsed;


            panelHistorial.Visibility =
                Visibility.Collapsed;


            panelPendienteIntegracion.Visibility =
                Visibility.Collapsed;
        }


        // ============================================================
        // INICIO
        // ============================================================

        private void MostrarInicio()
        {
            OcultarPaneles();

            panelInicio.Visibility =
                Visibility.Visible;
        }


        // ============================================================
        // EMERGENCIA
        // ============================================================

        private void btnEmergencia_Click(
            object sender,
            RoutedEventArgs e)
        {
            MostrarEmergencias();
        }


        private void MostrarEmergencias()
        {
            OcultarPaneles();


            txtBuscarEmergencia.Clear();


            dgEmergencias.ItemsSource =
                emergencias;


            panelEmergencias.Visibility =
                Visibility.Visible;
        }


        // ============================================================
        // INTERNAMIENTO
        // ============================================================

        private void btnInternamiento_Click(
            object sender,
            RoutedEventArgs e)
        {
            MostrarInternamientos();
        }


        private void MostrarInternamientos()
        {
            OcultarPaneles();


            txtBuscarInternamiento.Clear();


            dgInternamientosFacturacion.ItemsSource =
                internamientos;


            panelInternamientos.Visibility =
                Visibility.Visible;
        }


        // ============================================================
        // AMBULATORIO
        // ============================================================

        private void btnAmbulatorio_Click(
            object sender,
            RoutedEventArgs e)
        {
            OcultarPaneles();


            txtIconoPendiente.Text =
                "⚕";


            txtTituloPendiente.Text =
                "FACTURANDO: AMBULATORIO";


            txtDescripcionPendiente.Text =
                "Aquí integraremos Consulta, Laboratorio, Imágenes y otros servicios ambulatorios.";


            panelPendienteIntegracion.Visibility =
                Visibility.Visible;
        }


        // ============================================================
        // BUSCAR EMERGENCIA
        // ============================================================

        private void txtBuscarEmergencia_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (dgEmergencias == null)
                return;


            string texto =
                txtBuscarEmergencia.Text
                    .Trim()
                    .ToLower();


            if (string.IsNullOrWhiteSpace(
                texto))
            {
                dgEmergencias.ItemsSource =
                    emergencias;

                return;
            }


            dgEmergencias.ItemsSource =
                emergencias
                    .Where(
                        x =>
                            x.CodigoOrigen
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.NombrePaciente
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.Documento
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.Seguro
                                .ToLower()
                                .Contains(texto))
                    .ToList();
        }


        // ============================================================
        // BUSCAR INTERNAMIENTO
        // ============================================================

        private void txtBuscarInternamiento_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (dgInternamientosFacturacion == null)
                return;


            string texto =
                txtBuscarInternamiento.Text
                    .Trim()
                    .ToLower();


            if (string.IsNullOrWhiteSpace(
                texto))
            {
                dgInternamientosFacturacion.ItemsSource =
                    internamientos;

                return;
            }


            dgInternamientosFacturacion.ItemsSource =
                internamientos
                    .Where(
                        x =>
                            x.CodigoOrigen
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.NombrePaciente
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.Documento
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.Seguro
                                .ToLower()
                                .Contains(texto))
                    .ToList();
        }


        // ============================================================
        // PREPARAR EMERGENCIA
        // ============================================================

        private void btnPrepararEmergencia_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;


            if (boton.DataContext
                is not EpisodioFacturable episodio)
            {
                return;
            }


            try
            {
                List<DetalleFacturable> detalle =
                    facturacionDAO
                        .ObtenerDetalleEmergencia(
                            episodio.IdOrigen);


                PrepararDetalle(
                    episodio,
                    detalle,
                    "Emergencia");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible preparar la factura.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // PREPARAR INTERNAMIENTO
        // ============================================================

        private void btnPrepararInternamiento_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;


            if (boton.DataContext
                is not EpisodioFacturable episodio)
            {
                return;
            }


            try
            {
                List<DetalleFacturable> detalle =
                    facturacionDAO
                        .ObtenerDetalleInternamiento(
                            episodio.IdOrigen);


                PrepararDetalle(
                    episodio,
                    detalle,
                    "Internamiento");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible preparar la factura de internamiento.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // PREPARAR DETALLE GENÉRICO
        // ============================================================

        private void PrepararDetalle(
            EpisodioFacturable episodio,
            List<DetalleFacturable> detalle,
            string tipo)
        {
            if (detalle.Count == 0)
            {
                MessageBox.Show(
                    "El episodio seleccionado no tiene cargos facturables.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }


            episodioSeleccionado =
                episodio;


            detalleActual =
                detalle;


            tipoFacturaActual =
                tipo;


            // ========================================================
            // DISTINTIVO
            // ========================================================

            if (tipo.Equals(
                "Emergencia",
                StringComparison.OrdinalIgnoreCase))
            {
                txtIconoDetalle.Text =
                    "🚑";


                txtTipoDetalle.Text =
                    "FACTURANDO: EMERGENCIA";


                bannerDetalle.Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            255,
                            241,
                            242));


                bannerDetalle.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(
                            254,
                            205,
                            211));


                txtTipoDetalle.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            185,
                            28,
                            28));


                txtOrigenDetalle.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            127,
                            29,
                            29));
            }
            else
            {
                txtIconoDetalle.Text =
                    "🛏";


                txtTipoDetalle.Text =
                    "FACTURANDO: INTERNAMIENTO";


                bannerDetalle.Background =
                    new SolidColorBrush(
                        Color.FromRgb(
                            239,
                            246,
                            255));


                bannerDetalle.BorderBrush =
                    new SolidColorBrush(
                        Color.FromRgb(
                            191,
                            219,
                            254));


                txtTipoDetalle.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            29,
                            78,
                            216));


                txtOrigenDetalle.Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            30,
                            64,
                            175));
            }


            // ========================================================
            // DATOS
            // ========================================================

            txtOrigenDetalle.Text =
                episodio.CodigoOrigen;


            txtPacienteDetalle.Text =
                episodio.NombrePaciente;


            txtDocumentoDetalle.Text =
                episodio.Documento;


            txtSeguroDetalle.Text =
                episodio.Seguro;


            txtPlanDetalle.Text =

                string.IsNullOrWhiteSpace(
                    episodio.PlanTarifario)

                    ? "No configurado"

                    : episodio.PlanTarifario;


            // ========================================================
            // DETALLE
            // ========================================================

            dgDetalle.ItemsSource =
                null;


            dgDetalle.ItemsSource =
                detalleActual;


            ActualizarTotalesDetalle();


            // ========================================================
            // AUTORIZACIÓN
            // ========================================================

            bool requiereAutorizacion =
                detalleActual.Any(
                    x =>
                        x.RequiereAutorizacion);


            panelAutorizacion.Visibility =

                requiereAutorizacion

                    ? Visibility.Visible

                    : Visibility.Collapsed;


            txtAutorizacion.Clear();


            // ========================================================
            // MOSTRAR
            // ========================================================

            OcultarPaneles();


            panelDetalle.Visibility =
                Visibility.Visible;
        }


        // ============================================================
        // TOTALES
        // ============================================================

        private void ActualizarTotalesDetalle()
        {
            decimal totalParticular =
                detalleActual.Sum(
                    x => x.TotalParticular);


            decimal totalTarifa =
                detalleActual.Sum(
                    x => x.TotalTarifa);


            decimal totalARS =
                detalleActual.Sum(
                    x => x.MontoSeguro);


            decimal totalPaciente =
                detalleActual.Sum(
                    x => x.MontoPaciente);


            txtTotalParticularDetalle.Text =
                $"RD$ {totalParticular:N2}";


            txtTotalTarifaDetalle.Text =
                $"RD$ {totalTarifa:N2}";


            txtTotalARSDetalle.Text =
                $"RD$ {totalARS:N2}";


            txtTotalPacienteDetalle.Text =
                $"RD$ {totalPaciente:N2}";
        }


        // ============================================================
        // GENERAR FACTURA
        // ============================================================

        private void btnGenerarFactura_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (episodioSeleccionado == null)
            {
                MessageBox.Show(
                    "No hay un episodio seleccionado.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            if (detalleActual.Count == 0)
            {
                MessageBox.Show(
                    "No existen cargos para facturar.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // ========================================================
            // PRECIO PARTICULAR
            // ========================================================

            DetalleFacturable? sinPrecio =
                detalleActual
                    .FirstOrDefault(
                        x =>
                            !x.PrecioParticularConfigurado);


            if (sinPrecio != null)
            {
                MessageBox.Show(
                    $"No existe precio particular configurado para:\n\n{sinPrecio.Descripcion}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }


            // ========================================================
            // AUTORIZACIÓN
            // ========================================================

            bool requiereAutorizacion =
                detalleActual.Any(
                    x =>
                        x.RequiereAutorizacion);


            string autorizacion =
                txtAutorizacion.Text
                    .Trim();


            if (requiereAutorizacion
                &&
                string.IsNullOrWhiteSpace(
                    autorizacion))
            {
                MessageBox.Show(
                    "Uno o más cargos requieren autorización del seguro.\n\nIngrese el número de autorización.",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);


                txtAutorizacion.Focus();

                return;
            }


            // ========================================================
            // TOTALES
            // ========================================================

            decimal particular =
                detalleActual.Sum(
                    x => x.TotalParticular);


            decimal tarifa =
                detalleActual.Sum(
                    x => x.TotalTarifa);


            decimal ars =
                detalleActual.Sum(
                    x => x.MontoSeguro);


            decimal paciente =
                detalleActual.Sum(
                    x => x.MontoPaciente);


            MessageBoxResult respuesta =
                MessageBox.Show(
                    $"¿Desea generar la factura de {episodioSeleccionado.CodigoOrigen}?\n\n" +
                    $"Precio sin seguro: RD$ {particular:N2}\n" +
                    $"Tarifa aplicada: RD$ {tarifa:N2}\n" +
                    $"Cobertura ARS: RD$ {ars:N2}\n" +
                    $"Paciente paga: RD$ {paciente:N2}",
                    "Generar factura",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);


            if (respuesta !=
                MessageBoxResult.Yes)
            {
                return;
            }


            try
            {
                long idFactura;


                if (tipoFacturaActual.Equals(
                    "Internamiento",
                    StringComparison.OrdinalIgnoreCase))
                {
                    idFactura =
                        facturacionDAO
                            .GenerarFacturaInternamiento(
                                episodioSeleccionado,
                                detalleActual,
                                autorizacion);
                }
                else
                {
                    idFactura =
                        facturacionDAO
                            .GenerarFacturaEmergencia(
                                episodioSeleccionado,
                                detalleActual,
                                autorizacion);
                }


                MessageBox.Show(
                    $"Factura generada correctamente.\n\n" +
                    $"Id de factura: {idFactura}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);


                string tipoGenerado =
                    tipoFacturaActual;


                episodioSeleccionado =
                    null;


                detalleActual =
                    new List<DetalleFacturable>();


                tipoFacturaActual =
                    "";


                CargarDatos();


                if (tipoGenerado.Equals(
                    "Internamiento",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MostrarInternamientos();
                }
                else
                {
                    MostrarEmergencias();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible generar la factura.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // VOLVER DEL DETALLE
        // ============================================================

        private void btnVolverDetalle_Click(
            object sender,
            RoutedEventArgs e)
        {
            string tipoAnterior =
                tipoFacturaActual;


            episodioSeleccionado =
                null;


            detalleActual =
                new List<DetalleFacturable>();


            tipoFacturaActual =
                "";


            if (tipoAnterior.Equals(
                "Internamiento",
                StringComparison.OrdinalIgnoreCase))
            {
                MostrarInternamientos();
            }
            else
            {
                MostrarEmergencias();
            }
        }


        // ============================================================
        // HISTORIAL
        // ============================================================

        private void btnHistorial_Click(
            object sender,
            RoutedEventArgs e)
        {
            OcultarPaneles();


            txtBuscarFactura.Clear();


            dgHistorial.ItemsSource =
                historial;


            panelHistorial.Visibility =
                Visibility.Visible;
        }


        private void txtBuscarFactura_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (dgHistorial == null)
                return;


            string texto =
                txtBuscarFactura.Text
                    .Trim()
                    .ToLower();


            if (string.IsNullOrWhiteSpace(
                texto))
            {
                dgHistorial.ItemsSource =
                    historial;

                return;
            }


            dgHistorial.ItemsSource =
                historial
                    .Where(
                        x =>
                            x.CodigoFactura
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.NombrePaciente
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.CodigoPaciente
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.CodigoOrigen
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.TipoFactura
                                .ToLower()
                                .Contains(texto)

                            ||

                            x.Seguro
                                .ToLower()
                                .Contains(texto))
                    .ToList();
        }


        // ============================================================
        // VOLVER A INICIO
        // ============================================================

        private void btnVolverInicio_Click(
            object sender,
            RoutedEventArgs e)
        {
            MostrarInicio();
        }


        // ============================================================
        // ACTUALIZAR
        // ============================================================

        private void btnActualizar_Click(
            object sender,
            RoutedEventArgs e)
        {
            CargarDatos();
        }
    }
}