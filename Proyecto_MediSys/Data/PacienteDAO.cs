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

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        paciente.IdPaciente = Convert.ToInt32(reader["IdPaciente"]);
                        paciente.CodigoPaciente = reader["CodigoPaciente"].ToString()!;


                        return true;
                    }

                    MessageBox.Show("El procedimiento no devolvió ningún registro.");

                    return false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    throw;
                }
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
                                                P.IdPaciente,
                                                P.CodigoPaciente,

                                                P.Nombre,
                                                P.SegundoNombre,
                                                P.Apellido,
                                                P.SegundoApellido,

                                                P.FechaNacimiento,
                                                P.Sexo,

                                                P.Telefono,
                                                P.Correo,
                                                P.Direccion,

                                                P.IdTipoPaciente,
                                                TP.Nombre AS NombreTipoPaciente,

                                                P.IdSeguro,
                                                S.Nombre AS NombreSeguro,

                                                P.IdEstadoPaciente,
                                                EP.Nombre AS NombreEstadoPaciente,

                                                P.Indocumentado,
                                                P.TipoDocumento,
                                                P.NumeroDocumento,
                                                P.CodigoTemporal,

                                                P.Activo,
                                                P.FechaCreacion,
                                                P.FechaModificacion

                                            FROM tbPacientes P

                                                INNER JOIN tbTipoPaciente TP
                                                    ON TP.IdTipoPaciente = P.IdTipoPaciente

                                                INNER JOIN tbSeguros S
                                                    ON S.IdSeguro = P.IdSeguro

                                                INNER JOIN tbEstadoPaciente EP
                                                    ON EP.IdEstadoPaciente = P.IdEstadoPaciente

                                                WHERE P.Activo = 1

                                                ORDER BY P.Nombre;", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Paciente paciente = new Paciente();

                    paciente.IdPaciente = Convert.ToInt32(reader["IdPaciente"]);
                    paciente.CodigoPaciente = reader["CodigoPaciente"].ToString()!;
                    paciente.Nombre = reader["Nombre"].ToString()!;
                    paciente.SegundoNombre = reader["SegundoNombre"]?.ToString();
                    paciente.Apellido = reader["Apellido"].ToString()!;
                    paciente.SegundoApellido = reader["SegundoApellido"]?.ToString();       
                    paciente.FechaNacimiento = Convert.ToDateTime(reader["FechaNacimiento"]);
                    paciente.Sexo = reader["Sexo"].ToString()!;
                    paciente.Telefono = reader["Telefono"]?.ToString() ?? "";
                    paciente.Correo = reader["Correo"]?.ToString();
                    paciente.Direccion = reader["Direccion"]?.ToString() ?? "";
                    paciente.IdTipoPaciente = Convert.ToInt32(reader["IdTipoPaciente"]);
                    paciente.IdSeguro = Convert.ToInt32(reader["IdSeguro"]);
                    paciente.IdEstadoPaciente = Convert.ToInt32(reader["IdEstadoPaciente"]);
                    paciente.NombreTipoPaciente = reader["NombreTipoPaciente"].ToString()!;
                    paciente.NombreSeguro = reader["NombreSeguro"].ToString()!;
                    paciente.NombreEstadoPaciente = reader["NombreEstadoPaciente"].ToString()!;
                    paciente.Indocumentado = Convert.ToBoolean(reader["Indocumentado"]);
                    paciente.TipoDocumento = reader["TipoDocumento"] == DBNull.Value
                        ? ""
                        : reader["TipoDocumento"].ToString()!;

                    paciente.NumeroDocumento = reader["NumeroDocumento"] == DBNull.Value
                        ? ""
                        : reader["NumeroDocumento"].ToString()!;

                    paciente.CodigoTemporal = reader["CodigoTemporal"] == DBNull.Value
                        ? ""
                        : reader["CodigoTemporal"].ToString()!;

                    paciente.Activo = Convert.ToBoolean(reader["Activo"]);
                    paciente.FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]);

                    if (reader["FechaModificacion"] != DBNull.Value)
                        paciente.FechaModificacion = Convert.ToDateTime(reader["FechaModificacion"]);

                    
                    lista.Add(paciente);
                }

            }

            return lista;
        }

        //====================
        // Actualizar paciente
        //====================
        public bool Actualizar(Paciente paciente)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_ActualizarPaciente", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", paciente.IdPaciente);

                cmd.Parameters.AddWithValue("@Nombre", paciente.Nombre);
                cmd.Parameters.AddWithValue("@SegundoNombre",
                    (object?)paciente.SegundoNombre ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@Apellido", paciente.Apellido);

                cmd.Parameters.AddWithValue("@SegundoApellido",
                    (object?)paciente.SegundoApellido ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@FechaNacimiento", paciente.FechaNacimiento);
                cmd.Parameters.AddWithValue("@Sexo", paciente.Sexo);

                cmd.Parameters.AddWithValue("@Telefono",
                    string.IsNullOrWhiteSpace(paciente.Telefono)
                        ? DBNull.Value
                        : (object)paciente.Telefono);

                cmd.Parameters.AddWithValue("@Correo",
                    string.IsNullOrWhiteSpace(paciente.Correo)
                        ? DBNull.Value
                        : (object)paciente.Correo);

                cmd.Parameters.AddWithValue("@Direccion",
                    string.IsNullOrWhiteSpace(paciente.Direccion)
                        ? DBNull.Value
                        : (object)paciente.Direccion);

                cmd.Parameters.AddWithValue("@IdTipoPaciente", paciente.IdTipoPaciente);
                cmd.Parameters.AddWithValue("@IdSeguro", paciente.IdSeguro);
                cmd.Parameters.AddWithValue("@IdEstadoPaciente", paciente.IdEstadoPaciente);

                cmd.Parameters.AddWithValue("@Indocumentado", paciente.Indocumentado);

                cmd.Parameters.AddWithValue("@TipoDocumento",
                    paciente.Indocumentado
                        ? DBNull.Value
                        : (object)paciente.TipoDocumento);

                cmd.Parameters.AddWithValue("@NumeroDocumento",
                    paciente.Indocumentado
                        ? DBNull.Value
                        : (object)paciente.NumeroDocumento);

                cmd.Parameters.AddWithValue("@CodigoTemporal",
                    paciente.Indocumentado
                        ? (object)paciente.CodigoTemporal
                        : DBNull.Value);

                object resultado = cmd.ExecuteScalar();

                return Convert.ToInt32(resultado) == 1;
            }
        }

        //====================
        // Eliminar paciente (Eliminación lógica)
        //====================
        public bool Eliminar(int idPaciente)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_DesactivarPaciente", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdPaciente", idPaciente);

                int filas = cmd.ExecuteNonQuery();

               

                return filas > 0;
            }
        }
        //========================================
        // Buscar pacientes
        //========================================
        public List<Paciente> Buscar(string texto)
        {
            List<Paciente> lista = new();




            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();



                SqlCommand cmd = new SqlCommand(@"
                                                SELECT

                                                P.IdPaciente,
                                                P.CodigoPaciente,

                                                P.Nombre,
                                                P.SegundoNombre,
                                                P.Apellido,
                                                P.SegundoApellido,

                                                P.FechaNacimiento,

                                                P.Indocumentado,

                                                P.NumeroDocumento,
                                                P.CodigoTemporal,

                                                TP.Nombre AS TipoPaciente,

                                                S.Nombre AS Seguro

                                                FROM tbPacientes P

                                                INNER JOIN tbTipoPaciente TP
                                                ON TP.IdTipoPaciente=P.IdTipoPaciente

                                                INNER JOIN tbSeguros S
                                                ON S.IdSeguro=P.IdSeguro

                                                WHERE

                                                P.Activo=1

                                                AND
                                                (

                                                P.Nombre LIKE @Texto

                                                OR

                                                P.Apellido LIKE @Texto

                                                OR

                                                P.NumeroDocumento LIKE @Texto

                                                OR

                                                P.CodigoPaciente LIKE @Texto

                                                OR

                                                P.CodigoTemporal LIKE @Texto

                                                )

                                                ORDER BY P.Nombre

                                                ", conn);

                cmd.Parameters.AddWithValue("@Texto", "%" + texto + "%");

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Paciente p = new Paciente();

                    p.IdPaciente = Convert.ToInt32(reader["IdPaciente"]);
                    p.CodigoPaciente = reader["CodigoPaciente"].ToString()!;
                    p.Nombre = reader["Nombre"].ToString()!;
                    p.Apellido = reader["Apellido"].ToString()!;

                    p.NumeroDocumento =
                        reader["NumeroDocumento"] == DBNull.Value
                        ? ""
                        : reader["NumeroDocumento"].ToString()!;

                    p.CodigoTemporal =
                        reader["CodigoTemporal"] == DBNull.Value
                        ? ""
                        : reader["CodigoTemporal"].ToString()!;

                    p.NombreTipoPaciente =
                        reader["TipoPaciente"].ToString()!;

                    p.NombreSeguro =
                        reader["Seguro"].ToString()!;

                    lista.Add(p);
                }


            }

            return lista;
        }
    }
}