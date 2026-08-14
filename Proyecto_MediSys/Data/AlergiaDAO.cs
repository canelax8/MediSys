using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;

namespace Proyecto_MediSys.Data
{
    public class AlergiaDAO
    {
        private readonly Conexion conexion = new Conexion();


        // ============================================================
        // OBTENER TODAS LAS ALERGIAS DEL CATÁLOGO
        // ============================================================

        public List<Alergia> ObtenerTodas()
        {
            List<Alergia> lista = new();

            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT
                        IdAlergia,
                        Nombre,
                        Descripcion,
                        Activo,
                        FechaCreacion
                    FROM tbAlergias
                    WHERE Activo = 1
                    ORDER BY Nombre;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Alergia alergia = new Alergia
                        {
                            IdAlergia =
                                Convert.ToInt64(reader["IdAlergia"]),

                            Nombre =
                                reader["Nombre"]?.ToString() ?? "",

                            Descripcion =
                                reader["Descripcion"] == DBNull.Value
                                ? ""
                                : reader["Descripcion"].ToString() ?? "",

                            Activo =
                                Convert.ToBoolean(reader["Activo"]),

                            FechaCreacion =
                                Convert.ToDateTime(reader["FechaCreacion"])
                        };

                        lista.Add(alergia);
                    }
                }
            }

            return lista;
        }


        // ============================================================
        // BUSCAR EN CATÁLOGO
        // ============================================================

        public List<Alergia> Buscar(string texto)
        {
            List<Alergia> lista = new();

            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT
                        IdAlergia,
                        Nombre,
                        Descripcion,
                        Activo,
                        FechaCreacion
                    FROM tbAlergias
                    WHERE Activo = 1
                      AND
                      (
                          Nombre LIKE @Texto
                          OR Descripcion LIKE @Texto
                      )
                    ORDER BY Nombre;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@Texto",
                        "%" + texto.Trim() + "%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Alergia alergia = new Alergia
                            {
                                IdAlergia =
                                    Convert.ToInt64(reader["IdAlergia"]),

                                Nombre =
                                    reader["Nombre"]?.ToString() ?? "",

                                Descripcion =
                                    reader["Descripcion"] == DBNull.Value
                                    ? ""
                                    : reader["Descripcion"].ToString() ?? "",

                                Activo =
                                    Convert.ToBoolean(reader["Activo"]),

                                FechaCreacion =
                                    Convert.ToDateTime(reader["FechaCreacion"])
                            };

                            lista.Add(alergia);
                        }
                    }
                }
            }

            return lista;
        }


        // ============================================================
        // OBTENER ALERGIAS DE UN PACIENTE
        // ============================================================

        public List<PacienteAlergia> ObtenerPorPaciente(int idPaciente)
        {
            List<PacienteAlergia> lista = new();

            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT
                        PA.IdPacienteAlergia,
                        PA.IdPaciente,
                        PA.IdAlergia,

                        A.Nombre AS NombreAlergia,

                        PA.AlergiaTexto,
                        PA.Observaciones,
                        PA.Activo,
                        PA.FechaRegistro

                    FROM tbPacienteAlergias PA

                    LEFT JOIN tbAlergias A
                        ON PA.IdAlergia = A.IdAlergia

                    WHERE PA.IdPaciente = @IdPaciente
                      AND PA.Activo = 1

                    ORDER BY PA.FechaRegistro DESC;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdPaciente",
                        idPaciente);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PacienteAlergia alergia =
                                new PacienteAlergia
                                {
                                    IdPacienteAlergia =
                                        Convert.ToInt64(
                                            reader["IdPacienteAlergia"]),

                                    IdPaciente =
                                        Convert.ToInt32(
                                            reader["IdPaciente"]),

                                    IdAlergia =
                                        reader["IdAlergia"] == DBNull.Value
                                        ? null
                                        : Convert.ToInt64(
                                            reader["IdAlergia"]),

                                    NombreAlergia =
                                        reader["NombreAlergia"]
                                            == DBNull.Value
                                        ? ""
                                        : reader["NombreAlergia"]
                                            .ToString() ?? "",

                                    AlergiaTexto =
                                        reader["AlergiaTexto"]
                                            == DBNull.Value
                                        ? ""
                                        : reader["AlergiaTexto"]
                                            .ToString() ?? "",

                                    Observaciones =
                                        reader["Observaciones"]
                                            == DBNull.Value
                                        ? ""
                                        : reader["Observaciones"]
                                            .ToString() ?? "",

                                    Activo =
                                        Convert.ToBoolean(
                                            reader["Activo"]),

                                    FechaRegistro =
                                        Convert.ToDateTime(
                                            reader["FechaRegistro"])
                                };

                            lista.Add(alergia);
                        }
                    }
                }
            }

            return lista;
        }


        // ============================================================
        // ASOCIAR ALERGIA DEL CATÁLOGO AL PACIENTE
        // ============================================================

        public bool AgregarAlPaciente(
            int idPaciente,
            long idAlergia,
            string observaciones = "")
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                // Primero evitamos duplicados
                string sqlExiste = @"
                    SELECT COUNT(*)
                    FROM tbPacienteAlergias
                    WHERE IdPaciente = @IdPaciente
                      AND IdAlergia = @IdAlergia
                      AND Activo = 1;
                ";

                using (SqlCommand cmdExiste =
                       new SqlCommand(sqlExiste, con))
                {
                    cmdExiste.Parameters.AddWithValue(
                        "@IdPaciente",
                        idPaciente);

                    cmdExiste.Parameters.AddWithValue(
                        "@IdAlergia",
                        idAlergia);

                    int cantidad =
                        Convert.ToInt32(
                            cmdExiste.ExecuteScalar());

                    if (cantidad > 0)
                    {
                        return false;
                    }
                }


                string sql = @"
                    INSERT INTO tbPacienteAlergias
                    (
                        IdPaciente,
                        IdAlergia,
                        AlergiaTexto,
                        Observaciones,
                        Activo,
                        FechaRegistro
                    )
                    VALUES
                    (
                        @IdPaciente,
                        @IdAlergia,
                        NULL,
                        @Observaciones,
                        1,
                        GETDATE()
                    );
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdPaciente",
                        idPaciente);

                    cmd.Parameters.AddWithValue(
                        "@IdAlergia",
                        idAlergia);

                    cmd.Parameters.AddWithValue(
                        "@Observaciones",
                        string.IsNullOrWhiteSpace(observaciones)
                            ? DBNull.Value
                            : observaciones.Trim());

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }


        // ============================================================
        // AGREGAR ALERGIA MANUAL AL PACIENTE
        // ============================================================

        public bool AgregarManual(
            int idPaciente,
            string alergiaTexto,
            string observaciones = "")
        {
            if (string.IsNullOrWhiteSpace(alergiaTexto))
                return false;


            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();


                // Evitar repetir exactamente el mismo texto
                string sqlExiste = @"
                    SELECT COUNT(*)
                    FROM tbPacienteAlergias
                    WHERE IdPaciente = @IdPaciente
                      AND AlergiaTexto = @AlergiaTexto
                      AND Activo = 1;
                ";

                using (SqlCommand cmdExiste =
                       new SqlCommand(sqlExiste, con))
                {
                    cmdExiste.Parameters.AddWithValue(
                        "@IdPaciente",
                        idPaciente);

                    cmdExiste.Parameters.AddWithValue(
                        "@AlergiaTexto",
                        alergiaTexto.Trim());

                    int cantidad =
                        Convert.ToInt32(
                            cmdExiste.ExecuteScalar());

                    if (cantidad > 0)
                    {
                        return false;
                    }
                }


                string sql = @"
                    INSERT INTO tbPacienteAlergias
                    (
                        IdPaciente,
                        IdAlergia,
                        AlergiaTexto,
                        Observaciones,
                        Activo,
                        FechaRegistro
                    )
                    VALUES
                    (
                        @IdPaciente,
                        NULL,
                        @AlergiaTexto,
                        @Observaciones,
                        1,
                        GETDATE()
                    );
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdPaciente",
                        idPaciente);

                    cmd.Parameters.AddWithValue(
                        "@AlergiaTexto",
                        alergiaTexto.Trim());

                    cmd.Parameters.AddWithValue(
                        "@Observaciones",
                        string.IsNullOrWhiteSpace(observaciones)
                            ? DBNull.Value
                            : observaciones.Trim());

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }


        // ============================================================
        // DESACTIVAR / QUITAR ALERGIA DEL PACIENTE
        // ============================================================

        public bool QuitarDelPaciente(long idPacienteAlergia)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    UPDATE tbPacienteAlergias
                    SET Activo = 0
                    WHERE IdPacienteAlergia = @IdPacienteAlergia;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdPacienteAlergia",
                        idPacienteAlergia);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }


        // ============================================================
        // SABER SI UN PACIENTE TIENE ALERGIAS
        // ============================================================

        public bool TieneAlergias(int idPaciente)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT COUNT(*)
                    FROM tbPacienteAlergias
                    WHERE IdPaciente = @IdPaciente
                      AND Activo = 1;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdPaciente",
                        idPaciente);

                    int cantidad =
                        Convert.ToInt32(
                            cmd.ExecuteScalar());

                    return cantidad > 0;
                }
            }
        }


        // ============================================================
        // CONTAR ALERGIAS DEL PACIENTE
        // ============================================================

        public int ContarAlergias(int idPaciente)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT COUNT(*)
                    FROM tbPacienteAlergias
                    WHERE IdPaciente = @IdPaciente
                      AND Activo = 1;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@IdPaciente",
                        idPaciente);

                    return Convert.ToInt32(
                        cmd.ExecuteScalar());
                }
            }
        }
    }
}