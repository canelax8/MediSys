using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;

namespace Proyecto_MediSys.Data
{
    public class EmergenciaDiagnosticoDAO
    {
        private readonly Conexion conexion = new Conexion();


        // ============================================================
        // AGREGAR DIAGNÓSTICO DEL CATÁLOGO
        // ============================================================

        public bool Agregar(
            long idEmergencia,
            long idCIE10,
            bool esPrincipal,
            string observaciones = "")
        {
            using SqlConnection con = conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                INSERT INTO tbEmergenciaDiagnosticos
                (
                    IdEmergencia,
                    IdCIE10,
                    DiagnosticoTexto,
                    EsPrincipal,
                    Observaciones,
                    Activo,
                    FechaRegistro
                )
                VALUES
                (
                    @IdEmergencia,
                    @IdCIE10,
                    NULL,
                    @EsPrincipal,
                    @Observaciones,
                    1,
                    GETDATE()
                );
            ";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue(
                "@IdEmergencia",
                idEmergencia);

            cmd.Parameters.AddWithValue(
                "@IdCIE10",
                idCIE10);

            cmd.Parameters.AddWithValue(
                "@EsPrincipal",
                esPrincipal);

            cmd.Parameters.AddWithValue(
                "@Observaciones",
                string.IsNullOrWhiteSpace(observaciones)
                    ? DBNull.Value
                    : observaciones.Trim());

            return cmd.ExecuteNonQuery() > 0;
        }


        // ============================================================
        // AGREGAR DIAGNÓSTICO MANUAL
        // ============================================================

        public bool AgregarManual(
            long idEmergencia,
            string diagnostico,
            bool esPrincipal,
            string observaciones = "")
        {
            if (string.IsNullOrWhiteSpace(diagnostico))
                return false;

            using SqlConnection con = conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                INSERT INTO tbEmergenciaDiagnosticos
                (
                    IdEmergencia,
                    IdCIE10,
                    DiagnosticoTexto,
                    EsPrincipal,
                    Observaciones,
                    Activo,
                    FechaRegistro
                )
                VALUES
                (
                    @IdEmergencia,
                    NULL,
                    @DiagnosticoTexto,
                    @EsPrincipal,
                    @Observaciones,
                    1,
                    GETDATE()
                );
            ";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue(
                "@IdEmergencia",
                idEmergencia);

            cmd.Parameters.AddWithValue(
                "@DiagnosticoTexto",
                diagnostico.Trim());

            cmd.Parameters.AddWithValue(
                "@EsPrincipal",
                esPrincipal);

            cmd.Parameters.AddWithValue(
                "@Observaciones",
                string.IsNullOrWhiteSpace(observaciones)
                    ? DBNull.Value
                    : observaciones.Trim());

            return cmd.ExecuteNonQuery() > 0;
        }


        // ============================================================
        // OBTENER DIAGNÓSTICOS DE UNA EMERGENCIA
        // ============================================================

        public List<EmergenciaDiagnostico> ObtenerPorEmergencia(
            long idEmergencia)
        {
            List<EmergenciaDiagnostico> lista = new();

            using SqlConnection con = conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                SELECT
                    ED.IdEmergenciaDiagnostico,
                    ED.IdEmergencia,
                    ED.IdCIE10,

                    C.Codigo AS CodigoCIE10,
                    C.Descripcion AS DescripcionCIE10,

                    ED.DiagnosticoTexto,
                    ED.EsPrincipal,
                    ED.Observaciones,
                    ED.Activo,
                    ED.FechaRegistro

                FROM tbEmergenciaDiagnosticos ED

                LEFT JOIN tbCIE10 C
                    ON ED.IdCIE10 = C.IdCIE10

                WHERE ED.IdEmergencia = @IdEmergencia
                  AND ED.Activo = 1

                ORDER BY
                    ED.EsPrincipal DESC,
                    ED.FechaRegistro ASC;
            ";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue(
                "@IdEmergencia",
                idEmergencia);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                EmergenciaDiagnostico diagnostico = new()
                {
                    IdEmergenciaDiagnostico =
                        Convert.ToInt64(
                            reader["IdEmergenciaDiagnostico"]),

                    IdEmergencia =
                        Convert.ToInt64(
                            reader["IdEmergencia"]),

                    IdCIE10 =
                        reader["IdCIE10"] == DBNull.Value
                            ? null
                            : Convert.ToInt64(
                                reader["IdCIE10"]),

                    CodigoCIE10 =
                        reader["CodigoCIE10"] == DBNull.Value
                            ? ""
                            : reader["CodigoCIE10"].ToString() ?? "",

                    DescripcionCIE10 =
                        reader["DescripcionCIE10"] == DBNull.Value
                            ? ""
                            : reader["DescripcionCIE10"].ToString() ?? "",

                    DiagnosticoTexto =
                        reader["DiagnosticoTexto"] == DBNull.Value
                            ? ""
                            : reader["DiagnosticoTexto"].ToString() ?? "",

                    EsPrincipal =
                        Convert.ToBoolean(
                            reader["EsPrincipal"]),

                    Observaciones =
                        reader["Observaciones"] == DBNull.Value
                            ? ""
                            : reader["Observaciones"].ToString() ?? "",

                    Activo =
                        Convert.ToBoolean(
                            reader["Activo"]),

                    FechaRegistro =
                        Convert.ToDateTime(
                            reader["FechaRegistro"])
                };

                lista.Add(diagnostico);
            }

            return lista;
        }


        // ============================================================
        // QUITAR / DESACTIVAR
        // ============================================================

        public bool Quitar(long idEmergenciaDiagnostico)
        {
            using SqlConnection con = conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                UPDATE tbEmergenciaDiagnosticos

                SET Activo = 0

                WHERE IdEmergenciaDiagnostico =
                      @IdEmergenciaDiagnostico;
            ";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue(
                "@IdEmergenciaDiagnostico",
                idEmergenciaDiagnostico);

            return cmd.ExecuteNonQuery() > 0;
        }


        // ============================================================
        // HISTORIAL DIAGNÓSTICO DEL PACIENTE
        // ============================================================

        public List<EmergenciaDiagnostico> ObtenerPorPaciente(
            int idPaciente)
        {
            List<EmergenciaDiagnostico> lista = new();

            using SqlConnection con = conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                SELECT
                    ED.IdEmergenciaDiagnostico,
                    ED.IdEmergencia,
                    ED.IdCIE10,

                    C.Codigo AS CodigoCIE10,
                    C.Descripcion AS DescripcionCIE10,

                    ED.DiagnosticoTexto,
                    ED.EsPrincipal,
                    ED.Observaciones,
                    ED.Activo,
                    ED.FechaRegistro

                FROM tbEmergenciaDiagnosticos ED

                INNER JOIN tbEmergencias E
                    ON ED.IdEmergencia = E.IdEmergencia

                LEFT JOIN tbCIE10 C
                    ON ED.IdCIE10 = C.IdCIE10

                WHERE E.IdPaciente = @IdPaciente
                  AND ED.Activo = 1

                ORDER BY
                    E.FechaIngreso DESC,
                    ED.EsPrincipal DESC;
            ";

            using SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue(
                "@IdPaciente",
                idPaciente);

            using SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                EmergenciaDiagnostico diagnostico = new()
                {
                    IdEmergenciaDiagnostico =
                        Convert.ToInt64(
                            reader["IdEmergenciaDiagnostico"]),

                    IdEmergencia =
                        Convert.ToInt64(
                            reader["IdEmergencia"]),

                    IdCIE10 =
                        reader["IdCIE10"] == DBNull.Value
                            ? null
                            : Convert.ToInt64(
                                reader["IdCIE10"]),

                    CodigoCIE10 =
                        reader["CodigoCIE10"] == DBNull.Value
                            ? ""
                            : reader["CodigoCIE10"].ToString() ?? "",

                    DescripcionCIE10 =
                        reader["DescripcionCIE10"] == DBNull.Value
                            ? ""
                            : reader["DescripcionCIE10"].ToString() ?? "",

                    DiagnosticoTexto =
                        reader["DiagnosticoTexto"] == DBNull.Value
                            ? ""
                            : reader["DiagnosticoTexto"].ToString() ?? "",

                    EsPrincipal =
                        Convert.ToBoolean(
                            reader["EsPrincipal"]),

                    Observaciones =
                        reader["Observaciones"] == DBNull.Value
                            ? ""
                            : reader["Observaciones"].ToString() ?? "",

                    Activo =
                        Convert.ToBoolean(
                            reader["Activo"]),

                    FechaRegistro =
                        Convert.ToDateTime(
                            reader["FechaRegistro"])
                };

                lista.Add(diagnostico);
            }

            return lista;
        }
    }
}