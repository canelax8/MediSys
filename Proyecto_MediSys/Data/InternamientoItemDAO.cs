using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Proyecto_MediSys.Data
{
    public class InternamientoItemDAO
    {
        private readonly Conexion conexion =
            new Conexion();


        // ============================================================
        // OBTENER CONSUMOS DEL INTERNAMIENTO
        // ============================================================

        public List<InternamientoItem>
            ObtenerPorInternamiento(
                long idInternamiento)
        {
            List<InternamientoItem> lista =
                new List<InternamientoItem>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT

                    II.IdInternamientoItem,
                    II.IdInternamiento,
                    II.IdItemClinico,

                    I.Codigo,
                    I.Nombre,

                    TI.Nombre AS TipoItem,

                    II.Cantidad,
                    II.PrecioUnitarioAplicado,
                    II.IdPlanTarifarioAplicado,

                    ISNULL(
                        PT.Nombre,
                        ''
                    ) AS NombrePlanTarifario,

                    II.Estado,

                    ISNULL(
                        II.Observaciones,
                        ''
                    ) AS Observaciones,

                    II.FechaRegistro,
                    II.Activo

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

                WHERE
                    II.IdInternamiento =
                        @IdInternamiento

                    AND II.Activo = 1

                ORDER BY
                    II.FechaRegistro DESC,
                    II.IdInternamientoItem DESC;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.Add(
                "@IdInternamiento",
                SqlDbType.BigInt)
                .Value =
                    idInternamiento;


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                InternamientoItem item =
                    new InternamientoItem
                    {
                        IdInternamientoItem =
                            Convert.ToInt64(
                                reader[
                                    "IdInternamientoItem"
                                ]),

                        IdInternamiento =
                            Convert.ToInt64(
                                reader[
                                    "IdInternamiento"
                                ]),

                        IdItemClinico =
                            Convert.ToInt64(
                                reader[
                                    "IdItemClinico"
                                ]),

                        Codigo =
                            reader["Codigo"]
                                .ToString() ?? "",

                        Nombre =
                            reader["Nombre"]
                                .ToString() ?? "",

                        TipoItem =
                            reader["TipoItem"]
                                .ToString() ?? "",

                        Cantidad =
                            Convert.ToDecimal(
                                reader["Cantidad"]),

                        PrecioUnitarioAplicado =
                            Convert.ToDecimal(
                                reader[
                                    "PrecioUnitarioAplicado"
                                ]),

                        IdPlanTarifarioAplicado =
                            reader[
                                "IdPlanTarifarioAplicado"
                            ] == DBNull.Value

                                ? null

                                : Convert.ToInt32(
                                    reader[
                                        "IdPlanTarifarioAplicado"
                                    ]),

                        NombrePlanTarifario =
                            reader[
                                "NombrePlanTarifario"
                            ].ToString() ?? "",

                        Estado =
                            reader["Estado"]
                                .ToString() ?? "",

                        Observaciones =
                            reader["Observaciones"]
                                .ToString() ?? "",

                        FechaRegistro =
                            Convert.ToDateTime(
                                reader["FechaRegistro"]),

                        Activo =
                            Convert.ToBoolean(
                                reader["Activo"])
                    };


                lista.Add(item);
            }


            return lista;
        }


        // ============================================================
        // INSERTAR CONSUMO
        // ============================================================

        public long Insertar(
            InternamientoItem item)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            // ========================================================
            // COMPROBAR QUE EL INTERNAMIENTO EXISTE
            // ========================================================

            string sqlExiste = @"
                SELECT COUNT(*)

                FROM tbInternamientos

                WHERE
                    IdInternamiento =
                        @IdInternamiento

                    AND Activo = 1;
            ";


            using SqlCommand cmdExiste =
                new SqlCommand(
                    sqlExiste,
                    con);


            cmdExiste.Parameters.Add(
                "@IdInternamiento",
                SqlDbType.BigInt)
                .Value =
                    item.IdInternamiento;


            int existe =
                Convert.ToInt32(
                    cmdExiste.ExecuteScalar());


            if (existe == 0)
            {
                throw new Exception(
                    "El internamiento no existe o no está activo.");
            }


            // ========================================================
            // INSERTAR
            // ========================================================

            string sql = @"
                INSERT INTO tbInternamientoItems
                (
                    IdInternamiento,
                    IdItemClinico,
                    Cantidad,

                    PrecioUnitarioAplicado,
                    IdPlanTarifarioAplicado,

                    Estado,
                    Observaciones,

                    FechaRegistro,
                    Activo
                )

                VALUES
                (
                    @IdInternamiento,
                    @IdItemClinico,
                    @Cantidad,

                    @PrecioUnitarioAplicado,
                    @IdPlanTarifarioAplicado,

                    'Registrado',
                    @Observaciones,

                    SYSDATETIME(),
                    1
                );

                SELECT
                    CAST(
                        SCOPE_IDENTITY()
                        AS BIGINT
                    );
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.Add(
                "@IdInternamiento",
                SqlDbType.BigInt)
                .Value =
                    item.IdInternamiento;


            cmd.Parameters.Add(
                "@IdItemClinico",
                SqlDbType.BigInt)
                .Value =
                    item.IdItemClinico;


            cmd.Parameters.Add(
                "@Cantidad",
                SqlDbType.Decimal);

            cmd.Parameters[
                "@Cantidad"
            ].Precision = 12;

            cmd.Parameters[
                "@Cantidad"
            ].Scale = 2;

            cmd.Parameters[
                "@Cantidad"
            ].Value =
                item.Cantidad;


            cmd.Parameters.Add(
                "@PrecioUnitarioAplicado",
                SqlDbType.Decimal);

            cmd.Parameters[
                "@PrecioUnitarioAplicado"
            ].Precision = 18;

            cmd.Parameters[
                "@PrecioUnitarioAplicado"
            ].Scale = 2;

            cmd.Parameters[
                "@PrecioUnitarioAplicado"
            ].Value =
                item.PrecioUnitarioAplicado;


            cmd.Parameters.Add(
                "@IdPlanTarifarioAplicado",
                SqlDbType.Int)
                .Value =
                    item.IdPlanTarifarioAplicado.HasValue

                        ? item.IdPlanTarifarioAplicado.Value

                        : DBNull.Value;


            cmd.Parameters.Add(
                "@Observaciones",
                SqlDbType.VarChar,
                500)
                .Value =
                    string.IsNullOrWhiteSpace(
                        item.Observaciones)

                        ? DBNull.Value

                        : item.Observaciones.Trim();


            return Convert.ToInt64(
                cmd.ExecuteScalar());
        }


        // ============================================================
        // ANULAR CONSUMO
        // ============================================================

        public bool Anular(
            long idInternamientoItem)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            // ========================================================
            // NO PERMITIR ELIMINAR ALGO YA FACTURADO
            // ========================================================

            string sqlEstado = @"
                SELECT TOP 1
                    Estado

                FROM tbInternamientoItems

                WHERE
                    IdInternamientoItem =
                        @IdInternamientoItem

                    AND Activo = 1;
            ";


            using SqlCommand cmdEstado =
                new SqlCommand(
                    sqlEstado,
                    con);


            cmdEstado.Parameters.Add(
                "@IdInternamientoItem",
                SqlDbType.BigInt)
                .Value =
                    idInternamientoItem;


            object? resultado =
                cmdEstado.ExecuteScalar();


            if (resultado == null)
            {
                return false;
            }


            string estado =
                resultado.ToString() ?? "";


            if (estado.Equals(
                "Facturado",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Este consumo ya fue facturado y no puede eliminarse.");
            }


            // ========================================================
            // ANULACIÓN LÓGICA
            // ========================================================

            string sql = @"
                UPDATE tbInternamientoItems

                SET
                    Estado = 'Anulado',
                    Activo = 0

                WHERE
                    IdInternamientoItem =
                        @IdInternamientoItem

                    AND Activo = 1;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.Add(
                "@IdInternamientoItem",
                SqlDbType.BigInt)
                .Value =
                    idInternamientoItem;


            return cmd.ExecuteNonQuery() > 0;
        }
    }
}