using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;

namespace Proyecto_MediSys.Data
{
    public class TarifaDAO
    {
        private readonly Conexion conexion = new Conexion();


        // ============================================================
        // OBTENER PLAN PRIVADO
        // ============================================================

        public PlanTarifario? ObtenerPlanPrivado()
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                SELECT TOP 1
                    PT.IdPlanTarifario,
                    PT.Nombre,
                    PT.Tipo,
                    PT.IdSeguro,
                    PT.Activo,
                    PT.FechaCreacion

                FROM tbPlanesTarifarios PT

                WHERE PT.Activo = 1
                  AND PT.Tipo = 'PRIVADO'

                ORDER BY PT.IdPlanTarifario;
            ";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            using SqlDataReader reader =
                cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new PlanTarifario
            {
                IdPlanTarifario =
                    Convert.ToInt32(
                        reader["IdPlanTarifario"]),

                Nombre =
                    reader["Nombre"]
                        .ToString() ?? "",

                Tipo =
                    reader["Tipo"]
                        .ToString() ?? "",

                IdSeguro =
                    reader["IdSeguro"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(
                            reader["IdSeguro"]),

                Activo =
                    Convert.ToBoolean(
                        reader["Activo"]),

                FechaCreacion =
                    Convert.ToDateTime(
                        reader["FechaCreacion"])
            };
        }


        // ============================================================
        // OBTENER PLAN SEGÚN SEGURO
        // ============================================================

        public PlanTarifario? ObtenerPlanPorSeguro(
            int idSeguro)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                SELECT TOP 1
                    PT.IdPlanTarifario,
                    PT.Nombre,
                    PT.Tipo,
                    PT.IdSeguro,
                    PT.Activo,
                    PT.FechaCreacion

                FROM tbPlanesTarifarios PT

                WHERE PT.Activo = 1
                  AND PT.IdSeguro = @IdSeguro

                ORDER BY PT.IdPlanTarifario;
            ";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue(
                "@IdSeguro",
                idSeguro);

            using SqlDataReader reader =
                cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new PlanTarifario
            {
                IdPlanTarifario =
                    Convert.ToInt32(
                        reader["IdPlanTarifario"]),

                Nombre =
                    reader["Nombre"]
                        .ToString() ?? "",

                Tipo =
                    reader["Tipo"]
                        .ToString() ?? "",

                IdSeguro =
                    reader["IdSeguro"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(
                            reader["IdSeguro"]),

                Activo =
                    Convert.ToBoolean(
                        reader["Activo"]),

                FechaCreacion =
                    Convert.ToDateTime(
                        reader["FechaCreacion"])
            };
        }


        // ============================================================
        // DETERMINAR PLAN DEL PACIENTE
        // ============================================================

        public PlanTarifario? ObtenerPlanPaciente(
            Paciente paciente)
        {
            /*
             * Si posteriormente usamos una propiedad específica
             * para determinar si el paciente es privado, podemos
             * ajustar esta regla.
             *
             * Por ahora intentamos primero localizar el plan
             * correspondiente al IdSeguro.
             */

            if (paciente.IdSeguro > 0)
            {
                PlanTarifario? planSeguro =
                    ObtenerPlanPorSeguro(
                        paciente.IdSeguro);

                if (planSeguro != null)
                    return planSeguro;
            }

            return ObtenerPlanPrivado();
        }


        // ============================================================
        // OBTENER TARIFA VIGENTE
        // ============================================================

        public TarifaItem? ObtenerTarifaVigente(
            long idItemClinico,
            int idPlanTarifario)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                SELECT TOP 1
                    TF.IdTarifa,
                    TF.IdItemClinico,
                    TF.IdPlanTarifario,
                    TF.Precio,
                    TF.FechaInicio,
                    TF.FechaFin,
                    TF.Activo,
                    TF.FechaCreacion,

                    I.Nombre AS NombreItem,
                    PT.Nombre AS NombrePlan

                FROM tbTarifasItems TF

                INNER JOIN tbItemsClinicos I
                    ON TF.IdItemClinico =
                       I.IdItemClinico

                INNER JOIN tbPlanesTarifarios PT
                    ON TF.IdPlanTarifario =
                       PT.IdPlanTarifario

                WHERE TF.IdItemClinico =
                      @IdItemClinico

                  AND TF.IdPlanTarifario =
                      @IdPlanTarifario

                  AND TF.Activo = 1

                  AND TF.FechaInicio <=
                      CAST(GETDATE() AS DATE)

                  AND
                  (
                      TF.FechaFin IS NULL
                      OR
                      TF.FechaFin >=
                      CAST(GETDATE() AS DATE)
                  )

                ORDER BY TF.FechaInicio DESC;
            ";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue(
                "@IdItemClinico",
                idItemClinico);

            cmd.Parameters.AddWithValue(
                "@IdPlanTarifario",
                idPlanTarifario);

            using SqlDataReader reader =
                cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new TarifaItem
            {
                IdTarifa =
                    Convert.ToInt64(
                        reader["IdTarifa"]),

                IdItemClinico =
                    Convert.ToInt64(
                        reader["IdItemClinico"]),

                IdPlanTarifario =
                    Convert.ToInt32(
                        reader["IdPlanTarifario"]),

                Precio =
                    Convert.ToDecimal(
                        reader["Precio"]),

                FechaInicio =
                    Convert.ToDateTime(
                        reader["FechaInicio"]),

                FechaFin =
                    reader["FechaFin"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(
                            reader["FechaFin"]),

                Activo =
                    Convert.ToBoolean(
                        reader["Activo"]),

                FechaCreacion =
                    Convert.ToDateTime(
                        reader["FechaCreacion"]),

                NombreItem =
                    reader["NombreItem"]
                        .ToString() ?? "",

                NombrePlan =
                    reader["NombrePlan"]
                        .ToString() ?? ""
            };
        }


        // ============================================================
        // OBTENER TARIFA DEL PACIENTE
        // ============================================================

        public TarifaItem? ObtenerTarifaPaciente(
            long idItemClinico,
            Paciente paciente)
        {
            PlanTarifario? plan =
                ObtenerPlanPaciente(paciente);

            if (plan == null)
                return null;

            return ObtenerTarifaVigente(
                idItemClinico,
                plan.IdPlanTarifario);
        }
    }
}