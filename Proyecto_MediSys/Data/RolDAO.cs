using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;

namespace Proyecto_MediSys.Data
{
    class RolDAO
    {
        Conexion conexion = new Conexion();

        public List<Rol> ObtenerTodos()
        {
            List<Rol> lista = new();

            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                    SELECT *
                    FROM tbRoles
                    WHERE Activo = 1
                    ORDER BY Nombre;", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Rol rol = new Rol();

                    rol.IdRol = Convert.ToInt64(reader["IdRol"]);
                    rol.CodigoRol = reader["CodigoRol"].ToString()!;
                    rol.Nombre = reader["Nombre"].ToString()!;
                    rol.Descripcion = reader["Descripcion"].ToString()!;
                    rol.Activo = Convert.ToBoolean(reader["Activo"]);

                    lista.Add(rol);
                }
            }

            return lista;
        }
    }
}