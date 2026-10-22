using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;

namespace Proyecto_MediSys.Data
{
    public class MedicoDAO
    {
        private readonly Conexion conexion =
            new Conexion();


        // ============================================================
        // OBTENER TODOS
        // ============================================================

        public List<Medico> ObtenerTodos()
        {
            List<Medico> lista =
                new List<Medico>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT
                    M.IdMedico,
                    M.CodigoMedico,
                    M.Nombre,
                    M.Apellido,
                    M.Cedula,
                    M.Exequatur,
                    M.Telefono,
                    M.Correo,
                    M.IdEspecialidad,
                    M.Activo,
                    M.FechaCreacion,
                    M.FechaModificacion,

                    E.Nombre AS Especialidad

                FROM tbMedicos M

                LEFT JOIN tbEspecialidades E
                    ON M.IdEspecialidad =
                       E.IdEspecialidad

                ORDER BY
                    M.Activo DESC,
                    M.Nombre,
                    M.Apellido;
            ";


            using SqlCommand cmd =
                new SqlCommand(sql, con);


            using SqlDataReader dr =
                cmd.ExecuteReader();


            while (dr.Read())
            {
                lista.Add(
                    new Medico
                    {
                        IdMedico =
                            Convert.ToInt64(
                                dr["IdMedico"]),

                        CodigoMedico =
                            dr["CodigoMedico"]
                                .ToString() ?? "",

                        Nombre =
                            dr["Nombre"]
                                .ToString() ?? "",

                        Apellido =
                            dr["Apellido"]
                                .ToString() ?? "",

                        Cedula =
                            dr["Cedula"]
                                .ToString() ?? "",

                        Exequatur =
                            dr["Exequatur"]
                                .ToString() ?? "",

                        Telefono =
                            dr["Telefono"] == DBNull.Value
                                ? ""
                                : dr["Telefono"].ToString() ?? "",

                        Correo =
                            dr["Correo"] == DBNull.Value
                                ? ""
                                : dr["Correo"].ToString() ?? "",

                        IdEspecialidad =
                            dr["IdEspecialidad"] == DBNull.Value
                                ? null
                                : Convert.ToInt64(
                                    dr["IdEspecialidad"]),

                        NombreEspecialidad =
                            dr["Especialidad"] == DBNull.Value
                                ? "Sin especialidad"
                                : dr["Especialidad"]
                                    .ToString() ?? "",

                        Activo =
                            Convert.ToBoolean(
                                dr["Activo"]),

                        FechaCreacion =
                            Convert.ToDateTime(
                                dr["FechaCreacion"]),

                        FechaModificacion =
                            dr["FechaModificacion"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(
                                    dr["FechaModificacion"])
                    });
            }


            return lista;
        }


        // ============================================================
        // ESPECIALIDADES
        // ============================================================

        public List<EspecialidadMedicoOpcion>
            ObtenerEspecialidades()
        {
            List<EspecialidadMedicoOpcion> lista =
                new List<EspecialidadMedicoOpcion>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT
                    IdEspecialidad,
                    CodigoEspecialidad,
                    Nombre,
                    Descripcion

                FROM tbEspecialidades

                WHERE Activo = 1

                ORDER BY Nombre;
            ";


            using SqlCommand cmd =
                new SqlCommand(sql, con);


            using SqlDataReader dr =
                cmd.ExecuteReader();


            while (dr.Read())
            {
                lista.Add(
                    new EspecialidadMedicoOpcion
                    {
                        IdEspecialidad =
                            Convert.ToInt64(
                                dr["IdEspecialidad"]),

                        CodigoEspecialidad =
                            dr["CodigoEspecialidad"]
                                .ToString() ?? "",

                        Nombre =
                            dr["Nombre"]
                                .ToString() ?? "",

                        Descripcion =
                            dr["Descripcion"] == DBNull.Value
                                ? ""
                                : dr["Descripcion"]
                                    .ToString() ?? ""
                    });
            }


            return lista;
        }


        // ============================================================
        // VALIDAR CÉDULA
        // ============================================================

        public bool ExisteCedula(
            string cedula,
            long? idExcluir = null)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT COUNT(*)

                FROM tbMedicos

                WHERE
                    Cedula = @Cedula

                    AND
                    (
                        @IdExcluir IS NULL
                        OR IdMedico <> @IdExcluir
                    );
            ";


            using SqlCommand cmd =
                new SqlCommand(sql, con);


            cmd.Parameters.AddWithValue(
                "@Cedula",
                cedula);


            cmd.Parameters.AddWithValue(
                "@IdExcluir",
                (object?)idExcluir ??
                DBNull.Value);


            return Convert.ToInt32(
                cmd.ExecuteScalar()) > 0;
        }


        // ============================================================
        // VALIDAR EXEQUÁTUR
        // ============================================================

        public bool ExisteExequatur(
            string exequatur,
            long? idExcluir = null)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT COUNT(*)

                FROM tbMedicos

                WHERE
                    Exequatur = @Exequatur

                    AND
                    (
                        @IdExcluir IS NULL
                        OR IdMedico <> @IdExcluir
                    );
            ";


            using SqlCommand cmd =
                new SqlCommand(sql, con);


            cmd.Parameters.AddWithValue(
                "@Exequatur",
                exequatur);


            cmd.Parameters.AddWithValue(
                "@IdExcluir",
                (object?)idExcluir ??
                DBNull.Value);


            return Convert.ToInt32(
                cmd.ExecuteScalar()) > 0;
        }


        // ============================================================
        // INSERTAR
        // ============================================================

        public bool Insertar(
            Medico medico)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            using SqlTransaction trans =
                con.BeginTransaction();


            try
            {
                // ====================================================
                // GENERAR CÓDIGO
                // ====================================================

                string sqlNumero = @"
                    SELECT
                        ISNULL(MAX(IdMedico), 0) + 1

                    FROM tbMedicos
                    WITH (UPDLOCK, HOLDLOCK);
                ";


                using SqlCommand cmdNumero =
                    new SqlCommand(
                        sqlNumero,
                        con,
                        trans);


                long numero =
                    Convert.ToInt64(
                        cmdNumero.ExecuteScalar());


                medico.CodigoMedico =
                    $"MED{numero:00000000}";


                // ====================================================
                // INSERT
                // ====================================================

                string sql = @"
                    INSERT INTO tbMedicos
                    (
                        CodigoMedico,
                        Nombre,
                        Apellido,
                        Cedula,
                        Exequatur,
                        Telefono,
                        Correo,
                        IdEspecialidad,
                        Activo
                    )
                    VALUES
                    (
                        @CodigoMedico,
                        @Nombre,
                        @Apellido,
                        @Cedula,
                        @Exequatur,
                        @Telefono,
                        @Correo,
                        @IdEspecialidad,
                        @Activo
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
                        con,
                        trans);


                AgregarParametros(
                    cmd,
                    medico);


                medico.IdMedico =
                    Convert.ToInt64(
                        cmd.ExecuteScalar());


                trans.Commit();

                return true;
            }
            catch
            {
                trans.Rollback();

                throw;
            }
        }


        // ============================================================
        // ACTUALIZAR
        // ============================================================

        public bool Actualizar(
            Medico medico)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                UPDATE tbMedicos

                SET
                    Nombre = @Nombre,
                    Apellido = @Apellido,
                    Cedula = @Cedula,
                    Exequatur = @Exequatur,
                    Telefono = @Telefono,
                    Correo = @Correo,
                    IdEspecialidad = @IdEspecialidad,
                    Activo = @Activo,
                    FechaModificacion = SYSDATETIME()

                WHERE
                    IdMedico = @IdMedico;
            ";


            using SqlCommand cmd =
                new SqlCommand(sql, con);


            cmd.Parameters.AddWithValue(
                "@IdMedico",
                medico.IdMedico);


            AgregarParametros(
                cmd,
                medico,
                incluirCodigo: false);


            return cmd.ExecuteNonQuery() > 0;
        }


        // ============================================================
        // ACTIVAR / DESACTIVAR
        // ============================================================

        public bool CambiarEstado(
            long idMedico,
            bool activo)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                UPDATE tbMedicos

                SET
                    Activo = @Activo,
                    FechaModificacion =
                        SYSDATETIME()

                WHERE
                    IdMedico = @IdMedico;
            ";


            using SqlCommand cmd =
                new SqlCommand(sql, con);


            cmd.Parameters.AddWithValue(
                "@IdMedico",
                idMedico);


            cmd.Parameters.AddWithValue(
                "@Activo",
                activo);


            return cmd.ExecuteNonQuery() > 0;
        }


        // ============================================================
        // PARÁMETROS
        // ============================================================

        private void AgregarParametros(
            SqlCommand cmd,
            Medico medico,
            bool incluirCodigo = true)
        {
            if (incluirCodigo)
            {
                cmd.Parameters.AddWithValue(
                    "@CodigoMedico",
                    medico.CodigoMedico);
            }


            cmd.Parameters.AddWithValue(
                "@Nombre",
                medico.Nombre);


            cmd.Parameters.AddWithValue(
                "@Apellido",
                medico.Apellido);


            cmd.Parameters.AddWithValue(
                "@Cedula",
                medico.Cedula);


            cmd.Parameters.AddWithValue(
                "@Exequatur",
                medico.Exequatur);


            cmd.Parameters.AddWithValue(
                "@Telefono",
                string.IsNullOrWhiteSpace(
                    medico.Telefono)
                    ? DBNull.Value
                    : medico.Telefono);


            cmd.Parameters.AddWithValue(
                "@Correo",
                string.IsNullOrWhiteSpace(
                    medico.Correo)
                    ? DBNull.Value
                    : medico.Correo);


            cmd.Parameters.AddWithValue(
                "@IdEspecialidad",
                medico.IdEspecialidad.HasValue
                    ? medico.IdEspecialidad.Value
                    : DBNull.Value);


            cmd.Parameters.AddWithValue(
                "@Activo",
                medico.Activo);
        }
    }
}