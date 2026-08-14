using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace Proyecto_MediSys.Data
{
    class UsuarioDAO
    {
        Conexion conexion = new Conexion();

        /*metodo para validar el login del usuario, recibe el nombre de usuario y la clave encriptada, retorna un booleano indicando si el usuario es valido o no*/
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
        }/*aqui terqmina el metodo para validar el login del usuario*/

        //-------------------------------------------------
        // Iniciar Sesión
        //-------------------------------------------------

        public Usuario? IniciarSesion(string usuario, string clave)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                string sql = @"

                               SELECT
                    U.*,
                    R.Nombre AS NombreRol,
                    M.IdEspecialidad
                FROM tbUsuarios U
                INNER JOIN tbRoles R
                    ON U.IdRol = R.IdRol
                LEFT JOIN tbMedicos M
                    ON U.IdMedico = M.IdMedico
                WHERE
                    U.Usuario = @Usuario
                    AND U.ClaveHash = @Clave
                    AND U.Activo = 1;

                    ";
                SqlCommand cmd = new(sql, conn);

                cmd.Parameters.AddWithValue("@Usuario", usuario);

                cmd.Parameters.AddWithValue("@Clave", clave);

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.Read())
                {
                    return null;
                }
               
               return LeerUsuario(reader);
            }
            
        }


        /*metodo para obtener todos los usuarios de la base de datos, retorna una lista de objetos Usuario*/
        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> lista = new();

            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                                      SELECT
                                        U.*,
                                        R.Nombre AS NombreRol,
                                        M.IdEspecialidad
                                    FROM tbUsuarios U
                                    INNER JOIN tbRoles R
                                        ON U.IdRol = R.IdRol
                                    LEFT JOIN tbMedicos M
                                        ON U.IdMedico = M.IdMedico
                                    WHERE U.Activo = 1
                                    ORDER BY U.Nombre, U.Apellido;", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(LeerUsuario(reader));
                }
            }

            return lista;
        }/*aqui termina el metodo para obtener todos los usuarios de la base de datos, retorna una lista de objetos Usuario*/

        /*metodo para insertar un nuevo usuario en la base de datos, recibe un objeto Usuario y retorna un booleano indicando si la insercion fue exitosa o no*/
        public bool Insertar(Usuario usuario)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_InsertarUsuario", conn);

                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);

                cmd.Parameters.AddWithValue("@SegundoApellido",
                    string.IsNullOrWhiteSpace(usuario.SegundoApellido)
                    ? DBNull.Value
                    : usuario.SegundoApellido);

                cmd.Parameters.AddWithValue("@Usuario", usuario.UsuarioLogin);

                cmd.Parameters.AddWithValue("@ClaveHash", usuario.ClaveHash);

                cmd.Parameters.AddWithValue("@Correo", usuario.Correo);

                cmd.Parameters.AddWithValue("@Telefono",
                    string.IsNullOrWhiteSpace(usuario.Telefono)
                    ? DBNull.Value
                    : usuario.Telefono);

                cmd.Parameters.AddWithValue("@IdRol", usuario.IdRol);

                cmd.Parameters.AddWithValue("@Activo", usuario.Activo);

                cmd.Parameters.AddWithValue("@DebeCambiarClave",
                    usuario.DebeCambiarClave);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    usuario.IdUsuario = Convert.ToInt64(reader["IdUsuario"]);
                    usuario.CodigoUsuario = reader["CodigoUsuario"].ToString()!;
                }

                return true;
            }
        }/*aqui termina el metodo para insertar un nuevo usuario en la base de datos, recibe un objeto Usuario y retorna un booleano indicando si la insercion fue exitosa o no*/


        /*metodo para actualizar un usuario en la base de datos, recibe un objeto Usuario y retorna un booleano indicando si la actualizacion fue exitosa o no*/
        public bool Actualizar(Usuario usuario)
        {
            try
            {
                using (SqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_ActualizarUsuario", conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", usuario.IdUsuario);
                    cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                    cmd.Parameters.AddWithValue("@Apellido", usuario.Apellido);

                    cmd.Parameters.AddWithValue("@SegundoApellido",
                        string.IsNullOrWhiteSpace(usuario.SegundoApellido)
                        ? DBNull.Value
                        : usuario.SegundoApellido);

                    cmd.Parameters.AddWithValue("@Usuario", usuario.UsuarioLogin);
                    cmd.Parameters.AddWithValue("@ClaveHash", usuario.ClaveHash);
                    cmd.Parameters.AddWithValue("@Correo", usuario.Correo);

                    cmd.Parameters.AddWithValue("@Telefono",
                        string.IsNullOrWhiteSpace(usuario.Telefono)
                        ? DBNull.Value
                        : usuario.Telefono);

                    cmd.Parameters.AddWithValue("@IdRol", usuario.IdRol);
                    cmd.Parameters.AddWithValue("@Activo", usuario.Activo);
                    cmd.Parameters.AddWithValue("@DebeCambiarClave", usuario.DebeCambiarClave);

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }/*aqui termina el metodo para actualizar un usuario en la base de datos, recibe un objeto Usuario y retorna un booleano indicando si la actualizacion fue exitosa o no*/

        /*metodo para desactivar un usuario*/
        public bool Eliminar(long idUsuario)
        {
            try
            {
                using (SqlConnection conn = conexion.ObtenerConexion())
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("sp_EliminarUsuario", conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return false;
            }
        }



        /*metodo para buscar usuarios*/
        public List<Usuario> Buscar(string texto)
        {
            List<Usuario> lista = new();

            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                                                SELECT
                                                    U.*,
                                                    R.Nombre AS NombreRol,
                                                    M.IdEspecialidad
                                                FROM tbUsuarios U
                                                INNER JOIN tbRoles R
                                                    ON U.IdRol = R.IdRol
                                                LEFT JOIN tbMedicos M
                                                    ON U.IdMedico = M.IdMedico",
                                                                conn);

                cmd.Parameters.AddWithValue("@texto", "%" + texto + "%");

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Usuario usuario = new Usuario();

                    usuario.IdUsuario = Convert.ToInt64(reader["IdUsuario"]);
                    usuario.CodigoUsuario = reader["CodigoUsuario"].ToString()!;
                    usuario.Nombre = reader["Nombre"].ToString()!;
                    usuario.Apellido = reader["Apellido"].ToString()!;
                    usuario.SegundoApellido = reader["SegundoApellido"] == DBNull.Value
                        ? ""
                        : reader["SegundoApellido"].ToString()!;

                    usuario.UsuarioLogin = reader["Usuario"].ToString()!;
                    usuario.ClaveHash = reader["ClaveHash"].ToString()!;
                    usuario.Correo = reader["Correo"].ToString()!;
                    usuario.Telefono = reader["Telefono"] == DBNull.Value
                        ? ""
                        : reader["Telefono"].ToString()!;

                    usuario.IdRol = Convert.ToInt64(reader["IdRol"]);
                    usuario.NombreRol = reader["NombreRol"].ToString()!;

                    usuario.Activo = Convert.ToBoolean(reader["Activo"]);
                    usuario.DebeCambiarClave = Convert.ToBoolean(reader["DebeCambiarClave"]);
                    usuario.IntentosFallidos = Convert.ToInt16(reader["IntentosFallidos"]);

                    usuario.UltimoAcceso =
                        reader["UltimoAcceso"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["UltimoAcceso"]);

                    usuario.FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]);

                    usuario.FechaModificacion =
                        reader["FechaModificacion"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["FechaModificacion"]);

                    lista.Add(usuario);
                }
            }

            return lista;
        }

        private Usuario LeerUsuario(SqlDataReader reader)
        {
            Usuario usuario = new Usuario();

            usuario.IdUsuario = Convert.ToInt64(reader["IdUsuario"]);
            usuario.CodigoUsuario = reader["CodigoUsuario"].ToString()!;
            usuario.Nombre = reader["Nombre"].ToString()!;
            usuario.Apellido = reader["Apellido"].ToString()!;

            usuario.SegundoApellido =
                reader["SegundoApellido"] == DBNull.Value
                ? ""
                : reader["SegundoApellido"].ToString()!;

            usuario.UsuarioLogin = reader["Usuario"].ToString()!;
            usuario.ClaveHash = reader["ClaveHash"].ToString()!;
            usuario.Correo = reader["Correo"].ToString()!;

            usuario.Telefono =
                reader["Telefono"] == DBNull.Value
                ? ""
                : reader["Telefono"].ToString()!;

            usuario.IdRol = Convert.ToInt64(reader["IdRol"]);
            usuario.NombreRol = reader["NombreRol"].ToString()!;

            usuario.IdMedico = 
                reader["IdMedico"] == DBNull.Value 
                      ? null                      : Convert.ToInt64(reader["IdMedico"]);

            usuario.IdEspecialidad = 
                reader["IdEspecialidad"] == DBNull.Value 
                      ? null
                      : Convert.ToInt64(reader["IdEspecialidad"]);

            usuario.Activo = Convert.ToBoolean(reader["Activo"]);
            usuario.DebeCambiarClave = Convert.ToBoolean(reader["DebeCambiarClave"]);

            usuario.IntentosFallidos = Convert.ToInt16(reader["IntentosFallidos"]);

            usuario.UltimoAcceso =
                reader["UltimoAcceso"] == DBNull.Value
                ? null
                : Convert.ToDateTime(reader["UltimoAcceso"]);

            usuario.FechaCreacion =
                Convert.ToDateTime(reader["FechaCreacion"]);

            usuario.FechaModificacion =
                reader["FechaModificacion"] == DBNull.Value
                ? null
                : Convert.ToDateTime(reader["FechaModificacion"]);

            return usuario;
        }


    }
}
