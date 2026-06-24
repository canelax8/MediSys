using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System.Collections.Generic;

namespace Proyecto_MediSys.Data
{
    class EstadoPacienteDAO
    {
        private readonly Conexion conexion = new Conexion();

        public List<EstadoPaciente> ObtenerTodos()
        {
            List<EstadoPaciente> lista = new List<EstadoPaciente>();

            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                string sql = @"SELECT
                                    IdEstadoPaciente,
                                    CodigoEstadoPaciente,
                                    Nombre,
                                    Descripcion
                               FROM tbEstadoPaciente
                               WHERE Activo = 1
                               ORDER BY Nombre";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new EstadoPaciente
                    {
                        IdEstadoPaciente = (int)dr["IdEstadoPaciente"],
                        CodigoEstadoPaciente = dr["CodigoEstadoPaciente"].ToString()!,
                        Nombre = dr["Nombre"].ToString()!,
                        Descripcion = dr["Descripcion"].ToString()!
                    });
                }
            }

            return lista;
        }
    }
}