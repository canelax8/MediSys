using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System.Collections.Generic;

namespace Proyecto_MediSys.Data
{
    class TipoPacienteDAO
    {
        private readonly Conexion conexion = new Conexion();
        public List<TipoPaciente> ObtenerTodos()
        {
            List<TipoPaciente> lista = new List<TipoPaciente>();

            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();
                string sql = @"SELECT IdTipoPaciente, CodigoTipoPaciente, Nombre, Descripcion, Activo
                               FROM tbTipoPaciente WHERE Activo = 1 ORDER BY Nombre";

                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
{
                    lista.Add(new TipoPaciente
                    {
                        IdTipoPaciente = (int)dr["IdTipoPaciente"],
                        CodigoTipoPaciente = dr["CodigoTipoPaciente"].ToString()!,
                        Nombre = dr["Nombre"].ToString()!,
                        Descripcion = dr["Descripcion"].ToString()!,
                        Activo = (bool)dr["Activo"]
                    });
                }
            }
            return lista;
        }

    }
}