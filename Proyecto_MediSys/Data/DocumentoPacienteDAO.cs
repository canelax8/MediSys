using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
//using Proyecto_MediSys.Archivos.Pacientes;
using System.Data;
using System.Text;
using System.Windows;

namespace Proyecto_MediSys.Data
{
    internal class DocumentoPacienteDAO
    {
        private readonly Conexion conexion = new Conexion();
        public bool Insertar(DocumentoPaciente documento)
        {
            try
            {
                using (SqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"
                INSERT INTO tbDocumentosPaciente
                (
                    IdPaciente,
                    TipoDocumento,
                    NombreArchivo,
                    RutaArchivo,
                    Extension,
                    TamanoKB,
                    Activo
                )
                VALUES
                (
                    @IdPaciente,
                    @TipoDocumento,
                    @NombreArchivo,
                    @RutaArchivo,
                    @Extension,
                    @TamanoKB,
                    1
                )", conn);

                    cmd.Parameters.AddWithValue("@IdPaciente", documento.IdPaciente);
                    cmd.Parameters.AddWithValue("@TipoDocumento", documento.TipoDocumento);
                    cmd.Parameters.AddWithValue("@NombreArchivo", documento.NombreArchivo);
                    cmd.Parameters.AddWithValue("@RutaArchivo", documento.RutaArchivo);
                    cmd.Parameters.AddWithValue("@Extension", documento.Extension);
                    cmd.Parameters.AddWithValue("@TamanoKB", documento.TamanoKB);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

                return false;
            }
        }

        public List<DocumentoPaciente> ObtenerPorPaciente(int idPaciente)
        {
            List<DocumentoPaciente> lista = new();

            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                                                SELECT *
                                                FROM tbDocumentosPaciente
                                                WHERE IdPaciente=@IdPaciente
                                                AND Activo=1
                                                ORDER BY FechaSubida DESC;", conn);

                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    DocumentoPaciente doc = new DocumentoPaciente();

                    doc.IdDocumento = Convert.ToInt32(reader["IdDocumento"]);
                    doc.IdPaciente = Convert.ToInt32(reader["IdPaciente"]);
                    doc.TipoDocumento = reader["TipoDocumento"].ToString()!;
                    doc.NombreArchivo = reader["NombreArchivo"].ToString()!;
                    doc.RutaArchivo = reader["RutaArchivo"].ToString()!;
                    doc.Extension = reader["Extension"].ToString()!;
                    doc.TamanoKB = Convert.ToDecimal(reader["TamanoKB"]);
                    doc.FechaSubida = Convert.ToDateTime(reader["FechaSubida"]);
                    doc.Activo = Convert.ToBoolean(reader["Activo"]);

                    lista.Add(doc);
                }
            }

            return lista;
        }

        public bool Eliminar(int idDocumento)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                                                UPDATE tbDocumentosPaciente
                                                SET Activo=0
                                                WHERE IdDocumento=@IdDocumento;", conn);

                cmd.Parameters.AddWithValue("@IdDocumento", idDocumento);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
