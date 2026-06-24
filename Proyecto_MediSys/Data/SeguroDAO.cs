using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System.Collections.Generic;

namespace Proyecto_MediSys.Data
{
    internal class SeguroDAO
    {
        private readonly Conexion conexion = new Conexion();
        public List<Seguro> ObtenerTodos()
        {
            List<Seguro> lista = new List<Seguro>();

            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();
                string sql = @"SELECT
                               IdSeguro,
                               CodigoSeguro,
                               Nombre,
                               Descripcion
                               FROM tbSeguros
                               WHERE Activo = 1
                               ORDER BY Nombre";
                
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();
                
                while (dr.Read())
                {
                    lista.Add(new Seguro
                    {
                        IdSeguro = (int)dr["IdSeguro"],
                        CodigoSeguro = dr["CodigoSeguro"].ToString()!,
                        Nombre = dr["Nombre"].ToString()!,
                        Descripcion = dr["Descripcion"].ToString()!
                    });
                }
            }
            return lista;
        }
    }
}
