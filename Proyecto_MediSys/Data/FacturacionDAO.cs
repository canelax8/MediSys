using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Proyecto_MediSys.Data
{
    public class FacturacionDAO
    {
        private readonly Conexion conexion =
            new Conexion();


        // ============================================================
        // EMERGENCIAS PENDIENTES DE FACTURAR
        // ============================================================

        public List<EpisodioFacturable>
            ObtenerEmergenciasPendientes()
        {
            List<EpisodioFacturable> lista =
                new List<EpisodioFacturable>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT

                    E.IdEmergencia,
                    E.CodigoEmergencia,
                    E.IdPaciente,
                    E.FechaIngreso,
                    E.MotivoConsulta,

                    P.CodigoPaciente,

                    LTRIM(RTRIM(CONCAT(
                        P.Nombre, ' ',
                        P.SegundoNombre, ' ',
                        P.Apellido, ' ',
                        P.SegundoApellido
                    ))) AS Paciente,

                    CASE
                        WHEN P.Indocumentado = 1
                            THEN ISNULL(P.CodigoTemporal, '')
                        ELSE
                            ISNULL(P.NumeroDocumento, '')
                    END AS Documento,

                    P.IdSeguro,

                    ISNULL(S.Nombre, 'Sin seguro')
                        AS Seguro,

                    LTRIM(RTRIM(CONCAT(
                        M.Nombre, ' ',
                        M.Apellido
                    ))) AS Medico,

                    ES.Nombre AS Especialidad,


                    COALESCE(
                        PlanItem.IdPlanTarifario,
                        PlanPaciente.IdPlanTarifario
                    ) AS IdPlanTarifario,


                    COALESCE(
                        PlanItem.PlanTarifario,
                        PlanPaciente.PlanTarifario,
                        ''
                    ) AS PlanTarifario,


                    COALESCE(
                        PlanItem.TipoPlan,
                        PlanPaciente.TipoPlan,
                        ''
                    ) AS TipoPlan


                FROM tbEmergencias E


                INNER JOIN tbPacientes P
                    ON E.IdPaciente =
                       P.IdPaciente


                LEFT JOIN tbSeguros S
                    ON P.IdSeguro =
                       S.IdSeguro


                INNER JOIN tbMedicos M
                    ON E.IdMedico =
                       M.IdMedico


                INNER JOIN tbEspecialidades ES
                    ON E.IdEspecialidad =
                       ES.IdEspecialidad


                /* =================================================
                   PLAN UTILIZADO EN LOS ITEMS DE EMERGENCIA
                ================================================= */

                OUTER APPLY
                (
                    SELECT TOP 1

                        EI.IdPlanTarifarioAplicado
                            AS IdPlanTarifario,

                        PT.Nombre
                            AS PlanTarifario,

                        PT.Tipo
                            AS TipoPlan

                    FROM tbEmergenciaItems EI

                    LEFT JOIN tbPlanesTarifarios PT
                        ON EI.IdPlanTarifarioAplicado =
                           PT.IdPlanTarifario

                    WHERE
                        EI.IdEmergencia =
                            E.IdEmergencia

                        AND EI.Activo = 1

                        AND EI.IdPlanTarifarioAplicado
                            IS NOT NULL

                    ORDER BY
                        EI.FechaRegistro DESC,
                        EI.IdEmergenciaItem DESC

                ) PlanItem


                /* =================================================
                   PLAN ACTUAL DEL PACIENTE COMO RESPALDO
                ================================================= */

                OUTER APPLY
                (
                    SELECT TOP 1

                        PT2.IdPlanTarifario,

                        PT2.Nombre
                            AS PlanTarifario,

                        PT2.Tipo
                            AS TipoPlan

                    FROM tbPlanesTarifarios PT2

                    WHERE
                        PT2.IdSeguro =
                            P.IdSeguro

                        AND PT2.Activo = 1

                    ORDER BY
                        PT2.IdPlanTarifario

                ) PlanPaciente


                /* =================================================
                   SOLO EMERGENCIAS FINALIZADAS
                ================================================= */

                WHERE EXISTS
                (
                    SELECT 1

                    FROM tbDestinoEmergencia DE

                    WHERE
                        DE.IdEmergencia =
                            E.IdEmergencia
                )


                /* =================================================
                   EVITAR FACTURAR DOS VECES
                ================================================= */

                AND NOT EXISTS
                (
                    SELECT 1

                    FROM tbFacturas F

                    INNER JOIN tbEstadosFactura EF
                        ON F.IdEstadoFactura =
                           EF.IdEstadoFactura

                    WHERE
                        F.TipoFactura =
                            'Emergencia'

                        AND F.IdOrigen =
                            E.IdEmergencia

                        AND F.Activo = 1

                        AND EF.Nombre <>
                            'Anulada'
                )


                ORDER BY
                    E.FechaIngreso DESC,
                    E.IdEmergencia DESC;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                EpisodioFacturable episodio =
                    new EpisodioFacturable
                    {
                        IdOrigen =
                            Convert.ToInt64(
                                reader["IdEmergencia"]),

                        CodigoOrigen =
                            reader["CodigoEmergencia"]
                                .ToString() ?? "",

                        TipoFactura =
                            "Emergencia",

                        IdPaciente =
                            Convert.ToInt32(
                                reader["IdPaciente"]),

                        CodigoPaciente =
                            reader["CodigoPaciente"]
                                .ToString() ?? "",

                        NombrePaciente =
                            reader["Paciente"]
                                .ToString() ?? "",

                        Documento =
                            reader["Documento"]
                                .ToString() ?? "",

                        IdSeguro =
                            reader["IdSeguro"] ==
                            DBNull.Value

                                ? null

                                : Convert.ToInt32(
                                    reader["IdSeguro"]),

                        Seguro =
                            reader["Seguro"]
                                .ToString() ?? "",

                        IdPlanTarifario =
                            reader["IdPlanTarifario"] ==
                            DBNull.Value

                                ? null

                                : Convert.ToInt32(
                                    reader["IdPlanTarifario"]),

                        PlanTarifario =
                            reader["PlanTarifario"]
                                .ToString() ?? "",

                        TipoPlan =
                            reader["TipoPlan"]
                                .ToString() ?? "",

                        Fecha =
                            Convert.ToDateTime(
                                reader["FechaIngreso"]),

                        Motivo =
                            reader["MotivoConsulta"]
                                .ToString() ?? "",

                        Medico =
                            reader["Medico"]
                                .ToString() ?? "",

                        Especialidad =
                            reader["Especialidad"]
                                .ToString() ?? ""
                    };


                lista.Add(
                    episodio);
            }


            reader.Close();


            // ========================================================
            // CALCULAR TOTALES DE CADA EMERGENCIA
            // ========================================================

            foreach (EpisodioFacturable episodio
                     in lista)
            {
                List<DetalleFacturable> detalle =
                    ObtenerDetalleEmergencia(
                        episodio.IdOrigen);


                episodio.CantidadItems =
                    detalle.Count;


                episodio.TotalParticular =
                    detalle.Sum(
                        x => x.TotalParticular);


                episodio.TotalTarifa =
                    detalle.Sum(
                        x => x.TotalTarifa);


                episodio.TotalSeguro =
                    detalle.Sum(
                        x => x.MontoSeguro);


                episodio.TotalPaciente =
                    detalle.Sum(
                        x => x.MontoPaciente);


                episodio.TieneProblemasCobertura =
                    detalle.Any(
                            x =>
                                  !x.PrecioParticularConfigurado

                                   ||

                                   (
                                         x.RequiereAutorizacion
                                         &&
                                         string.IsNullOrWhiteSpace(
                                             x.NumeroAutorizacion)
                                     ));
            }


            return lista;
        }


        // ============================================================
        // DETALLE DE UNA EMERGENCIA
        // ============================================================

        public List<DetalleFacturable>
            ObtenerDetalleEmergencia(
                long idEmergencia)
        {
            List<DetalleFacturable> lista =
                new List<DetalleFacturable>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT

                    EI.IdEmergenciaItem,
                    EI.IdItemClinico,

                    I.Codigo,
                    I.Nombre,

                    TI.Nombre
                        AS TipoItem,

                    EI.Cantidad,

                    EI.PrecioUnitarioAplicado,

                    EI.IdPlanTarifarioAplicado,

                    ISNULL(
                        PA.Nombre,
                        ''
                    ) AS PlanTarifario,

                    ISNULL(
                        PA.Tipo,
                        ''
                    ) AS TipoPlan,

                    EI.FechaRegistro,


                    /* =============================================
                       PRECIO PARTICULAR
                    ============================================== */

                    TP.Precio
                        AS PrecioParticular,


                    CASE
                        WHEN TP.Precio IS NULL
                            THEN CAST(0 AS BIT)
                        ELSE
                            CAST(1 AS BIT)
                    END AS PrecioParticularConfigurado,


                    /* =============================================
                       COBERTURA
                    ============================================== */

                    CP.IdCoberturaPlan,

                    CP.PorcentajeCobertura,

                    CP.CopagoFijo,

                    CP.MontoMaximoCobertura,

                    CP.RequiereAutorizacion


                FROM tbEmergenciaItems EI


                INNER JOIN tbItemsClinicos I
                    ON EI.IdItemClinico =
                       I.IdItemClinico


                INNER JOIN tbTiposItemClinico TI
                    ON I.IdTipoItem =
                       TI.IdTipoItem


                LEFT JOIN tbPlanesTarifarios PA
                    ON EI.IdPlanTarifarioAplicado =
                       PA.IdPlanTarifario


                /* ===============================================
                   BUSCAR PRECIO PARTICULAR VIGENTE EN LA FECHA
                   EN QUE SE PRESTÓ EL SERVICIO
                =============================================== */

                OUTER APPLY
                (
                    SELECT TOP 1

                        T.Precio

                    FROM tbTarifasItems T

                    INNER JOIN tbPlanesTarifarios PP
                        ON T.IdPlanTarifario =
                           PP.IdPlanTarifario

                    WHERE
                        T.IdItemClinico =
                            EI.IdItemClinico

                        AND T.Activo = 1

                        AND PP.Activo = 1

                        AND
                        (
                            (
       LOWER(ISNULL(PP.Tipo, '')) LIKE '%particular%'
    OR LOWER(ISNULL(PP.Nombre, '')) LIKE '%particular%'

    OR LOWER(ISNULL(PP.Tipo, '')) LIKE '%privado%'
    OR LOWER(ISNULL(PP.Nombre, '')) LIKE '%privado%'

    OR LOWER(ISNULL(PP.Tipo, '')) LIKE '%sin seguro%'
    OR LOWER(ISNULL(PP.Nombre, '')) LIKE '%sin seguro%'
)
                        )

                        AND T.FechaInicio <=
                            CAST(
                                EI.FechaRegistro
                                AS DATE
                            )

                        AND
                        (
                            T.FechaFin IS NULL

                            OR

                            T.FechaFin >=
                                CAST(
                                    EI.FechaRegistro
                                    AS DATE
                                )
                        )

                    ORDER BY
                        T.FechaInicio DESC,
                        T.IdTarifa DESC

                ) TP


                /* ===============================================
                   COBERTURA DEL PLAN
                =============================================== */

                OUTER APPLY
                (
                    SELECT TOP 1

                        C.IdCoberturaPlan,

                        C.PorcentajeCobertura,

                        C.CopagoFijo,

                        C.MontoMaximoCobertura,

                        C.RequiereAutorizacion

                    FROM tbCoberturasPlan C

                    WHERE
                        C.IdPlanTarifario =
                            EI.IdPlanTarifarioAplicado

                        AND C.IdItemClinico =
                            EI.IdItemClinico

                        AND C.Activo = 1

                        AND C.FechaInicio <=
                            CAST(
                                EI.FechaRegistro
                                AS DATE
                            )

                        AND
                        (
                            C.FechaFin IS NULL

                            OR

                            C.FechaFin >=
                                CAST(
                                    EI.FechaRegistro
                                    AS DATE
                                )
                        )

                    ORDER BY
                        C.FechaInicio DESC,
                        C.IdCoberturaPlan DESC

                ) CP


                WHERE

                    EI.IdEmergencia =
                        @IdEmergencia

                    AND EI.Activo = 1


                ORDER BY

                    TI.Nombre,
                    I.Nombre;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.Add(
                "@IdEmergencia",
                SqlDbType.BigInt)
                .Value =
                    idEmergencia;


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                decimal cantidad =
                    Convert.ToDecimal(
                        reader["Cantidad"]);


                decimal tarifaAplicada =
                    Convert.ToDecimal(
                        reader["PrecioUnitarioAplicado"]);


                bool particularConfigurado =
                    Convert.ToBoolean(
                        reader["PrecioParticularConfigurado"]);


                decimal precioParticular =
                    particularConfigurado

                        ? Convert.ToDecimal(
                            reader["PrecioParticular"])

                        : 0;


                DetalleFacturable detalle =
                    new DetalleFacturable
                    {
                        IdDetalleOrigen =
                            Convert.ToInt64(
                                reader["IdEmergenciaItem"]),

                        IdItemClinico =
                            Convert.ToInt64(
                                reader["IdItemClinico"]),

                        CodigoItem =
                            reader["Codigo"]
                                .ToString() ?? "",

                        TipoItem =
                            reader["TipoItem"]
                                .ToString() ?? "",

                        Descripcion =
                            reader["Nombre"]
                                .ToString() ?? "",

                        Cantidad =
                            cantidad,

                        PrecioParticularUnitario =
                            precioParticular,

                        PrecioParticularConfigurado =
                            particularConfigurado,

                        PrecioTarifaUnitario =
                            tarifaAplicada,

                        IdPlanTarifario =
                            reader["IdPlanTarifarioAplicado"] ==
                            DBNull.Value

                                ? null

                                : Convert.ToInt32(
                                    reader["IdPlanTarifarioAplicado"]),

                        PlanTarifario =
                            reader["PlanTarifario"]
                                .ToString() ?? "",

                        TipoPlan =
                            reader["TipoPlan"]
                                .ToString() ?? "",

                        CoberturaConfigurada =
                            reader["IdCoberturaPlan"] !=
                            DBNull.Value,

                        PorcentajeCobertura =
                            reader["PorcentajeCobertura"] ==
                            DBNull.Value

                                ? 0

                                : Convert.ToDecimal(
                                    reader["PorcentajeCobertura"]),

                        CopagoFijo =
                            reader["CopagoFijo"] ==
                            DBNull.Value

                                ? 0

                                : Convert.ToDecimal(
                                    reader["CopagoFijo"]),

                        MontoMaximoCobertura =
                            reader["MontoMaximoCobertura"] ==
                            DBNull.Value

                                ? null

                                : Convert.ToDecimal(
                                    reader["MontoMaximoCobertura"]),

                        RequiereAutorizacion =
                            reader["RequiereAutorizacion"] !=
                            DBNull.Value

                            &&
                            Convert.ToBoolean(
                                reader["RequiereAutorizacion"]),

                        FechaServicio =
                            Convert.ToDateTime(
                                reader["FechaRegistro"])
                    };


                // ====================================================
                // SI ES PARTICULAR Y NO ENCONTRÓ OTRA TARIFA
                // LA TARIFA APLICADA ES SU PRECIO PARTICULAR
                // ====================================================

                if (!detalle.PrecioParticularConfigurado
                    &&
                    EsPlanParticular(
                        detalle.TipoPlan,
                        detalle.PlanTarifario))
                {
                    detalle.PrecioParticularUnitario =
                        detalle.PrecioTarifaUnitario;


                    detalle.PrecioParticularConfigurado =
                        true;
                }


                // ====================================================
                // TOTALES
                // ====================================================

                detalle.TotalParticular =
                    detalle.Cantidad *
                    detalle.PrecioParticularUnitario;


                detalle.TotalTarifa =
                    detalle.Cantidad *
                    detalle.PrecioTarifaUnitario;


                // ====================================================
                // COBERTURA
                // ====================================================

                CalcularCobertura(
                    detalle);


                lista.Add(
                    detalle);
            }


            return lista;
        }


        // ============================================================
        // CALCULAR COBERTURA
        // ============================================================

        private void CalcularCobertura(
            DetalleFacturable detalle)
        {
            decimal total =
                detalle.TotalTarifa;


            // ========================================================
            // 1. PARTICULAR / PRIVADO / SIN SEGURO
            //
            // El paciente paga el 100%.
            // ========================================================

            if (EsPlanParticular(
                detalle.TipoPlan,
                detalle.PlanTarifario))
            {
                detalle.CoberturaConfigurada =
                    true;

                detalle.PorcentajeCobertura =
                    0;

                detalle.MontoSeguro =
                    0;

                detalle.MontoPaciente =
                    total;

                detalle.EstadoCobertura =
                    "Particular";

                return;
            }


            // ========================================================
            // 2. EL SERVICIO NO TIENE COBERTURA DE ARS
            //
            // IMPORTANTE:
            // NO se bloquea la factura.
            // La ARS paga RD$ 0.
            // El paciente paga el 100%.
            // ========================================================

            if (!detalle.CoberturaConfigurada)
            {
                detalle.PorcentajeCobertura =
                    0;

                detalle.CopagoFijo =
                    0;

                detalle.MontoSeguro =
                    0;

                detalle.MontoPaciente =
                    total;

                detalle.EstadoCobertura =
                    "No cubierto por ARS";

                return;
            }


            // ========================================================
            // 3. COBERTURA CONFIGURADA EN 0%
            //
            // También significa que no lo cubre la ARS.
            // ========================================================

            if (detalle.PorcentajeCobertura <= 0)
            {
                detalle.MontoSeguro =
                    0;

                detalle.MontoPaciente =
                    total;

                detalle.EstadoCobertura =
                    "No cubierto por ARS";

                return;
            }


            // ========================================================
            // 4. CALCULAR COBERTURA NORMAL
            // ========================================================

            decimal montoSeguro =
                total *
                (
                    detalle.PorcentajeCobertura
                    /
                    100m
                );


            // ========================================================
            // 5. APLICAR MONTO MÁXIMO DE COBERTURA
            // ========================================================

            if (detalle.MontoMaximoCobertura.HasValue
                &&
                detalle.MontoMaximoCobertura.Value > 0)
            {
                montoSeguro =
                    Math.Min(
                        montoSeguro,
                        detalle.MontoMaximoCobertura.Value);
            }


            // Nunca permitir que el seguro pague más
            // que el total de la tarifa.

            montoSeguro =
                Math.Min(
                    montoSeguro,
                    total);


            decimal montoPaciente =
                total -
                montoSeguro;


            // ========================================================
            // 6. COPAGO FIJO
            // ========================================================

            if (detalle.CopagoFijo >
                montoPaciente)
            {
                montoPaciente =
                    Math.Min(
                        detalle.CopagoFijo,
                        total);

                montoSeguro =
                    total -
                    montoPaciente;
            }


            // ========================================================
            // 7. ASIGNAR MONTOS
            // ========================================================

            detalle.MontoSeguro =
                Math.Max(
                    0,
                    montoSeguro);


            detalle.MontoPaciente =
                Math.Max(
                    0,
                    montoPaciente);


            // ========================================================
            // 8. ESTADO
            // ========================================================

            if (detalle.RequiereAutorizacion
                &&
                string.IsNullOrWhiteSpace(
                    detalle.NumeroAutorizacion))
            {
                detalle.EstadoCobertura =
                    "Requiere autorización";
            }
            else if (detalle.MontoSeguro <= 0)
            {
                detalle.EstadoCobertura =
                    "No cubierto por ARS";
            }
            else if (detalle.MontoPaciente <= 0.01m)
            {
                detalle.EstadoCobertura =
                    "Cubierto 100%";
            }
            else
            {
                detalle.EstadoCobertura =
                    "Cobertura parcial";
            }
        }


        // ============================================================
        // VERIFICAR SI EL PLAN ES PARTICULAR / PRIVADO
        // ============================================================

        private bool EsPlanParticular(
            string tipoPlan,
            string nombrePlan)
        {
            string tipo =
                tipoPlan
                    ?.Trim()
                    .ToLower()
                ?? "";


            string nombre =
                nombrePlan
                    ?.Trim()
                    .ToLower()
                ?? "";


            return

                // PARTICULAR
                tipo.Contains("particular")
                ||
                nombre.Contains("particular")

                ||

                // PRIVADO
                tipo.Contains("privado")
                ||
                nombre.Contains("privado")

                ||

                // SIN SEGURO
                tipo.Contains("sin seguro")
                ||
                nombre.Contains("sin seguro");
        }

        // ============================================================
        // HISTORIAL DE FACTURAS DE UN PACIENTE
        // ============================================================

        public List<FacturaResumen>
            ObtenerHistorialPaciente(
                int idPaciente)
        {
            return ObtenerFacturas(
                idPaciente,
                null);
        }


        // ============================================================
        // FACTURAS RECIENTES
        // ============================================================

        public List<FacturaResumen>
            ObtenerFacturasRecientes(
                int cantidad = 100)
        {
            return ObtenerFacturas(
                null,
                cantidad);
        }


        // ============================================================
        // OBTENER FACTURAS
        // ============================================================

        private List<FacturaResumen>
            ObtenerFacturas(
                int? idPaciente,
                int? limite)
        {
            List<FacturaResumen> lista =
                new List<FacturaResumen>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string top =
                limite.HasValue
                    ? $"TOP {limite.Value}"
                    : "";


            string sql =
                $@"
                SELECT {top}

                    F.IdFactura,
                    F.CodigoFactura,
                    F.IdPaciente,

                    P.CodigoPaciente,

                    LTRIM(RTRIM(CONCAT(
                        P.Nombre, ' ',
                        P.SegundoNombre, ' ',
                        P.Apellido, ' ',
                        P.SegundoApellido
                    ))) AS Paciente,

                    F.TipoFactura,

                    ISNULL(
                        F.SubtipoAmbulatorio,
                        ''
                    ) AS SubtipoAmbulatorio,

                    ISNULL(
                        F.CodigoOrigen,
                        ''
                    ) AS CodigoOrigen,

                    ISNULL(
                        S.Nombre,
                        'Sin seguro'
                    ) AS Seguro,

                    ISNULL(
                        PT.Nombre,
                        ''
                    ) AS PlanTarifario,

                    F.TotalPrecioParticular,

                    F.TotalTarifaAplicada,

                    F.TotalCoberturaSeguro,

                    F.TotalPaciente,

                    EF.Nombre
                        AS Estado,

                    F.FechaFactura,


                    ISNULL(
                        Pagos.PagadoPaciente,
                        0
                    ) AS PagadoPaciente,


                    ISNULL(
                        Pagos.PagadoSeguro,
                        0
                    ) AS PagadoSeguro


                FROM tbFacturas F


                INNER JOIN tbPacientes P
                    ON F.IdPaciente =
                       P.IdPaciente


                INNER JOIN tbEstadosFactura EF
                    ON F.IdEstadoFactura =
                       EF.IdEstadoFactura


                LEFT JOIN tbSeguros S
                    ON F.IdSeguro =
                       S.IdSeguro


                LEFT JOIN tbPlanesTarifarios PT
                    ON F.IdPlanTarifario =
                       PT.IdPlanTarifario


                OUTER APPLY
                (
                    SELECT

                        SUM
                        (
                            CASE
                                WHEN PF.TipoPagador =
                                     'Paciente'

                                     AND PF.Estado =
                                         'Aplicado'

                                     AND PF.Activo = 1

                                    THEN PF.Monto

                                ELSE 0
                            END
                        ) AS PagadoPaciente,


                        SUM
                        (
                            CASE
                                WHEN PF.TipoPagador =
                                     'ARS'

                                     AND PF.Estado =
                                         'Aplicado'

                                     AND PF.Activo = 1

                                    THEN PF.Monto

                                ELSE 0
                            END
                        ) AS PagadoSeguro


                    FROM tbPagosFactura PF

                    WHERE
                        PF.IdFactura =
                            F.IdFactura

                ) Pagos


                WHERE

                    F.Activo = 1

                    AND
                    (
                        @IdPaciente IS NULL

                        OR

                        F.IdPaciente =
                            @IdPaciente
                    )


                ORDER BY

                    F.FechaFactura DESC,
                    F.IdFactura DESC;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.Add(
                "@IdPaciente",
                SqlDbType.Int)
                .Value =
                    idPaciente.HasValue

                        ? idPaciente.Value

                        : DBNull.Value;


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                lista.Add(
                    new FacturaResumen
                    {
                        IdFactura =
                            Convert.ToInt64(
                                reader["IdFactura"]),

                        CodigoFactura =
                            reader["CodigoFactura"]
                                .ToString() ?? "",

                        IdPaciente =
                            Convert.ToInt32(
                                reader["IdPaciente"]),

                        CodigoPaciente =
                            reader["CodigoPaciente"]
                                .ToString() ?? "",

                        NombrePaciente =
                            reader["Paciente"]
                                .ToString() ?? "",

                        TipoFactura =
                            reader["TipoFactura"]
                                .ToString() ?? "",

                        SubtipoAmbulatorio =
                            reader["SubtipoAmbulatorio"]
                                .ToString() ?? "",

                        CodigoOrigen =
                            reader["CodigoOrigen"]
                                .ToString() ?? "",

                        Seguro =
                            reader["Seguro"]
                                .ToString() ?? "",

                        PlanTarifario =
                            reader["PlanTarifario"]
                                .ToString() ?? "",

                        TotalParticular =
                            Convert.ToDecimal(
                                reader["TotalPrecioParticular"]),

                        TotalTarifa =
                            Convert.ToDecimal(
                                reader["TotalTarifaAplicada"]),

                        TotalSeguro =
                            Convert.ToDecimal(
                                reader["TotalCoberturaSeguro"]),

                        TotalPaciente =
                            Convert.ToDecimal(
                                reader["TotalPaciente"]),

                        PagadoPaciente =
                            Convert.ToDecimal(
                                reader["PagadoPaciente"]),

                        PagadoSeguro =
                            Convert.ToDecimal(
                                reader["PagadoSeguro"]),

                        Estado =
                            reader["Estado"]
                                .ToString() ?? "",

                        FechaFactura =
                            Convert.ToDateTime(
                                reader["FechaFactura"])
                    });
            }


            return lista;
        }


        // ============================================================
        // ESTADOS DE FACTURA
        // ============================================================

        public List<EstadoFactura>
            ObtenerEstadosFactura()
        {
            List<EstadoFactura> lista =
                new List<EstadoFactura>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT
                    IdEstadoFactura,
                    Nombre,
                    Activo

                FROM tbEstadosFactura

                WHERE Activo = 1

                ORDER BY IdEstadoFactura;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                lista.Add(
                    new EstadoFactura
                    {
                        IdEstadoFactura =
                            Convert.ToInt32(
                                reader["IdEstadoFactura"]),

                        Nombre =
                            reader["Nombre"]
                                .ToString() ?? "",

                        Activo =
                            Convert.ToBoolean(
                                reader["Activo"])
                    });
            }


            return lista;
        }

        // ============================================================
        // GENERAR FACTURA DE EMERGENCIA
        // ============================================================

        public long GenerarFacturaEmergencia(
            EpisodioFacturable episodio,
            List<DetalleFacturable> detalles,
            string numeroAutorizacion)
        {
            if (episodio == null)
                throw new ArgumentNullException(nameof(episodio));


            if (detalles == null ||
                detalles.Count == 0)
            {
                throw new Exception(
                    "La emergencia no tiene servicios para facturar.");
            }


            // ========================================================
            // VALIDACIONES PREVIAS
            // ========================================================

            if (detalles.Any(x =>
                !x.PrecioParticularConfigurado))
            {
                throw new Exception(
                    "Existen servicios sin precio particular configurado.");
            }


            

            bool necesitaAutorizacion =
                detalles.Any(x =>
                    x.RequiereAutorizacion);


            if (necesitaAutorizacion &&
                string.IsNullOrWhiteSpace(
                    numeroAutorizacion))
            {
                throw new Exception(
                    "Esta factura contiene servicios que requieren autorización del seguro.");
            }


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            using SqlTransaction trans =
                con.BeginTransaction();


            try
            {
                // ====================================================
                // EVITAR FACTURAR LA MISMA EMERGENCIA DOS VECES
                // ====================================================

                string sqlExiste = @"
            SELECT COUNT(*)

            FROM tbFacturas F

            INNER JOIN tbEstadosFactura EF
                ON F.IdEstadoFactura =
                   EF.IdEstadoFactura

            WHERE
                F.TipoFactura = 'Emergencia'

                AND F.IdOrigen =
                    @IdOrigen

                AND F.Activo = 1

                AND EF.Nombre <> 'Anulada';
        ";


                using SqlCommand cmdExiste =
                    new SqlCommand(
                        sqlExiste,
                        con,
                        trans);


                cmdExiste.Parameters.AddWithValue(
                    "@IdOrigen",
                    episodio.IdOrigen);


                int existe =
                    Convert.ToInt32(
                        cmdExiste.ExecuteScalar());


                if (existe > 0)
                {
                    throw new Exception(
                        "Esta emergencia ya posee una factura activa.");
                }


                // ====================================================
                // ESTADO PENDIENTE DE PAGO
                // ====================================================

                string sqlEstado = @"
            SELECT TOP 1
                IdEstadoFactura

            FROM tbEstadosFactura

            WHERE Nombre =
                'Pendiente de pago';
        ";


                using SqlCommand cmdEstado =
                    new SqlCommand(
                        sqlEstado,
                        con,
                        trans);


                object? resultadoEstado =
                    cmdEstado.ExecuteScalar();


                if (resultadoEstado == null)
                {
                    throw new Exception(
                        "No existe el estado 'Pendiente de pago'.");
                }


                int idEstadoFactura =
                    Convert.ToInt32(
                        resultadoEstado);


                // ====================================================
                // GENERAR CÓDIGO
                // ====================================================

                string sqlSiguiente = @"
            SELECT
                ISNULL(MAX(IdFactura), 0) + 1

            FROM tbFacturas
            WITH (UPDLOCK, HOLDLOCK);
        ";


                using SqlCommand cmdSiguiente =
                    new SqlCommand(
                        sqlSiguiente,
                        con,
                        trans);


                long siguiente =
                    Convert.ToInt64(
                        cmdSiguiente.ExecuteScalar());


                string codigoFactura =
                    $"FAC{siguiente:000000}";


                // ====================================================
                // TOTALES
                // ====================================================

                decimal totalParticular =
                    detalles.Sum(x =>
                        x.TotalParticular);


                decimal totalTarifa =
                    detalles.Sum(x =>
                        x.TotalTarifa);


                decimal totalSeguro =
                    detalles.Sum(x =>
                        x.MontoSeguro);


                decimal totalPaciente =
                    detalles.Sum(x =>
                        x.MontoPaciente);


                // ====================================================
                // INSERTAR FACTURA
                // ====================================================

                string sqlFactura = @"
            INSERT INTO tbFacturas
            (
                CodigoFactura,
                IdPaciente,
                TipoFactura,
                SubtipoAmbulatorio,
                IdOrigen,
                CodigoOrigen,

                IdSeguro,
                IdPlanTarifario,
                NumeroAutorizacion,

                TotalPrecioParticular,
                TotalTarifaAplicada,
                TotalCoberturaSeguro,
                TotalPaciente,

                IdEstadoFactura,
                FechaFactura,

                Observaciones,
                Activo,
                FechaCreacion
            )

            VALUES
            (
                @CodigoFactura,
                @IdPaciente,
                'Emergencia',
                NULL,
                @IdOrigen,
                @CodigoOrigen,

                @IdSeguro,
                @IdPlanTarifario,
                @NumeroAutorizacion,

                @TotalParticular,
                @TotalTarifa,
                @TotalSeguro,
                @TotalPaciente,

                @IdEstadoFactura,
                SYSDATETIME(),

                NULL,
                1,
                SYSDATETIME()
            );

            SELECT
                CAST(SCOPE_IDENTITY() AS BIGINT);
        ";


                using SqlCommand cmdFactura =
                    new SqlCommand(
                        sqlFactura,
                        con,
                        trans);


                cmdFactura.Parameters.AddWithValue(
                    "@CodigoFactura",
                    codigoFactura);


                cmdFactura.Parameters.AddWithValue(
                    "@IdPaciente",
                    episodio.IdPaciente);


                cmdFactura.Parameters.AddWithValue(
                    "@IdOrigen",
                    episodio.IdOrigen);


                cmdFactura.Parameters.AddWithValue(
                    "@CodigoOrigen",
                    episodio.CodigoOrigen);


                cmdFactura.Parameters.AddWithValue(
                    "@IdSeguro",
                    episodio.IdSeguro.HasValue
                        ? episodio.IdSeguro.Value
                        : DBNull.Value);


                cmdFactura.Parameters.AddWithValue(
                    "@IdPlanTarifario",
                    episodio.IdPlanTarifario.HasValue
                        ? episodio.IdPlanTarifario.Value
                        : DBNull.Value);


                cmdFactura.Parameters.AddWithValue(
                    "@NumeroAutorizacion",
                    string.IsNullOrWhiteSpace(numeroAutorizacion)
                        ? DBNull.Value
                        : numeroAutorizacion.Trim());


                cmdFactura.Parameters.AddWithValue(
                    "@TotalParticular",
                    totalParticular);


                cmdFactura.Parameters.AddWithValue(
                    "@TotalTarifa",
                    totalTarifa);


                cmdFactura.Parameters.AddWithValue(
                    "@TotalSeguro",
                    totalSeguro);


                cmdFactura.Parameters.AddWithValue(
                    "@TotalPaciente",
                    totalPaciente);


                cmdFactura.Parameters.AddWithValue(
                    "@IdEstadoFactura",
                    idEstadoFactura);


                long idFactura =
                    Convert.ToInt64(
                        cmdFactura.ExecuteScalar());


                // ====================================================
                // CARGOS + DETALLE
                // ====================================================

                foreach (DetalleFacturable detalle
                         in detalles)
                {
                    string autorizacionDetalle =
                        detalle.RequiereAutorizacion
                            ? numeroAutorizacion.Trim()
                            : "";


                    // ================================================
                    // INSERTAR CARGO
                    // ================================================

                    string sqlCargo = @"
                INSERT INTO tbCargosPaciente
                (
                    IdPaciente,

                    TipoOrigen,
                    SubtipoAmbulatorio,
                    IdOrigen,
                    CodigoOrigen,
                    IdDetalleOrigen,

                    IdItemClinico,
                    Descripcion,
                    Cantidad,

                    PrecioParticularUnitario,
                    TotalParticular,

                    PrecioTarifaUnitario,
                    TotalTarifa,

                    PorcentajeCobertura,
                    CopagoFijoAplicado,
                    MontoSeguro,
                    MontoPaciente,

                    IdSeguro,
                    IdPlanTarifario,

                    RequiereAutorizacion,
                    NumeroAutorizacion,

                    Estado,
                    FechaCargo,
                    Activo,
                    FechaCreacion
                )

                VALUES
                (
                    @IdPaciente,

                    'Emergencia',
                    NULL,
                    @IdOrigen,
                    @CodigoOrigen,
                    @IdDetalleOrigen,

                    @IdItemClinico,
                    @Descripcion,
                    @Cantidad,

                    @PrecioParticular,
                    @TotalParticular,

                    @PrecioTarifa,
                    @TotalTarifa,

                    @Cobertura,
                    @Copago,
                    @MontoSeguro,
                    @MontoPaciente,

                    @IdSeguro,
                    @IdPlanTarifario,

                    @RequiereAutorizacion,
                    @NumeroAutorizacion,

                    'Facturado',
                    @FechaCargo,
                    1,
                    SYSDATETIME()
                );

                SELECT
                    CAST(SCOPE_IDENTITY() AS BIGINT);
            ";


                    using SqlCommand cmdCargo =
                        new SqlCommand(
                            sqlCargo,
                            con,
                            trans);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdPaciente",
                        episodio.IdPaciente);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdOrigen",
                        episodio.IdOrigen);


                    cmdCargo.Parameters.AddWithValue(
                        "@CodigoOrigen",
                        episodio.CodigoOrigen);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdDetalleOrigen",
                        detalle.IdDetalleOrigen.HasValue
                            ? detalle.IdDetalleOrigen.Value
                            : DBNull.Value);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdItemClinico",
                        detalle.IdItemClinico.HasValue
                            ? detalle.IdItemClinico.Value
                            : DBNull.Value);


                    cmdCargo.Parameters.AddWithValue(
                        "@Descripcion",
                        detalle.Descripcion);


                    cmdCargo.Parameters.AddWithValue(
                        "@Cantidad",
                        detalle.Cantidad);


                    cmdCargo.Parameters.AddWithValue(
                        "@PrecioParticular",
                        detalle.PrecioParticularUnitario);


                    cmdCargo.Parameters.AddWithValue(
                        "@TotalParticular",
                        detalle.TotalParticular);


                    cmdCargo.Parameters.AddWithValue(
                        "@PrecioTarifa",
                        detalle.PrecioTarifaUnitario);


                    cmdCargo.Parameters.AddWithValue(
                        "@TotalTarifa",
                        detalle.TotalTarifa);


                    cmdCargo.Parameters.AddWithValue(
                        "@Cobertura",
                        detalle.PorcentajeCobertura);


                    cmdCargo.Parameters.AddWithValue(
                        "@Copago",
                        detalle.CopagoFijo);


                    cmdCargo.Parameters.AddWithValue(
                        "@MontoSeguro",
                        detalle.MontoSeguro);


                    cmdCargo.Parameters.AddWithValue(
                        "@MontoPaciente",
                        detalle.MontoPaciente);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdSeguro",
                        episodio.IdSeguro.HasValue
                            ? episodio.IdSeguro.Value
                            : DBNull.Value);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdPlanTarifario",
                        detalle.IdPlanTarifario.HasValue
                            ? detalle.IdPlanTarifario.Value
                            : DBNull.Value);


                    cmdCargo.Parameters.AddWithValue(
                        "@RequiereAutorizacion",
                        detalle.RequiereAutorizacion);


                    cmdCargo.Parameters.AddWithValue(
                        "@NumeroAutorizacion",
                        string.IsNullOrWhiteSpace(
                            autorizacionDetalle)

                            ? DBNull.Value

                            : autorizacionDetalle);


                    cmdCargo.Parameters.AddWithValue(
                        "@FechaCargo",
                        detalle.FechaServicio);


                    long idCargo =
                        Convert.ToInt64(
                            cmdCargo.ExecuteScalar());


                    // ================================================
                    // INSERTAR DETALLE FACTURA
                    // ================================================

                    string sqlDetalle = @"
                INSERT INTO tbFacturaDetalles
                (
                    IdFactura,
                    IdCargo,
                    IdItemClinico,
                    TipoItem,
                    Descripcion,
                    Cantidad,

                    PrecioParticularUnitario,
                    TotalParticular,

                    PrecioTarifaUnitario,
                    TotalTarifa,

                    PorcentajeCobertura,
                    CopagoFijoAplicado,

                    MontoSeguro,
                    MontoPaciente,

                    RequiereAutorizacion,
                    NumeroAutorizacion,

                    FechaCreacion
                )

                VALUES
                (
                    @IdFactura,
                    @IdCargo,
                    @IdItemClinico,
                    @TipoItem,
                    @Descripcion,
                    @Cantidad,

                    @PrecioParticular,
                    @TotalParticular,

                    @PrecioTarifa,
                    @TotalTarifa,

                    @Cobertura,
                    @Copago,

                    @MontoSeguro,
                    @MontoPaciente,

                    @RequiereAutorizacion,
                    @NumeroAutorizacion,

                    SYSDATETIME()
                );
            ";


                    using SqlCommand cmdDetalle =
                        new SqlCommand(
                            sqlDetalle,
                            con,
                            trans);


                    cmdDetalle.Parameters.AddWithValue(
                        "@IdFactura",
                        idFactura);


                    cmdDetalle.Parameters.AddWithValue(
                        "@IdCargo",
                        idCargo);


                    cmdDetalle.Parameters.AddWithValue(
                        "@IdItemClinico",
                        detalle.IdItemClinico.HasValue
                            ? detalle.IdItemClinico.Value
                            : DBNull.Value);


                    cmdDetalle.Parameters.AddWithValue(
                        "@TipoItem",
                        detalle.TipoItem);


                    cmdDetalle.Parameters.AddWithValue(
                        "@Descripcion",
                        detalle.Descripcion);


                    cmdDetalle.Parameters.AddWithValue(
                        "@Cantidad",
                        detalle.Cantidad);


                    cmdDetalle.Parameters.AddWithValue(
                        "@PrecioParticular",
                        detalle.PrecioParticularUnitario);


                    cmdDetalle.Parameters.AddWithValue(
                        "@TotalParticular",
                        detalle.TotalParticular);


                    cmdDetalle.Parameters.AddWithValue(
                        "@PrecioTarifa",
                        detalle.PrecioTarifaUnitario);


                    cmdDetalle.Parameters.AddWithValue(
                        "@TotalTarifa",
                        detalle.TotalTarifa);


                    cmdDetalle.Parameters.AddWithValue(
                        "@Cobertura",
                        detalle.PorcentajeCobertura);


                    cmdDetalle.Parameters.AddWithValue(
                        "@Copago",
                        detalle.CopagoFijo);


                    cmdDetalle.Parameters.AddWithValue(
                        "@MontoSeguro",
                        detalle.MontoSeguro);


                    cmdDetalle.Parameters.AddWithValue(
                        "@MontoPaciente",
                        detalle.MontoPaciente);


                    cmdDetalle.Parameters.AddWithValue(
                        "@RequiereAutorizacion",
                        detalle.RequiereAutorizacion);


                    cmdDetalle.Parameters.AddWithValue(
                        "@NumeroAutorizacion",
                        string.IsNullOrWhiteSpace(
                            autorizacionDetalle)

                            ? DBNull.Value

                            : autorizacionDetalle);


                    cmdDetalle.ExecuteNonQuery();
                }


                // ====================================================
                // MARCAR ITEMS DE EMERGENCIA COMO FACTURADOS
                // ====================================================

                string sqlActualizarItems = @"
            UPDATE tbEmergenciaItems

            SET Estado = 'Facturado'

            WHERE
                IdEmergencia = @IdEmergencia
                AND Activo = 1;
        ";


                using SqlCommand cmdActualizar =
                    new SqlCommand(
                        sqlActualizarItems,
                        con,
                        trans);


                cmdActualizar.Parameters.AddWithValue(
                    "@IdEmergencia",
                    episodio.IdOrigen);


                cmdActualizar.ExecuteNonQuery();


                trans.Commit();


                return idFactura;
            }
            catch
            {
                trans.Rollback();

                throw;
            }
        }

        // ============================================================
        // INTERNAMIENTOS PENDIENTES DE FACTURAR
        // ============================================================

        public List<EpisodioFacturable>
            ObtenerInternamientosPendientes()
        {
            List<EpisodioFacturable> lista =
                new List<EpisodioFacturable>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
        SELECT

            H.IdInternamiento,
            H.CodigoInternamiento,
            H.IdPaciente,
            H.FechaIngreso,
            H.FechaAlta,

            ISNULL(
                H.MotivoInternamiento,
                ''
            ) AS MotivoInternamiento,

            P.CodigoPaciente,

            LTRIM(RTRIM(CONCAT(
                P.Nombre, ' ',
                P.SegundoNombre, ' ',
                P.Apellido, ' ',
                P.SegundoApellido
            ))) AS Paciente,

            CASE

                WHEN P.Indocumentado = 1
                    THEN ISNULL(
                        P.CodigoTemporal,
                        ''
                    )

                ELSE
                    ISNULL(
                        P.NumeroDocumento,
                        ''
                    )

            END AS Documento,

            P.IdSeguro,

            ISNULL(
                S.Nombre,
                'Sin seguro'
            ) AS Seguro,

            LTRIM(RTRIM(CONCAT(
                M.Nombre, ' ',
                M.Apellido
            ))) AS Medico,

            ES.Nombre
                AS Especialidad,

            TI.Nombre
                AS TipoInternamiento,

            EI.Nombre
                AS EstadoInternamiento,


            COALESCE(
                PlanItem.IdPlanTarifario,
                PlanPaciente.IdPlanTarifario
            ) AS IdPlanTarifario,


            COALESCE(
                PlanItem.NombrePlan,
                PlanPaciente.NombrePlan,
                ''
            ) AS PlanTarifario,


            COALESCE(
                PlanItem.TipoPlan,
                PlanPaciente.TipoPlan,
                ''
            ) AS TipoPlan


        FROM tbInternamientos H


        INNER JOIN tbPacientes P
            ON H.IdPaciente =
               P.IdPaciente


        LEFT JOIN tbSeguros S
            ON P.IdSeguro =
               S.IdSeguro


        INNER JOIN tbMedicos M
            ON H.IdMedicoResponsable =
               M.IdMedico


        INNER JOIN tbEspecialidades ES
            ON H.IdEspecialidad =
               ES.IdEspecialidad


        INNER JOIN tbTiposInternamiento TI
            ON H.IdTipoInternamiento =
               TI.IdTipoInternamiento


        INNER JOIN tbEstadosInternamiento EI
            ON H.IdEstadoInternamiento =
               EI.IdEstadoInternamiento


        /* =================================================
           PLAN QUE YA FUE UTILIZADO EN LOS CONSUMOS
        ================================================= */

        OUTER APPLY
        (
            SELECT TOP 1

                PT.IdPlanTarifario,

                PT.Nombre
                    AS NombrePlan,

                PT.Tipo
                    AS TipoPlan

            FROM tbInternamientoItems II

            INNER JOIN tbPlanesTarifarios PT
                ON II.IdPlanTarifarioAplicado =
                   PT.IdPlanTarifario

            WHERE
                II.IdInternamiento =
                    H.IdInternamiento

                AND II.Activo = 1

                AND II.IdPlanTarifarioAplicado
                    IS NOT NULL

            ORDER BY
                II.FechaRegistro DESC,
                II.IdInternamientoItem DESC

        ) PlanItem


        /* =================================================
           PLAN ACTUAL DEL PACIENTE COMO RESPALDO
        ================================================= */

        OUTER APPLY
        (
            SELECT TOP 1

                PT.IdPlanTarifario,

                PT.Nombre
                    AS NombrePlan,

                PT.Tipo
                    AS TipoPlan

            FROM tbPlanesTarifarios PT

            WHERE
                PT.IdSeguro =
                    P.IdSeguro

                AND PT.Activo = 1

            ORDER BY
                PT.IdPlanTarifario

        ) PlanPaciente


        /* =================================================
           SOLO EPISODIOS FINALIZADOS
        ================================================= */

        WHERE

            H.Activo = 1

            AND EI.Nombre IN
            (
                'Alta médica',
                'Trasladado'
            )


            /* =============================================
               EVITAR FACTURAR DOS VECES
            ============================================== */

            AND NOT EXISTS
            (
                SELECT 1

                FROM tbFacturas F

                INNER JOIN tbEstadosFactura EF
                    ON F.IdEstadoFactura =
                       EF.IdEstadoFactura

                WHERE
                    F.TipoFactura =
                        'Internamiento'

                    AND F.IdOrigen =
                        H.IdInternamiento

                    AND F.Activo = 1

                    AND EF.Nombre <>
                        'Anulada'
            )


        ORDER BY

            ISNULL(
                H.FechaAlta,
                H.FechaIngreso
            ) DESC,

            H.IdInternamiento DESC;
    ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                EpisodioFacturable episodio =
                    new EpisodioFacturable
                    {
                        IdOrigen =
                            Convert.ToInt64(
                                reader[
                                    "IdInternamiento"
                                ]),

                        CodigoOrigen =
                            reader[
                                "CodigoInternamiento"
                            ].ToString() ?? "",

                        TipoFactura =
                            "Internamiento",

                        IdPaciente =
                            Convert.ToInt32(
                                reader[
                                    "IdPaciente"
                                ]),

                        CodigoPaciente =
                            reader[
                                "CodigoPaciente"
                            ].ToString() ?? "",

                        NombrePaciente =
                            reader[
                                "Paciente"
                            ].ToString() ?? "",

                        Documento =
                            reader[
                                "Documento"
                            ].ToString() ?? "",

                        IdSeguro =
                            reader["IdSeguro"] ==
                            DBNull.Value

                                ? null

                                : Convert.ToInt32(
                                    reader[
                                        "IdSeguro"
                                    ]),

                        Seguro =
                            reader[
                                "Seguro"
                            ].ToString() ?? "",

                        IdPlanTarifario =
                            reader[
                                "IdPlanTarifario"
                            ] == DBNull.Value

                                ? null

                                : Convert.ToInt32(
                                    reader[
                                        "IdPlanTarifario"
                                    ]),

                        PlanTarifario =
                            reader[
                                "PlanTarifario"
                            ].ToString() ?? "",

                        TipoPlan =
                            reader[
                                "TipoPlan"
                            ].ToString() ?? "",

                        Fecha =
                            Convert.ToDateTime(
                                reader[
                                    "FechaIngreso"
                                ]),

                        Motivo =
                            reader[
                                "MotivoInternamiento"
                            ].ToString() ?? "",

                        Medico =
                            reader[
                                "Medico"
                            ].ToString() ?? "",

                        Especialidad =
                            reader[
                                "Especialidad"
                            ].ToString() ?? ""
                    };


                lista.Add(
                    episodio);
            }


            reader.Close();


            // ========================================================
            // CALCULAR TOTALES
            // ========================================================

            foreach (EpisodioFacturable episodio
                     in lista)
            {
                List<DetalleFacturable> detalle =
                    ObtenerDetalleInternamiento(
                        episodio.IdOrigen);


                episodio.CantidadItems =
                    detalle.Count;


                episodio.TotalParticular =
                    detalle.Sum(
                        x => x.TotalParticular);


                episodio.TotalTarifa =
                    detalle.Sum(
                        x => x.TotalTarifa);


                episodio.TotalSeguro =
                    detalle.Sum(
                        x => x.MontoSeguro);


                episodio.TotalPaciente =
                    detalle.Sum(
                        x => x.MontoPaciente);


                episodio.TieneProblemasCobertura =
                    detalle.Any(
                        x =>
                            !x.PrecioParticularConfigurado

                            ||

                            (
                                x.RequiereAutorizacion
                                &&
                                string.IsNullOrWhiteSpace(
                                    x.NumeroAutorizacion)
                            ));
            }


            return lista;
        }

        // ============================================================
        // DETALLE FACTURABLE DE INTERNAMIENTO
        // ============================================================

        public List<DetalleFacturable>
            ObtenerDetalleInternamiento(
                long idInternamiento)
        {
            List<DetalleFacturable> lista =
                new List<DetalleFacturable>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            // ========================================================
            // INFORMACIÓN DEL INTERNAMIENTO
            // ========================================================

            DateTime fechaIngreso;

            DateTime fechaFin;

            string tipoInternamiento;

            int? idPlanTarifario;

            string nombrePlan;

            string tipoPlan;


            string sqlInfo = @"
        SELECT

            H.FechaIngreso,

            H.FechaAlta,

            TI.Nombre
                AS TipoInternamiento,


            COALESCE(
                PlanItem.IdPlanTarifario,
                PlanPaciente.IdPlanTarifario
            ) AS IdPlanTarifario,


            COALESCE(
                PlanItem.NombrePlan,
                PlanPaciente.NombrePlan,
                ''
            ) AS NombrePlan,


            COALESCE(
                PlanItem.TipoPlan,
                PlanPaciente.TipoPlan,
                ''
            ) AS TipoPlan


        FROM tbInternamientos H


        INNER JOIN tbPacientes P
            ON H.IdPaciente =
               P.IdPaciente


        INNER JOIN tbTiposInternamiento TI
            ON H.IdTipoInternamiento =
               TI.IdTipoInternamiento


        OUTER APPLY
        (
            SELECT TOP 1

                PT.IdPlanTarifario,

                PT.Nombre
                    AS NombrePlan,

                PT.Tipo
                    AS TipoPlan

            FROM tbInternamientoItems II

            INNER JOIN tbPlanesTarifarios PT
                ON II.IdPlanTarifarioAplicado =
                   PT.IdPlanTarifario

            WHERE
                II.IdInternamiento =
                    H.IdInternamiento

                AND II.Activo = 1

                AND II.IdPlanTarifarioAplicado
                    IS NOT NULL

            ORDER BY
                II.FechaRegistro DESC,
                II.IdInternamientoItem DESC

        ) PlanItem


        OUTER APPLY
        (
            SELECT TOP 1

                PT.IdPlanTarifario,

                PT.Nombre
                    AS NombrePlan,

                PT.Tipo
                    AS TipoPlan

            FROM tbPlanesTarifarios PT

            WHERE
                PT.IdSeguro =
                    P.IdSeguro

                AND PT.Activo = 1

            ORDER BY
                PT.IdPlanTarifario

        ) PlanPaciente


        WHERE
            H.IdInternamiento =
                @IdInternamiento;
    ";


            using
            (
                SqlCommand cmdInfo =
                    new SqlCommand(
                        sqlInfo,
                        con)
            )
            {
                cmdInfo.Parameters.Add(
                    "@IdInternamiento",
                    SqlDbType.BigInt)
                    .Value =
                        idInternamiento;


                using SqlDataReader reader =
                    cmdInfo.ExecuteReader();


                if (!reader.Read())
                {
                    throw new Exception(
                        "No se encontró el internamiento.");
                }


                fechaIngreso =
                    Convert.ToDateTime(
                        reader[
                            "FechaIngreso"
                        ]);


                fechaFin =
                    reader["FechaAlta"] ==
                    DBNull.Value

                        ? DateTime.Now

                        : Convert.ToDateTime(
                            reader[
                                "FechaAlta"
                            ]);


                tipoInternamiento =
                    reader[
                        "TipoInternamiento"
                    ].ToString() ?? "";


                idPlanTarifario =
                    reader[
                        "IdPlanTarifario"
                    ] == DBNull.Value

                        ? null

                        : Convert.ToInt32(
                            reader[
                                "IdPlanTarifario"
                            ]);


                nombrePlan =
                    reader[
                        "NombrePlan"
                    ].ToString() ?? "";


                tipoPlan =
                    reader[
                        "TipoPlan"
                    ].ToString() ?? "";
            }


            if (!idPlanTarifario.HasValue)
            {
                throw new Exception(
                    "No se pudo determinar el plan tarifario del internamiento.");
            }


            // ========================================================
            // 1. ESTANCIA
            // ========================================================

            string codigoEstancia =

                tipoInternamiento.Contains(
                    "UCI",
                    StringComparison.OrdinalIgnoreCase)

                    ? "HOSP-002"

                    : "HOSP-001";


            // ========================================================
            // DÍAS FACTURABLES
            //
            // Ejemplo:
            // 18 → 21 = 4 días
            // Mismo día = mínimo 1
            // ========================================================

            decimal dias =
                Math.Max(
                    1,
                    (
                        fechaFin.Date -
                        fechaIngreso.Date
                    ).Days + 1);


            string sqlEstancia = @"
        SELECT TOP 1

            I.IdItemClinico,
            I.Codigo,
            I.Nombre,

            TI.Nombre
                AS TipoItem,


            TarifaParticular.Precio
                AS PrecioParticular,


            CASE

                WHEN TarifaParticular.Precio
                     IS NULL

                    THEN CAST(0 AS BIT)

                ELSE
                    CAST(1 AS BIT)

            END
                AS PrecioParticularConfigurado,


            TarifaPlan.Precio
                AS PrecioTarifa,


            CP.IdCoberturaPlan,

            CP.PorcentajeCobertura,

            CP.CopagoFijo,

            CP.MontoMaximoCobertura,

            CP.RequiereAutorizacion


        FROM tbItemsClinicos I


        INNER JOIN tbTiposItemClinico TI
            ON I.IdTipoItem =
               TI.IdTipoItem


        /* ===============================================
           PRECIO PARTICULAR
        =============================================== */

        OUTER APPLY
        (
            SELECT TOP 1

                T.Precio

            FROM tbTarifasItems T

            INNER JOIN tbPlanesTarifarios P
                ON T.IdPlanTarifario =
                   P.IdPlanTarifario

            WHERE
                T.IdItemClinico =
                    I.IdItemClinico

                AND T.Activo = 1

                AND P.Activo = 1

                AND
                (
                       LOWER(
                           ISNULL(
                               P.Tipo,
                               ''
                           )
                       ) LIKE '%privado%'

                    OR LOWER(
                           ISNULL(
                               P.Nombre,
                               ''
                           )
                       ) LIKE '%privado%'

                    OR LOWER(
                           ISNULL(
                               P.Tipo,
                               ''
                           )
                       ) LIKE '%particular%'

                    OR LOWER(
                           ISNULL(
                               P.Nombre,
                               ''
                           )
                       ) LIKE '%particular%'

                    OR LOWER(
                           ISNULL(
                               P.Tipo,
                               ''
                           )
                       ) LIKE '%sin seguro%'

                    OR LOWER(
                           ISNULL(
                               P.Nombre,
                               ''
                           )
                       ) LIKE '%sin seguro%'
                )

                AND T.FechaInicio <=
                    CAST(GETDATE() AS DATE)

                AND
                (
                    T.FechaFin IS NULL

                    OR

                    T.FechaFin >=
                        CAST(GETDATE() AS DATE)
                )

            ORDER BY
                T.FechaInicio DESC,
                T.IdTarifa DESC

        ) TarifaParticular


        /* ===============================================
           TARIFA DEL PLAN
        =============================================== */

        OUTER APPLY
        (
            SELECT TOP 1

                T.Precio

            FROM tbTarifasItems T

            WHERE
                T.IdItemClinico =
                    I.IdItemClinico

                AND T.IdPlanTarifario =
                    @IdPlanTarifario

                AND T.Activo = 1

                AND T.FechaInicio <=
                    CAST(GETDATE() AS DATE)

                AND
                (
                    T.FechaFin IS NULL

                    OR

                    T.FechaFin >=
                        CAST(GETDATE() AS DATE)
                )

            ORDER BY
                T.FechaInicio DESC,
                T.IdTarifa DESC

        ) TarifaPlan


        /* ===============================================
           COBERTURA
        =============================================== */

        OUTER APPLY
        (
            SELECT TOP 1

                C.IdCoberturaPlan,

                C.PorcentajeCobertura,

                C.CopagoFijo,

                C.MontoMaximoCobertura,

                C.RequiereAutorizacion

            FROM tbCoberturasPlan C

            WHERE
                C.IdPlanTarifario =
                    @IdPlanTarifario

                AND C.IdItemClinico =
                    I.IdItemClinico

                AND C.Activo = 1

                AND C.FechaInicio <=
                    CAST(GETDATE() AS DATE)

                AND
                (
                    C.FechaFin IS NULL

                    OR

                    C.FechaFin >=
                        CAST(GETDATE() AS DATE)
                )

            ORDER BY
                C.FechaInicio DESC,
                C.IdCoberturaPlan DESC

        ) CP


        WHERE
            I.Codigo =
                @CodigoEstancia

            AND I.Activo = 1;
    ";


            using
            (
                SqlCommand cmdEstancia =
                    new SqlCommand(
                        sqlEstancia,
                        con)
            )
            {
                cmdEstancia.Parameters.Add(
                    "@IdPlanTarifario",
                    SqlDbType.Int)
                    .Value =
                        idPlanTarifario.Value;


                cmdEstancia.Parameters.Add(
                    "@CodigoEstancia",
                    SqlDbType.VarChar,
                    30)
                    .Value =
                        codigoEstancia;


                using SqlDataReader reader =
                    cmdEstancia.ExecuteReader();


                if (!reader.Read())
                {
                    throw new Exception(
                        $"No se encontró el servicio {codigoEstancia} de estancia hospitalaria.");
                }


                decimal precioTarifa =
                    reader[
                        "PrecioTarifa"
                    ] == DBNull.Value

                        ? 0

                        : Convert.ToDecimal(
                            reader[
                                "PrecioTarifa"
                            ]);


                bool particularConfigurado =
                    Convert.ToBoolean(
                        reader[
                            "PrecioParticularConfigurado"
                        ]);


                decimal precioParticular =

                    particularConfigurado

                        ? Convert.ToDecimal(
                            reader[
                                "PrecioParticular"
                            ])

                        : 0;


                DetalleFacturable estancia =
                    new DetalleFacturable
                    {
                        IdDetalleOrigen =
                            null,

                        IdItemClinico =
                            Convert.ToInt64(
                                reader[
                                    "IdItemClinico"
                                ]),

                        CodigoItem =
                            reader[
                                "Codigo"
                            ].ToString() ?? "",

                        TipoItem =
                            "Hospitalización",

                        Descripcion =
                            reader[
                                "Nombre"
                            ].ToString() ?? "",

                        Cantidad =
                            dias,

                        PrecioParticularUnitario =
                            precioParticular,

                        PrecioParticularConfigurado =
                            particularConfigurado,

                        PrecioTarifaUnitario =
                            precioTarifa,

                        IdPlanTarifario =
                            idPlanTarifario,

                        PlanTarifario =
                            nombrePlan,

                        TipoPlan =
                            tipoPlan,

                        CoberturaConfigurada =
                            reader[
                                "IdCoberturaPlan"
                            ] != DBNull.Value,

                        PorcentajeCobertura =
                            reader[
                                "PorcentajeCobertura"
                            ] == DBNull.Value

                                ? 0

                                : Convert.ToDecimal(
                                    reader[
                                        "PorcentajeCobertura"
                                    ]),

                        CopagoFijo =
                            reader[
                                "CopagoFijo"
                            ] == DBNull.Value

                                ? 0

                                : Convert.ToDecimal(
                                    reader[
                                        "CopagoFijo"
                                    ]),

                        MontoMaximoCobertura =
                            reader[
                                "MontoMaximoCobertura"
                            ] == DBNull.Value

                                ? null

                                : Convert.ToDecimal(
                                    reader[
                                        "MontoMaximoCobertura"
                                    ]),

                        RequiereAutorizacion =
                            reader[
                                "RequiereAutorizacion"
                            ] != DBNull.Value

                            &&

                            Convert.ToBoolean(
                                reader[
                                    "RequiereAutorizacion"
                                ]),

                        FechaServicio =
                            fechaIngreso
                    };


                if (!estancia.PrecioParticularConfigurado
                    &&
                    EsPlanParticular(
                        estancia.TipoPlan,
                        estancia.PlanTarifario))
                {
                    estancia.PrecioParticularUnitario =
                        estancia.PrecioTarifaUnitario;


                    estancia.PrecioParticularConfigurado =
                        true;
                }


                estancia.TotalParticular =
                    estancia.Cantidad *
                    estancia.PrecioParticularUnitario;


                estancia.TotalTarifa =
                    estancia.Cantidad *
                    estancia.PrecioTarifaUnitario;


                CalcularCobertura(
                    estancia);


                lista.Add(
                    estancia);
            }


            // ========================================================
            // 2. CONSUMOS DEL INTERNAMIENTO
            // ========================================================

            string sqlConsumos = @"
        SELECT

            II.IdInternamientoItem,
            II.IdItemClinico,

            I.Codigo,
            I.Nombre,

            TI.Nombre
                AS TipoItem,

            II.Cantidad,

            II.PrecioUnitarioAplicado,

            II.IdPlanTarifarioAplicado,

            ISNULL(
                PT.Nombre,
                ''
            ) AS PlanTarifario,

            ISNULL(
                PT.Tipo,
                ''
            ) AS TipoPlan,

            II.FechaRegistro,


            /* =============================================
               PRECIO PARTICULAR
            ============================================== */

            TP.Precio
                AS PrecioParticular,


            CASE

                WHEN TP.Precio IS NULL
                    THEN CAST(0 AS BIT)

                ELSE
                    CAST(1 AS BIT)

            END
                AS PrecioParticularConfigurado,


            /* =============================================
               COBERTURA
            ============================================== */

            CP.IdCoberturaPlan,

            CP.PorcentajeCobertura,

            CP.CopagoFijo,

            CP.MontoMaximoCobertura,

            CP.RequiereAutorizacion


        FROM tbInternamientoItems II


        INNER JOIN tbItemsClinicos I
            ON II.IdItemClinico =
               I.IdItemClinico


        INNER JOIN tbTiposItemClinico TI
            ON I.IdTipoItem =
               TI.IdTipoItem


        LEFT JOIN tbPlanesTarifarios PT
            ON II.IdPlanTarifarioAplicado =
               PT.IdPlanTarifario


        /* ===============================================
           PRECIO PARTICULAR
        =============================================== */

        OUTER APPLY
        (
            SELECT TOP 1

                T.Precio

            FROM tbTarifasItems T

            INNER JOIN tbPlanesTarifarios PP
                ON T.IdPlanTarifario =
                   PP.IdPlanTarifario

            WHERE
                T.IdItemClinico =
                    II.IdItemClinico

                AND T.Activo = 1

                AND PP.Activo = 1

                AND
                (
                       LOWER(ISNULL(PP.Tipo, ''))
                            LIKE '%privado%'

                    OR LOWER(ISNULL(PP.Nombre, ''))
                            LIKE '%privado%'

                    OR LOWER(ISNULL(PP.Tipo, ''))
                            LIKE '%particular%'

                    OR LOWER(ISNULL(PP.Nombre, ''))
                            LIKE '%particular%'

                    OR LOWER(ISNULL(PP.Tipo, ''))
                            LIKE '%sin seguro%'

                    OR LOWER(ISNULL(PP.Nombre, ''))
                            LIKE '%sin seguro%'
                )

                AND T.FechaInicio <=
                    CAST(GETDATE() AS DATE)

                AND
                (
                    T.FechaFin IS NULL

                    OR

                    T.FechaFin >=
                        CAST(GETDATE() AS DATE)
                )

            ORDER BY
                T.FechaInicio DESC,
                T.IdTarifa DESC

        ) TP


        /* ===============================================
           COBERTURA DEL PLAN
        =============================================== */

        OUTER APPLY
        (
            SELECT TOP 1

                C.IdCoberturaPlan,

                C.PorcentajeCobertura,

                C.CopagoFijo,

                C.MontoMaximoCobertura,

                C.RequiereAutorizacion

            FROM tbCoberturasPlan C

            WHERE
                C.IdPlanTarifario =
                    II.IdPlanTarifarioAplicado

                AND C.IdItemClinico =
                    II.IdItemClinico

                AND C.Activo = 1

                AND C.FechaInicio <=
                    CAST(GETDATE() AS DATE)

                AND
                (
                    C.FechaFin IS NULL

                    OR

                    C.FechaFin >=
                        CAST(GETDATE() AS DATE)
                )

            ORDER BY
                C.FechaInicio DESC,
                C.IdCoberturaPlan DESC

        ) CP


        WHERE
            II.IdInternamiento =
                @IdInternamiento

            AND II.Activo = 1

            AND II.Estado =
                'Registrado'


        ORDER BY

            TI.Nombre,

            I.Nombre;
    ";


            using
            (
                SqlCommand cmdConsumos =
                    new SqlCommand(
                        sqlConsumos,
                        con)
            )
            {
                cmdConsumos.Parameters.Add(
                    "@IdInternamiento",
                    SqlDbType.BigInt)
                    .Value =
                        idInternamiento;


                using SqlDataReader reader =
                    cmdConsumos.ExecuteReader();


                while (reader.Read())
                {
                    decimal cantidad =
                        Convert.ToDecimal(
                            reader[
                                "Cantidad"
                            ]);


                    decimal tarifaAplicada =
                        Convert.ToDecimal(
                            reader[
                                "PrecioUnitarioAplicado"
                            ]);


                    bool particularConfigurado =
                        Convert.ToBoolean(
                            reader[
                                "PrecioParticularConfigurado"
                            ]);


                    decimal precioParticular =

                        particularConfigurado

                            ? Convert.ToDecimal(
                                reader[
                                    "PrecioParticular"
                                ])

                            : 0;


                    DetalleFacturable detalle =
                        new DetalleFacturable
                        {
                            IdDetalleOrigen =
                                Convert.ToInt64(
                                    reader[
                                        "IdInternamientoItem"
                                    ]),

                            IdItemClinico =
                                Convert.ToInt64(
                                    reader[
                                        "IdItemClinico"
                                    ]),

                            CodigoItem =
                                reader[
                                    "Codigo"
                                ].ToString() ?? "",

                            TipoItem =
                                reader[
                                    "TipoItem"
                                ].ToString() ?? "",

                            Descripcion =
                                reader[
                                    "Nombre"
                                ].ToString() ?? "",

                            Cantidad =
                                cantidad,

                            PrecioParticularUnitario =
                                precioParticular,

                            PrecioParticularConfigurado =
                                particularConfigurado,

                            PrecioTarifaUnitario =
                                tarifaAplicada,

                            IdPlanTarifario =
                                reader[
                                    "IdPlanTarifarioAplicado"
                                ] == DBNull.Value

                                    ? null

                                    : Convert.ToInt32(
                                        reader[
                                            "IdPlanTarifarioAplicado"
                                        ]),

                            PlanTarifario =
                                reader[
                                    "PlanTarifario"
                                ].ToString() ?? "",

                            TipoPlan =
                                reader[
                                    "TipoPlan"
                                ].ToString() ?? "",

                            CoberturaConfigurada =
                                reader[
                                    "IdCoberturaPlan"
                                ] != DBNull.Value,

                            PorcentajeCobertura =
                                reader[
                                    "PorcentajeCobertura"
                                ] == DBNull.Value

                                    ? 0

                                    : Convert.ToDecimal(
                                        reader[
                                            "PorcentajeCobertura"
                                        ]),

                            CopagoFijo =
                                reader[
                                    "CopagoFijo"
                                ] == DBNull.Value

                                    ? 0

                                    : Convert.ToDecimal(
                                        reader[
                                            "CopagoFijo"
                                        ]),

                            MontoMaximoCobertura =
                                reader[
                                    "MontoMaximoCobertura"
                                ] == DBNull.Value

                                    ? null

                                    : Convert.ToDecimal(
                                        reader[
                                            "MontoMaximoCobertura"
                                        ]),

                            RequiereAutorizacion =
                                reader[
                                    "RequiereAutorizacion"
                                ] != DBNull.Value

                                &&

                                Convert.ToBoolean(
                                    reader[
                                        "RequiereAutorizacion"
                                    ]),

                            FechaServicio =
                                Convert.ToDateTime(
                                    reader[
                                        "FechaRegistro"
                                    ])
                        };


                    if (!detalle.PrecioParticularConfigurado
                        &&
                        EsPlanParticular(
                            detalle.TipoPlan,
                            detalle.PlanTarifario))
                    {
                        detalle.PrecioParticularUnitario =
                            detalle.PrecioTarifaUnitario;


                        detalle.PrecioParticularConfigurado =
                            true;
                    }


                    detalle.TotalParticular =
                        detalle.Cantidad *
                        detalle.PrecioParticularUnitario;


                    detalle.TotalTarifa =
                        detalle.Cantidad *
                        detalle.PrecioTarifaUnitario;


                    CalcularCobertura(
                        detalle);


                    lista.Add(
                        detalle);
                }
            }


            return lista;
        }

        // ============================================================
        // GENERAR FACTURA DE INTERNAMIENTO
        // ============================================================

        public long GenerarFacturaInternamiento(
            EpisodioFacturable episodio,
            List<DetalleFacturable> detalles,
            string numeroAutorizacion)
        {
            if (episodio == null)
            {
                throw new ArgumentNullException(
                    nameof(episodio));
            }


            if (detalles == null ||
                detalles.Count == 0)
            {
                throw new Exception(
                    "El internamiento no tiene cargos para facturar.");
            }


            // ========================================================
            // VALIDAR PRECIOS
            // ========================================================

            if (detalles.Any(
                x =>
                    !x.PrecioParticularConfigurado))
            {
                throw new Exception(
                    "Existen servicios sin precio particular configurado.");
            }


            // ========================================================
            // AUTORIZACIÓN
            // ========================================================

            bool necesitaAutorizacion =
                detalles.Any(
                    x =>
                        x.RequiereAutorizacion);


            if (necesitaAutorizacion
                &&
                string.IsNullOrWhiteSpace(
                    numeroAutorizacion))
            {
                throw new Exception(
                    "Uno o más servicios requieren autorización del seguro.");
            }


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            using SqlTransaction trans =
                con.BeginTransaction();


            try
            {
                // ====================================================
                // VERIFICAR QUE EL INTERNAMIENTO ESTÉ FINALIZADO
                // ====================================================

                string sqlEstadoInternamiento = @"
            SELECT TOP 1
                EI.Nombre

            FROM tbInternamientos H

            INNER JOIN tbEstadosInternamiento EI
                ON H.IdEstadoInternamiento =
                   EI.IdEstadoInternamiento

            WHERE
                H.IdInternamiento =
                    @IdInternamiento

                AND H.Activo = 1;
        ";


                using SqlCommand cmdEstadoInternamiento =
                    new SqlCommand(
                        sqlEstadoInternamiento,
                        con,
                        trans);


                cmdEstadoInternamiento.Parameters
                    .AddWithValue(
                        "@IdInternamiento",
                        episodio.IdOrigen);


                string estadoInternamiento =
                    cmdEstadoInternamiento
                        .ExecuteScalar()
                        ?.ToString()
                    ?? "";


                bool finalizado =

                    estadoInternamiento.Equals(
                        "Alta médica",
                        StringComparison.OrdinalIgnoreCase)

                    ||

                    estadoInternamiento.Equals(
                        "Trasladado",
                        StringComparison.OrdinalIgnoreCase);


                if (!finalizado)
                {
                    throw new Exception(
                        "El internamiento debe tener Alta médica o estar Trasladado antes de generar su factura final.");
                }


                // ====================================================
                // EVITAR FACTURA DUPLICADA
                // ====================================================

                string sqlExiste = @"
            SELECT COUNT(*)

            FROM tbFacturas F

            INNER JOIN tbEstadosFactura EF
                ON F.IdEstadoFactura =
                   EF.IdEstadoFactura

            WHERE
                F.TipoFactura =
                    'Internamiento'

                AND F.IdOrigen =
                    @IdOrigen

                AND F.Activo = 1

                AND EF.Nombre <>
                    'Anulada';
        ";


                using SqlCommand cmdExiste =
                    new SqlCommand(
                        sqlExiste,
                        con,
                        trans);


                cmdExiste.Parameters
                    .AddWithValue(
                        "@IdOrigen",
                        episodio.IdOrigen);


                int existe =
                    Convert.ToInt32(
                        cmdExiste.ExecuteScalar());


                if (existe > 0)
                {
                    throw new Exception(
                        "Este internamiento ya posee una factura activa.");
                }


                // ====================================================
                // ESTADO FACTURA
                // ====================================================

                string sqlEstado = @"
            SELECT TOP 1
                IdEstadoFactura

            FROM tbEstadosFactura

            WHERE Nombre =
                'Pendiente de pago';
        ";


                using SqlCommand cmdEstado =
                    new SqlCommand(
                        sqlEstado,
                        con,
                        trans);


                object? resultadoEstado =
                    cmdEstado.ExecuteScalar();


                if (resultadoEstado == null)
                {
                    throw new Exception(
                        "No existe el estado Pendiente de pago.");
                }


                int idEstadoFactura =
                    Convert.ToInt32(
                        resultadoEstado);


                // ====================================================
                // CÓDIGO DE FACTURA
                // ====================================================

                string sqlSiguiente = @"
            SELECT
                ISNULL(MAX(IdFactura), 0) + 1

            FROM tbFacturas
            WITH (UPDLOCK, HOLDLOCK);
        ";


                using SqlCommand cmdSiguiente =
                    new SqlCommand(
                        sqlSiguiente,
                        con,
                        trans);


                long siguiente =
                    Convert.ToInt64(
                        cmdSiguiente.ExecuteScalar());


                string codigoFactura =
                    $"FAC{siguiente:000000}";


                // ====================================================
                // TOTALES
                // ====================================================

                decimal totalParticular =
                    detalles.Sum(
                        x => x.TotalParticular);


                decimal totalTarifa =
                    detalles.Sum(
                        x => x.TotalTarifa);


                decimal totalSeguro =
                    detalles.Sum(
                        x => x.MontoSeguro);


                decimal totalPaciente =
                    detalles.Sum(
                        x => x.MontoPaciente);


                // ====================================================
                // INSERTAR FACTURA
                // ====================================================

                string sqlFactura = @"
            INSERT INTO tbFacturas
            (
                CodigoFactura,
                IdPaciente,

                TipoFactura,
                SubtipoAmbulatorio,
                IdOrigen,
                CodigoOrigen,

                IdSeguro,
                IdPlanTarifario,
                NumeroAutorizacion,

                TotalPrecioParticular,
                TotalTarifaAplicada,
                TotalCoberturaSeguro,
                TotalPaciente,

                IdEstadoFactura,
                FechaFactura,

                Observaciones,

                Activo,
                FechaCreacion
            )

            VALUES
            (
                @CodigoFactura,
                @IdPaciente,

                'Internamiento',
                NULL,
                @IdOrigen,
                @CodigoOrigen,

                @IdSeguro,
                @IdPlanTarifario,
                @NumeroAutorizacion,

                @TotalParticular,
                @TotalTarifa,
                @TotalSeguro,
                @TotalPaciente,

                @IdEstadoFactura,
                SYSDATETIME(),

                NULL,

                1,
                SYSDATETIME()
            );

            SELECT
                CAST(
                    SCOPE_IDENTITY()
                    AS BIGINT
                );
        ";


                using SqlCommand cmdFactura =
                    new SqlCommand(
                        sqlFactura,
                        con,
                        trans);


                cmdFactura.Parameters.AddWithValue(
                    "@CodigoFactura",
                    codigoFactura);


                cmdFactura.Parameters.AddWithValue(
                    "@IdPaciente",
                    episodio.IdPaciente);


                cmdFactura.Parameters.AddWithValue(
                    "@IdOrigen",
                    episodio.IdOrigen);


                cmdFactura.Parameters.AddWithValue(
                    "@CodigoOrigen",
                    episodio.CodigoOrigen);


                cmdFactura.Parameters.AddWithValue(
                    "@IdSeguro",
                    episodio.IdSeguro.HasValue

                        ? episodio.IdSeguro.Value

                        : DBNull.Value);


                cmdFactura.Parameters.AddWithValue(
                    "@IdPlanTarifario",
                    episodio.IdPlanTarifario.HasValue

                        ? episodio.IdPlanTarifario.Value

                        : DBNull.Value);


                cmdFactura.Parameters.AddWithValue(
                    "@NumeroAutorizacion",

                    string.IsNullOrWhiteSpace(
                        numeroAutorizacion)

                        ? DBNull.Value

                        : numeroAutorizacion.Trim());


                cmdFactura.Parameters.AddWithValue(
                    "@TotalParticular",
                    totalParticular);


                cmdFactura.Parameters.AddWithValue(
                    "@TotalTarifa",
                    totalTarifa);


                cmdFactura.Parameters.AddWithValue(
                    "@TotalSeguro",
                    totalSeguro);


                cmdFactura.Parameters.AddWithValue(
                    "@TotalPaciente",
                    totalPaciente);


                cmdFactura.Parameters.AddWithValue(
                    "@IdEstadoFactura",
                    idEstadoFactura);


                long idFactura =
                    Convert.ToInt64(
                        cmdFactura.ExecuteScalar());


                // ====================================================
                // CARGOS + DETALLES
                // ====================================================

                foreach (DetalleFacturable detalle
                         in detalles)
                {
                    string autorizacionDetalle =

                        detalle.RequiereAutorizacion

                            ? numeroAutorizacion.Trim()

                            : "";


                    // =================================================
                    // CARGO
                    // =================================================

                    string sqlCargo = @"
                INSERT INTO tbCargosPaciente
                (
                    IdPaciente,

                    TipoOrigen,
                    SubtipoAmbulatorio,
                    IdOrigen,
                    CodigoOrigen,
                    IdDetalleOrigen,

                    IdItemClinico,
                    Descripcion,
                    Cantidad,

                    PrecioParticularUnitario,
                    TotalParticular,

                    PrecioTarifaUnitario,
                    TotalTarifa,

                    PorcentajeCobertura,
                    CopagoFijoAplicado,

                    MontoSeguro,
                    MontoPaciente,

                    IdSeguro,
                    IdPlanTarifario,

                    RequiereAutorizacion,
                    NumeroAutorizacion,

                    Estado,
                    FechaCargo,

                    Activo,
                    FechaCreacion
                )

                VALUES
                (
                    @IdPaciente,

                    'Internamiento',
                    NULL,
                    @IdOrigen,
                    @CodigoOrigen,
                    @IdDetalleOrigen,

                    @IdItemClinico,
                    @Descripcion,
                    @Cantidad,

                    @PrecioParticular,
                    @TotalParticular,

                    @PrecioTarifa,
                    @TotalTarifa,

                    @Cobertura,
                    @Copago,

                    @MontoSeguro,
                    @MontoPaciente,

                    @IdSeguro,
                    @IdPlanTarifario,

                    @RequiereAutorizacion,
                    @NumeroAutorizacion,

                    'Facturado',
                    @FechaCargo,

                    1,
                    SYSDATETIME()
                );

                SELECT
                    CAST(
                        SCOPE_IDENTITY()
                        AS BIGINT
                    );
            ";


                    using SqlCommand cmdCargo =
                        new SqlCommand(
                            sqlCargo,
                            con,
                            trans);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdPaciente",
                        episodio.IdPaciente);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdOrigen",
                        episodio.IdOrigen);


                    cmdCargo.Parameters.AddWithValue(
                        "@CodigoOrigen",
                        episodio.CodigoOrigen);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdDetalleOrigen",

                        detalle.IdDetalleOrigen.HasValue

                            ? detalle.IdDetalleOrigen.Value

                            : DBNull.Value);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdItemClinico",

                        detalle.IdItemClinico.HasValue

                            ? detalle.IdItemClinico.Value

                            : DBNull.Value);


                    cmdCargo.Parameters.AddWithValue(
                        "@Descripcion",
                        detalle.Descripcion);


                    cmdCargo.Parameters.AddWithValue(
                        "@Cantidad",
                        detalle.Cantidad);


                    cmdCargo.Parameters.AddWithValue(
                        "@PrecioParticular",
                        detalle.PrecioParticularUnitario);


                    cmdCargo.Parameters.AddWithValue(
                        "@TotalParticular",
                        detalle.TotalParticular);


                    cmdCargo.Parameters.AddWithValue(
                        "@PrecioTarifa",
                        detalle.PrecioTarifaUnitario);


                    cmdCargo.Parameters.AddWithValue(
                        "@TotalTarifa",
                        detalle.TotalTarifa);


                    cmdCargo.Parameters.AddWithValue(
                        "@Cobertura",
                        detalle.PorcentajeCobertura);


                    cmdCargo.Parameters.AddWithValue(
                        "@Copago",
                        detalle.CopagoFijo);


                    cmdCargo.Parameters.AddWithValue(
                        "@MontoSeguro",
                        detalle.MontoSeguro);


                    cmdCargo.Parameters.AddWithValue(
                        "@MontoPaciente",
                        detalle.MontoPaciente);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdSeguro",

                        episodio.IdSeguro.HasValue

                            ? episodio.IdSeguro.Value

                            : DBNull.Value);


                    cmdCargo.Parameters.AddWithValue(
                        "@IdPlanTarifario",

                        detalle.IdPlanTarifario.HasValue

                            ? detalle.IdPlanTarifario.Value

                            : DBNull.Value);


                    cmdCargo.Parameters.AddWithValue(
                        "@RequiereAutorizacion",
                        detalle.RequiereAutorizacion);


                    cmdCargo.Parameters.AddWithValue(
                        "@NumeroAutorizacion",

                        string.IsNullOrWhiteSpace(
                            autorizacionDetalle)

                            ? DBNull.Value

                            : autorizacionDetalle);


                    cmdCargo.Parameters.AddWithValue(
                        "@FechaCargo",
                        detalle.FechaServicio);


                    long idCargo =
                        Convert.ToInt64(
                            cmdCargo.ExecuteScalar());


                    // =================================================
                    // DETALLE
                    // =================================================

                    string sqlDetalle = @"
                INSERT INTO tbFacturaDetalles
                (
                    IdFactura,
                    IdCargo,
                    IdItemClinico,

                    TipoItem,
                    Descripcion,
                    Cantidad,

                    PrecioParticularUnitario,
                    TotalParticular,

                    PrecioTarifaUnitario,
                    TotalTarifa,

                    PorcentajeCobertura,
                    CopagoFijoAplicado,

                    MontoSeguro,
                    MontoPaciente,

                    RequiereAutorizacion,
                    NumeroAutorizacion,

                    FechaCreacion
                )

                VALUES
                (
                    @IdFactura,
                    @IdCargo,
                    @IdItemClinico,

                    @TipoItem,
                    @Descripcion,
                    @Cantidad,

                    @PrecioParticular,
                    @TotalParticular,

                    @PrecioTarifa,
                    @TotalTarifa,

                    @Cobertura,
                    @Copago,

                    @MontoSeguro,
                    @MontoPaciente,

                    @RequiereAutorizacion,
                    @NumeroAutorizacion,

                    SYSDATETIME()
                );
            ";


                    using SqlCommand cmdDetalle =
                        new SqlCommand(
                            sqlDetalle,
                            con,
                            trans);


                    cmdDetalle.Parameters.AddWithValue(
                        "@IdFactura",
                        idFactura);


                    cmdDetalle.Parameters.AddWithValue(
                        "@IdCargo",
                        idCargo);


                    cmdDetalle.Parameters.AddWithValue(
                        "@IdItemClinico",

                        detalle.IdItemClinico.HasValue

                            ? detalle.IdItemClinico.Value

                            : DBNull.Value);


                    cmdDetalle.Parameters.AddWithValue(
                        "@TipoItem",
                        detalle.TipoItem);


                    cmdDetalle.Parameters.AddWithValue(
                        "@Descripcion",
                        detalle.Descripcion);


                    cmdDetalle.Parameters.AddWithValue(
                        "@Cantidad",
                        detalle.Cantidad);


                    cmdDetalle.Parameters.AddWithValue(
                        "@PrecioParticular",
                        detalle.PrecioParticularUnitario);


                    cmdDetalle.Parameters.AddWithValue(
                        "@TotalParticular",
                        detalle.TotalParticular);


                    cmdDetalle.Parameters.AddWithValue(
                        "@PrecioTarifa",
                        detalle.PrecioTarifaUnitario);


                    cmdDetalle.Parameters.AddWithValue(
                        "@TotalTarifa",
                        detalle.TotalTarifa);


                    cmdDetalle.Parameters.AddWithValue(
                        "@Cobertura",
                        detalle.PorcentajeCobertura);


                    cmdDetalle.Parameters.AddWithValue(
                        "@Copago",
                        detalle.CopagoFijo);


                    cmdDetalle.Parameters.AddWithValue(
                        "@MontoSeguro",
                        detalle.MontoSeguro);


                    cmdDetalle.Parameters.AddWithValue(
                        "@MontoPaciente",
                        detalle.MontoPaciente);


                    cmdDetalle.Parameters.AddWithValue(
                        "@RequiereAutorizacion",
                        detalle.RequiereAutorizacion);


                    cmdDetalle.Parameters.AddWithValue(
                        "@NumeroAutorizacion",

                        string.IsNullOrWhiteSpace(
                            autorizacionDetalle)

                            ? DBNull.Value

                            : autorizacionDetalle);


                    cmdDetalle.ExecuteNonQuery();
                }


                // ====================================================
                // MARCAR CONSUMOS COMO FACTURADOS
                // ====================================================

                string sqlActualizar = @"
            UPDATE tbInternamientoItems

            SET
                Estado = 'Facturado'

            WHERE
                IdInternamiento =
                    @IdInternamiento

                AND Activo = 1

                AND Estado =
                    'Registrado';
        ";


                using SqlCommand cmdActualizar =
                    new SqlCommand(
                        sqlActualizar,
                        con,
                        trans);


                cmdActualizar.Parameters
                    .AddWithValue(
                        "@IdInternamiento",
                        episodio.IdOrigen);


                cmdActualizar.ExecuteNonQuery();


                trans.Commit();


                return idFactura;
            }
            catch
            {
                trans.Rollback();

                throw;
            }
        }
    }

}