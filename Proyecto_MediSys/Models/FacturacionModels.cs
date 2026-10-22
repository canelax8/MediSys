using System;
using System.Collections.Generic;

namespace Proyecto_MediSys.Models
{
    // ============================================================
    // EPISODIO DISPONIBLE PARA FACTURAR
    // ============================================================

    public class EpisodioFacturable
    {
        public long IdOrigen { get; set; }

        public string CodigoOrigen { get; set; } = "";

        public string TipoFactura { get; set; } = "";

        public string SubtipoAmbulatorio { get; set; } = "";


        // ========================================================
        // PACIENTE
        // ========================================================

        public int IdPaciente { get; set; }

        public string CodigoPaciente { get; set; } = "";

        public string NombrePaciente { get; set; } = "";

        public string Documento { get; set; } = "";


        // ========================================================
        // SEGURO
        // ========================================================

        public int? IdSeguro { get; set; }

        public string Seguro { get; set; } = "";

        public int? IdPlanTarifario { get; set; }

        public string PlanTarifario { get; set; } = "";

        public string TipoPlan { get; set; } = "";


        // ========================================================
        // EPISODIO
        // ========================================================

        public DateTime Fecha { get; set; }

        public string Motivo { get; set; } = "";

        public string Medico { get; set; } = "";

        public string Especialidad { get; set; } = "";


        // ========================================================
        // TOTALES
        // ========================================================

        public int CantidadItems { get; set; }

        public decimal TotalParticular { get; set; }

        public decimal TotalTarifa { get; set; }

        public decimal TotalSeguro { get; set; }

        public decimal TotalPaciente { get; set; }


        // ========================================================
        // VALIDACIÓN
        // ========================================================

        public bool TieneProblemasCobertura { get; set; }


        // ========================================================
        // PROPIEDADES VISUALES
        // ========================================================

        public string FechaMostrar =>
            Fecha.ToString("dd/MM/yyyy hh:mm tt");


        public string TotalParticularTexto =>
            $"RD$ {TotalParticular:N2}";


        public string TotalTarifaTexto =>
            $"RD$ {TotalTarifa:N2}";


        public string TotalSeguroTexto =>
            $"RD$ {TotalSeguro:N2}";


        public string TotalPacienteTexto =>
            $"RD$ {TotalPaciente:N2}";


        public string EstadoCobertura =>
            TieneProblemasCobertura
                ? "Revisar cobertura"
                : "Listo para facturar";
    }


    // ============================================================
    // DETALLE DE UN SERVICIO A FACTURAR
    // ============================================================

    public class DetalleFacturable
    {
        public bool Seleccionado { get; set; } = true;


        // ========================================================
        // ORIGEN
        // ========================================================

        public long? IdDetalleOrigen { get; set; }

        public long? IdItemClinico { get; set; }


        // ========================================================
        // SERVICIO
        // ========================================================

        public string CodigoItem { get; set; } = "";

        public string TipoItem { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public decimal Cantidad { get; set; }


        // ========================================================
        // PRECIO PARTICULAR
        // ========================================================

        public decimal PrecioParticularUnitario { get; set; }

        public decimal TotalParticular { get; set; }

        public bool PrecioParticularConfigurado { get; set; }


        // ========================================================
        // TARIFA DEL PLAN
        // ========================================================

        public decimal PrecioTarifaUnitario { get; set; }

        public decimal TotalTarifa { get; set; }

        public int? IdPlanTarifario { get; set; }

        public string PlanTarifario { get; set; } = "";

        public string TipoPlan { get; set; } = "";


        // ========================================================
        // COBERTURA
        // ========================================================

        public bool CoberturaConfigurada { get; set; }

        public decimal PorcentajeCobertura { get; set; }

        public decimal CopagoFijo { get; set; }

        public decimal? MontoMaximoCobertura { get; set; }

        public decimal MontoSeguro { get; set; }

        public decimal MontoPaciente { get; set; }


        // ========================================================
        // AUTORIZACIÓN
        // ========================================================

        public bool RequiereAutorizacion { get; set; }

        public string NumeroAutorizacion { get; set; } = "";


        // ========================================================
        // ESTADO
        // ========================================================

        public string EstadoCobertura { get; set; } = "";


        // ========================================================
        // FECHA DEL CARGO ORIGINAL
        // ========================================================

        public DateTime FechaServicio { get; set; }


        // ========================================================
        // PROPIEDADES VISUALES
        // ========================================================

        public string CantidadTexto =>
            Cantidad.ToString("N2");


        public string PrecioParticularTexto =>
            PrecioParticularConfigurado
                ? $"RD$ {PrecioParticularUnitario:N2}"
                : "No configurado";


        public string TotalParticularTexto =>
            PrecioParticularConfigurado
                ? $"RD$ {TotalParticular:N2}"
                : "—";


        public string PrecioTarifaTexto =>
            $"RD$ {PrecioTarifaUnitario:N2}";


        public string TotalTarifaTexto =>
            $"RD$ {TotalTarifa:N2}";


        public string CoberturaTexto =>
            $"{PorcentajeCobertura:N0}%";


        public string MontoSeguroTexto =>
            $"RD$ {MontoSeguro:N2}";


        public string MontoPacienteTexto =>
            $"RD$ {MontoPaciente:N2}";
    }


    // ============================================================
    // COBERTURA DE PLAN
    // ============================================================

    public class CoberturaPlan
    {
        public long IdCoberturaPlan { get; set; }

        public int IdPlanTarifario { get; set; }

        public long IdItemClinico { get; set; }

        public decimal PorcentajeCobertura { get; set; }

        public decimal CopagoFijo { get; set; }

        public decimal? MontoMaximoCobertura { get; set; }

        public bool RequiereAutorizacion { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public bool Activo { get; set; }
    }


    // ============================================================
    // CARGO DEL PACIENTE
    // ============================================================

    public class CargoPaciente
    {
        public long IdCargo { get; set; }

        public int IdPaciente { get; set; }


        public string TipoOrigen { get; set; } = "";

        public string SubtipoAmbulatorio { get; set; } = "";

        public long? IdOrigen { get; set; }

        public string CodigoOrigen { get; set; } = "";

        public long? IdDetalleOrigen { get; set; }


        public long? IdItemClinico { get; set; }

        public string Descripcion { get; set; } = "";

        public decimal Cantidad { get; set; }


        public decimal PrecioParticularUnitario { get; set; }

        public decimal TotalParticular { get; set; }


        public decimal PrecioTarifaUnitario { get; set; }

        public decimal TotalTarifa { get; set; }


        public decimal PorcentajeCobertura { get; set; }

        public decimal CopagoFijoAplicado { get; set; }

        public decimal MontoSeguro { get; set; }

        public decimal MontoPaciente { get; set; }


        public int? IdSeguro { get; set; }

        public int? IdPlanTarifario { get; set; }


        public bool RequiereAutorizacion { get; set; }

        public string NumeroAutorizacion { get; set; } = "";


        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaCargo { get; set; }

        public bool Activo { get; set; }
    }


    // ============================================================
    // FACTURA
    // ============================================================

    public class Factura
    {
        public long IdFactura { get; set; }

        public string CodigoFactura { get; set; } = "";

        public int IdPaciente { get; set; }

        public string NombrePaciente { get; set; } = "";


        public string TipoFactura { get; set; } = "";

        public string SubtipoAmbulatorio { get; set; } = "";

        public long? IdOrigen { get; set; }

        public string CodigoOrigen { get; set; } = "";


        public int? IdSeguro { get; set; }

        public string Seguro { get; set; } = "";

        public int? IdPlanTarifario { get; set; }

        public string PlanTarifario { get; set; } = "";

        public string NumeroAutorizacion { get; set; } = "";


        public decimal TotalPrecioParticular { get; set; }

        public decimal TotalTarifaAplicada { get; set; }

        public decimal TotalCoberturaSeguro { get; set; }

        public decimal TotalPaciente { get; set; }


        public int IdEstadoFactura { get; set; }

        public string Estado { get; set; } = "";


        public DateTime FechaFactura { get; set; }

        public string Observaciones { get; set; } = "";

        public bool Activo { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }


        public List<FacturaDetalle> Detalles { get; set; } =
            new List<FacturaDetalle>();


        public string FechaMostrar =>
            FechaFactura.ToString("dd/MM/yyyy hh:mm tt");


        public string TotalTexto =>
            $"RD$ {TotalTarifaAplicada:N2}";


        public string SeguroTexto =>
            $"RD$ {TotalCoberturaSeguro:N2}";


        public string PacienteTexto =>
            $"RD$ {TotalPaciente:N2}";
    }


    // ============================================================
    // DETALLE DE FACTURA
    // ============================================================

    public class FacturaDetalle
    {
        public long IdFacturaDetalle { get; set; }

        public long IdFactura { get; set; }

        public long? IdCargo { get; set; }

        public long? IdItemClinico { get; set; }


        public string TipoItem { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public decimal Cantidad { get; set; }


        public decimal PrecioParticularUnitario { get; set; }

        public decimal TotalParticular { get; set; }


        public decimal PrecioTarifaUnitario { get; set; }

        public decimal TotalTarifa { get; set; }


        public decimal PorcentajeCobertura { get; set; }

        public decimal CopagoFijoAplicado { get; set; }

        public decimal MontoSeguro { get; set; }

        public decimal MontoPaciente { get; set; }


        public bool RequiereAutorizacion { get; set; }

        public string NumeroAutorizacion { get; set; } = "";
    }


    // ============================================================
    // ESTADO DE FACTURA
    // ============================================================

    public class EstadoFactura
    {
        public int IdEstadoFactura { get; set; }

        public string Nombre { get; set; } = "";

        public bool Activo { get; set; }


        public override string ToString()
        {
            return Nombre;
        }
    }


    // ============================================================
    // PAGO
    // ============================================================

    public class PagoFactura
    {
        public long IdPago { get; set; }

        public long IdFactura { get; set; }

        public string TipoPagador { get; set; } = "";

        public decimal Monto { get; set; }

        public string MetodoPago { get; set; } = "";

        public string Referencia { get; set; } = "";

        public string Observaciones { get; set; } = "";

        public string Estado { get; set; } = "";

        public DateTime FechaPago { get; set; }

        public bool Activo { get; set; }
    }


    // ============================================================
    // HISTORIAL / RESUMEN DE FACTURAS
    // ============================================================

    public class FacturaResumen
    {
        public long IdFactura { get; set; }

        public string CodigoFactura { get; set; } = "";

        public int IdPaciente { get; set; }

        public string NombrePaciente { get; set; } = "";

        public string CodigoPaciente { get; set; } = "";


        public string TipoFactura { get; set; } = "";

        public string SubtipoAmbulatorio { get; set; } = "";

        public string CodigoOrigen { get; set; } = "";


        public string Seguro { get; set; } = "";

        public string PlanTarifario { get; set; } = "";


        public decimal TotalParticular { get; set; }

        public decimal TotalTarifa { get; set; }

        public decimal TotalSeguro { get; set; }

        public decimal TotalPaciente { get; set; }


        public decimal PagadoPaciente { get; set; }

        public decimal PagadoSeguro { get; set; }


        public decimal PendientePaciente =>
            Math.Max(
                0,
                TotalPaciente - PagadoPaciente);


        public decimal PendienteSeguro =>
            Math.Max(
                0,
                TotalSeguro - PagadoSeguro);


        public string Estado { get; set; } = "";

        public DateTime FechaFactura { get; set; }


        public string FechaMostrar =>
            FechaFactura.ToString("dd/MM/yyyy hh:mm tt");


        public string TotalTexto =>
            $"RD$ {TotalTarifa:N2}";


        public string PacientePagaTexto =>
            $"RD$ {TotalPaciente:N2}";


        public string SeguroPagaTexto =>
            $"RD$ {TotalSeguro:N2}";
    }
}