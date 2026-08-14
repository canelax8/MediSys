using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;

namespace Proyecto_MediSys.Data
{
    public class ItemClinicoDAO
    {
        private readonly Conexion conexion = new Conexion();


        // ============================================================
        // OBTENER TODOS LOS ITEMS ACTIVOS
        // ============================================================

        public List<ItemClinico> ObtenerTodos()
        {
            List<ItemClinico> lista = new();

            using SqlConnection con = conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                SELECT
                    I.IdItemClinico,
                    I.Codigo,
                    I.Nombre,
                    I.Descripcion,
                    I.IdTipoItem,
                    T.Nombre AS TipoItem,
                    I.Activo,
                    I.FechaCreacion,

                    IM.PrincipioActivo,
                    IM.Concentracion,
                    IM.Presentacion,
                    IM.FormaFarmaceutica

                FROM tbItemsClinicos I

                INNER JOIN tbTiposItemClinico T
                    ON I.IdTipoItem = T.IdTipoItem

                LEFT JOIN tbItemMedicamentos IM
                    ON I.IdItemClinico = IM.IdItemClinico

                WHERE I.Activo = 1
                  AND T.Activo = 1

                ORDER BY
                    T.Nombre,
                    I.Nombre;
            ";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearItem(reader));
            }

            return lista;
        }


        // ============================================================
        // OBTENER ITEMS POR TIPO
        // ============================================================

        public List<ItemClinico> ObtenerPorTipo(string tipo)
        {
            List<ItemClinico> lista = new();

            using SqlConnection con = conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                SELECT
                    I.IdItemClinico,
                    I.Codigo,
                    I.Nombre,
                    I.Descripcion,
                    I.IdTipoItem,
                    T.Nombre AS TipoItem,
                    I.Activo,
                    I.FechaCreacion,

                    IM.PrincipioActivo,
                    IM.Concentracion,
                    IM.Presentacion,
                    IM.FormaFarmaceutica

                FROM tbItemsClinicos I

                INNER JOIN tbTiposItemClinico T
                    ON I.IdTipoItem = T.IdTipoItem

                LEFT JOIN tbItemMedicamentos IM
                    ON I.IdItemClinico = IM.IdItemClinico

                WHERE I.Activo = 1
                  AND T.Activo = 1
                  AND T.Nombre = @Tipo

                ORDER BY I.Nombre;
            ";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue(
                "@Tipo",
                tipo);

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearItem(reader));
            }

            return lista;
        }


        // ============================================================
        // BUSCAR POR TIPO
        // ============================================================

        public List<ItemClinico> BuscarPorTipo(
            string tipo,
            string texto)
        {
            List<ItemClinico> lista = new();

            using SqlConnection con = conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                SELECT
                    I.IdItemClinico,
                    I.Codigo,
                    I.Nombre,
                    I.Descripcion,
                    I.IdTipoItem,
                    T.Nombre AS TipoItem,
                    I.Activo,
                    I.FechaCreacion,

                    IM.PrincipioActivo,
                    IM.Concentracion,
                    IM.Presentacion,
                    IM.FormaFarmaceutica

                FROM tbItemsClinicos I

                INNER JOIN tbTiposItemClinico T
                    ON I.IdTipoItem = T.IdTipoItem

                LEFT JOIN tbItemMedicamentos IM
                    ON I.IdItemClinico = IM.IdItemClinico

                WHERE I.Activo = 1
                  AND T.Activo = 1
                  AND T.Nombre = @Tipo

                  AND
                  (
                      I.Codigo LIKE @Texto
                      OR I.Nombre LIKE @Texto
                      OR I.Descripcion LIKE @Texto
                      OR IM.PrincipioActivo LIKE @Texto
                  )

                ORDER BY I.Nombre;
            ";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue(
                "@Tipo",
                tipo);

            cmd.Parameters.AddWithValue(
                "@Texto",
                "%" + texto.Trim() + "%");

            using SqlDataReader reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearItem(reader));
            }

            return lista;
        }


        // ============================================================
        // OBTENER POR ID
        // ============================================================

        public ItemClinico? ObtenerPorId(long idItemClinico)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();

            string sql = @"
                SELECT
                    I.IdItemClinico,
                    I.Codigo,
                    I.Nombre,
                    I.Descripcion,
                    I.IdTipoItem,
                    T.Nombre AS TipoItem,
                    I.Activo,
                    I.FechaCreacion,

                    IM.PrincipioActivo,
                    IM.Concentracion,
                    IM.Presentacion,
                    IM.FormaFarmaceutica

                FROM tbItemsClinicos I

                INNER JOIN tbTiposItemClinico T
                    ON I.IdTipoItem = T.IdTipoItem

                LEFT JOIN tbItemMedicamentos IM
                    ON I.IdItemClinico = IM.IdItemClinico

                WHERE I.IdItemClinico = @IdItemClinico;
            ";

            using SqlCommand cmd =
                new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue(
                "@IdItemClinico",
                idItemClinico);

            using SqlDataReader reader =
                cmd.ExecuteReader();

            if (reader.Read())
            {
                return MapearItem(reader);
            }

            return null;
        }


        // ============================================================
        // MAPEO
        // ============================================================

        private ItemClinico MapearItem(
            SqlDataReader reader)
        {
            return new ItemClinico
            {
                IdItemClinico =
                    Convert.ToInt64(
                        reader["IdItemClinico"]),

                Codigo =
                    reader["Codigo"]
                        .ToString() ?? "",

                Nombre =
                    reader["Nombre"]
                        .ToString() ?? "",

                Descripcion =
                    reader["Descripcion"] == DBNull.Value
                        ? ""
                        : reader["Descripcion"]
                            .ToString() ?? "",

                IdTipoItem =
                    Convert.ToInt32(
                        reader["IdTipoItem"]),

                TipoItem =
                    reader["TipoItem"]
                        .ToString() ?? "",

                Activo =
                    Convert.ToBoolean(
                        reader["Activo"]),

                FechaCreacion =
                    Convert.ToDateTime(
                        reader["FechaCreacion"]),

                PrincipioActivo =
                    reader["PrincipioActivo"] == DBNull.Value
                        ? ""
                        : reader["PrincipioActivo"]
                            .ToString() ?? "",

                Concentracion =
                    reader["Concentracion"] == DBNull.Value
                        ? ""
                        : reader["Concentracion"]
                            .ToString() ?? "",

                Presentacion =
                    reader["Presentacion"] == DBNull.Value
                        ? ""
                        : reader["Presentacion"]
                            .ToString() ?? "",

                FormaFarmaceutica =
                    reader["FormaFarmaceutica"] == DBNull.Value
                        ? ""
                        : reader["FormaFarmaceutica"]
                            .ToString() ?? ""
            };
        }
    }
}