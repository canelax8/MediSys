using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;

namespace Proyecto_MediSys.Data
{
    public class CIE10DAO
    {
        private readonly Conexion conexion = new Conexion();


        // ============================================================
        // OBTENER TODOS
        // ============================================================

        public List<CIE10> ObtenerTodos()
        {
            List<CIE10> lista = new();

            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT
                        IdCIE10,
                        Codigo,
                        Descripcion,
                        Categoria,
                        Activo,
                        FechaCreacion
                    FROM tbCIE10
                    WHERE Activo = 1
                    ORDER BY Codigo;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(
                            new CIE10
                            {
                                IdCIE10 =
                                    Convert.ToInt64(reader["IdCIE10"]),

                                Codigo =
                                    reader["Codigo"].ToString() ?? "",

                                Descripcion =
                                    reader["Descripcion"].ToString() ?? "",

                                Categoria =
                                    reader["Categoria"] == DBNull.Value
                                    ? ""
                                    : reader["Categoria"].ToString() ?? "",

                                Activo =
                                    Convert.ToBoolean(reader["Activo"]),

                                FechaCreacion =
                                    Convert.ToDateTime(reader["FechaCreacion"])
                            });
                    }
                }
            }

            return lista;
        }


        // ============================================================
        // BUSCAR
        // ============================================================

        public List<CIE10> Buscar(string texto)
        {
            List<CIE10> lista = new();

            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                    SELECT
                        IdCIE10,
                        Codigo,
                        Descripcion,
                        Categoria,
                        Activo,
                        FechaCreacion

                    FROM tbCIE10

                    WHERE Activo = 1
                      AND
                      (
                          Codigo LIKE @Texto
                          OR Descripcion LIKE @Texto
                          OR Categoria LIKE @Texto
                      )

                    ORDER BY Codigo;
                ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue(
                        "@Texto",
                        "%" + texto.Trim() + "%");

                    using (SqlDataReader reader =
                           cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(
                                new CIE10
                                {
                                    IdCIE10 =
                                        Convert.ToInt64(
                                            reader["IdCIE10"]),

                                    Codigo =
                                        reader["Codigo"]
                                            .ToString() ?? "",

                                    Descripcion =
                                        reader["Descripcion"]
                                            .ToString() ?? "",

                                    Categoria =
                                        reader["Categoria"]
                                            == DBNull.Value
                                        ? ""
                                        : reader["Categoria"]
                                            .ToString() ?? "",

                                    Activo =
                                        Convert.ToBoolean(
                                            reader["Activo"]),

                                    FechaCreacion =
                                        Convert.ToDateTime(
                                            reader["FechaCreacion"])
                                });
                        }
                    }
                }
            }

            return lista;
        }
    }
}