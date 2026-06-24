using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Proyecto_MediSys.Data
{
    class UsuarioDAO
    {
        Conexion conexion = new Conexion();

        public bool ValidarLogin(string usuario, string clave)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                string query = @"SELECT COUNT(*)
                                 FROM tbUsuarios
                                 WHERE Usuario = @usuario
                                 AND ClaveHash = @clave
                                 AND Activo = 1";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@usuario", usuario);
                cmd.Parameters.AddWithValue("@clave", clave);

                int cantidad = (int)cmd.ExecuteScalar();

                return cantidad > 0;
            }
        }
    }
}
