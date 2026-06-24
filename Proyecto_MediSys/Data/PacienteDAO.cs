using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Proyecto_MediSys.Data
{
    class PacienteDAO
    {
        private readonly Conexion conexion = new Conexion();

        //metodo insertar paciente123456
        public bool Insertar(Paciente paciente)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("dbo.sp_InsertarPaciente", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", paciente.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", paciente.Apellido);
                cmd.Parameters.AddWithValue("@SegundoNombre",(object?)paciente.SegundoNombre ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SegundoApellido",(object?)paciente.SegundoApellido ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaNacimiento", paciente.FechaNacimiento);
                cmd.Parameters.AddWithValue("@Sexo", paciente.Sexo);
                cmd.Parameters.AddWithValue("@Telefono", paciente.Telefono);
                cmd.Parameters.AddWithValue("@Correo",(object?)paciente.Correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Direccion", paciente.Direccion);
                cmd.Parameters.AddWithValue("@IdTipoPaciente", paciente.IdTipoPaciente);
                cmd.Parameters.AddWithValue("@IdSeguro", paciente.IdSeguro);
                cmd.Parameters.AddWithValue("@IdEstadoPaciente", paciente.IdEstadoPaciente);
                cmd.Parameters.AddWithValue("@Indocumentado", paciente.Indocumentado);
                cmd.Parameters.AddWithValue("@TipoDocumento",paciente.Indocumentado? DBNull.Value: (object)paciente.TipoDocumento);
                cmd.Parameters.AddWithValue("@NumeroDocumento",paciente.Indocumentado? DBNull.Value: (object)paciente.NumeroDocumento);
                cmd.Parameters.AddWithValue("@CodigoTemporal",paciente.Indocumentado? (object)paciente.CodigoTemporal : DBNull.Value);

                cmd.ExecuteNonQuery();

                return true;
               
            }
        } //aqui termina el metodo insertar paciente

        //metodo para obtener todos los pacientes
        public List<Paciente> ObtenerTodos()
        {
            List<Paciente> lista = new List<Paciente>();

            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(@"SELECT
                                                  IdPaciente,
                                                   CodigoPaciente,
                                                   Nombre,
                                                   SegundoNombre,
                                                   Apellido,
                                                   SegundoApellido,
                                                   FechaNacimiento,
                                                   Sexo,
                                                   Telefono,
                                                   Correo,
                                                   Direccion,
                                                   IdTipoPaciente,
                                                   IdSeguro,
                                                   IdEstadoPaciente,
                                                   Indocumentado,
                                                   TipoDocumento,
                                                   NumeroDocumento,
                                                   CodigoTemporal,
                                                   Activo,
                                                   FechaCreacion,
                                                   FechaModificacion
                                                  FROM tbPacientes", conn);

            }

            return lista;
        }

        //====================
        // Actualizar paciente
        //====================
        public bool Actualizar(Paciente paciente)
        {
            return false;
        }

        //====================
        // Eliminar paciente
        //====================
        public bool Eliminar(int idPaciente)
        {
            return false;
        }

    }
}