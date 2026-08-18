using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace Proyecto_MediSys.Data
{
    public class CitaDAO
    {
        private readonly Conexion conexion =
            new Conexion();


        // =========================================================
        // OBTENER TODAS LAS CITAS
        // =========================================================

        public List<Cita> ObtenerTodos()
        {
            List<Cita> lista =
                new List<Cita>();


            using SqlConnection con =
                conexion.ObtenerConexion();


            con.Open();


            string sql = @"
                SELECT
                    C.IdCita,
                    C.CodigoCita,

                    C.IdPaciente,

                    LTRIM(RTRIM(
                        CONCAT(
                            P.Nombre, ' ',
                            P.SegundoNombre, ' ',
                            P.Apellido, ' ',
                            P.SegundoApellido
                        )
                    )) AS Paciente,

                    C.IdMedico,

                    LTRIM(RTRIM(
                        CONCAT(
                            M.Nombre, ' ',
                            M.Apellido
                        )
                    )) AS Medico,

                    C.IdEspecialidad,

                    E.Nombre AS Especialidad,

                    C.IdEstadoCita,

                    EC.Nombre AS Estado,

                    C.FechaCita,
                    C.HoraCita,
                    C.Motivo,
                    C.Observaciones,
                    C.FechaCreacion,
                    C.FechaModificacion,
                    C.Activo

                FROM tbCitas C

                INNER JOIN tbPacientes P
                    ON C.IdPaciente =
                       P.IdPaciente

                INNER JOIN tbMedicos M
                    ON C.IdMedico =
                       M.IdMedico

                INNER JOIN tbEspecialidades E
                    ON C.IdEspecialidad =
                       E.IdEspecialidad

                INNER JOIN tbEstadosCita EC
                    ON C.IdEstadoCita =
                       EC.IdEstadoCita

                WHERE C.Activo = 1

                ORDER BY
                    C.FechaCita DESC,
                    C.HoraCita DESC;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                lista.Add(
                    LeerCita(reader));
            }


            return lista;
        }


        // =========================================================
        // OBTENER CITA POR ID
        // =========================================================

        public Cita? ObtenerPorId(
            long idCita)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();


            con.Open();


            string sql = @"
                SELECT
                    C.IdCita,
                    C.CodigoCita,

                    C.IdPaciente,

                    LTRIM(RTRIM(
                        CONCAT(
                            P.Nombre, ' ',
                            P.SegundoNombre, ' ',
                            P.Apellido, ' ',
                            P.SegundoApellido
                        )
                    )) AS Paciente,

                    C.IdMedico,

                    LTRIM(RTRIM(
                        CONCAT(
                            M.Nombre, ' ',
                            M.Apellido
                        )
                    )) AS Medico,

                    C.IdEspecialidad,

                    E.Nombre AS Especialidad,

                    C.IdEstadoCita,

                    EC.Nombre AS Estado,

                    C.FechaCita,
                    C.HoraCita,
                    C.Motivo,
                    C.Observaciones,
                    C.FechaCreacion,
                    C.FechaModificacion,
                    C.Activo

                FROM tbCitas C

                INNER JOIN tbPacientes P
                    ON C.IdPaciente =
                       P.IdPaciente

                INNER JOIN tbMedicos M
                    ON C.IdMedico =
                       M.IdMedico

                INNER JOIN tbEspecialidades E
                    ON C.IdEspecialidad =
                       E.IdEspecialidad

                INNER JOIN tbEstadosCita EC
                    ON C.IdEstadoCita =
                       EC.IdEstadoCita

                WHERE C.IdCita =
                      @IdCita;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.AddWithValue(
                "@IdCita",
                idCita);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            if (!reader.Read())
            {
                return null;
            }


            return LeerCita(
                reader);
        }


        // =========================================================
        // OBTENER ESTADOS
        // =========================================================

        public List<EstadoCita> ObtenerEstados()
        {
            List<EstadoCita> lista =
                new List<EstadoCita>();


            using SqlConnection con =
                conexion.ObtenerConexion();


            con.Open();


            string sql = @"
                SELECT
                    IdEstadoCita,
                    Nombre,
                    Activo

                FROM tbEstadosCita

                WHERE Activo = 1

                ORDER BY IdEstadoCita;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                lista.Add(
                    new EstadoCita
                    {
                        IdEstadoCita =
                            Convert.ToInt32(
                                reader["IdEstadoCita"]),

                        Nombre =
                            reader["Nombre"]
                                .ToString() ?? "",

                        Activo =
                            Convert.ToBoolean(
                                reader["Activo"])
                    });
            }


            return lista;
        }


        // =========================================================
        // INSERTAR CITA
        // =========================================================

        public bool Insertar(
            Cita cita)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();


            con.Open();


            using SqlTransaction trans =
                con.BeginTransaction();


            try
            {
                // =================================================
                // VALIDAR HORARIO
                // =================================================

                if (ExisteHorario(
                    con,
                    trans,
                    cita.IdMedico,
                    cita.FechaCita,
                    cita.HoraCita,
                    null))
                {
                    throw new InvalidOperationException(
                        "El médico ya tiene una cita registrada para la fecha y hora seleccionadas.");
                }


                // =================================================
                // GENERAR CÓDIGO
                // =================================================

                cita.CodigoCita =
                    GenerarCodigoCita(
                        con,
                        trans);


                // Si no se especifica estado,
                // comienza como Pendiente.

                if (cita.IdEstadoCita <= 0)
                {
                    cita.IdEstadoCita =
                        1;
                }


                string sql = @"
                    INSERT INTO tbCitas
                    (
                        CodigoCita,
                        IdPaciente,
                        IdMedico,
                        IdEspecialidad,
                        IdEstadoCita,
                        FechaCita,
                        HoraCita,
                        Motivo,
                        Observaciones,
                        FechaCreacion,
                        Activo
                    )

                    VALUES
                    (
                        @CodigoCita,
                        @IdPaciente,
                        @IdMedico,
                        @IdEspecialidad,
                        @IdEstadoCita,
                        @FechaCita,
                        @HoraCita,
                        @Motivo,
                        @Observaciones,
                        GETDATE(),
                        1
                    );
                ";


                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        con,
                        trans);


                cmd.Parameters.AddWithValue(
                    "@CodigoCita",
                    cita.CodigoCita);


                cmd.Parameters.AddWithValue(
                    "@IdPaciente",
                    cita.IdPaciente);


                cmd.Parameters.AddWithValue(
                    "@IdMedico",
                    cita.IdMedico);


                cmd.Parameters.AddWithValue(
                    "@IdEspecialidad",
                    cita.IdEspecialidad);


                cmd.Parameters.AddWithValue(
                    "@IdEstadoCita",
                    cita.IdEstadoCita);


                cmd.Parameters.AddWithValue(
                    "@FechaCita",
                    cita.FechaCita.Date);


                SqlParameter hora =
                    cmd.Parameters.Add(
                        "@HoraCita",
                        SqlDbType.Time);


                hora.Value =
                    cita.HoraCita;


                cmd.Parameters.AddWithValue(
                    "@Motivo",
                    cita.Motivo ?? "");


                cmd.Parameters.AddWithValue(
                    "@Observaciones",
                    string.IsNullOrWhiteSpace(
                        cita.Observaciones)

                        ? DBNull.Value

                        : cita.Observaciones);


                cmd.ExecuteNonQuery();


                trans.Commit();


                return true;
            }
            catch
            {
                trans.Rollback();

                throw;
            }
        }


        // =========================================================
        // ACTUALIZAR CITA
        // =========================================================

        public bool Actualizar(
            Cita cita)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();


            con.Open();


            using SqlTransaction trans =
                con.BeginTransaction();


            try
            {
                // =================================================
                // VALIDAR QUE OTRO REGISTRO NO TENGA EL HORARIO
                // =================================================

                if (ExisteHorario(
                    con,
                    trans,
                    cita.IdMedico,
                    cita.FechaCita,
                    cita.HoraCita,
                    cita.IdCita))
                {
                    throw new InvalidOperationException(
                        "El médico ya tiene otra cita para la fecha y hora seleccionadas.");
                }


                string sql = @"
                    UPDATE tbCitas

                    SET
                        IdPaciente =
                            @IdPaciente,

                        IdMedico =
                            @IdMedico,

                        IdEspecialidad =
                            @IdEspecialidad,

                        FechaCita =
                            @FechaCita,

                        HoraCita =
                            @HoraCita,

                        Motivo =
                            @Motivo,

                        Observaciones =
                            @Observaciones,

                        FechaModificacion =
                            GETDATE()

                    WHERE IdCita =
                          @IdCita

                      AND Activo = 1;
                ";


                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        con,
                        trans);


                cmd.Parameters.AddWithValue(
                    "@IdCita",
                    cita.IdCita);


                cmd.Parameters.AddWithValue(
                    "@IdPaciente",
                    cita.IdPaciente);


                cmd.Parameters.AddWithValue(
                    "@IdMedico",
                    cita.IdMedico);


                cmd.Parameters.AddWithValue(
                    "@IdEspecialidad",
                    cita.IdEspecialidad);


                cmd.Parameters.AddWithValue(
                    "@FechaCita",
                    cita.FechaCita.Date);


                SqlParameter hora =
                    cmd.Parameters.Add(
                        "@HoraCita",
                        SqlDbType.Time);


                hora.Value =
                    cita.HoraCita;


                cmd.Parameters.AddWithValue(
                    "@Motivo",
                    cita.Motivo ?? "");


                cmd.Parameters.AddWithValue(
                    "@Observaciones",
                    string.IsNullOrWhiteSpace(
                        cita.Observaciones)

                        ? DBNull.Value

                        : cita.Observaciones);


                int filas =
                    cmd.ExecuteNonQuery();


                trans.Commit();


                return filas > 0;
            }
            catch
            {
                trans.Rollback();

                throw;
            }
        }


        // =========================================================
        // CAMBIAR ESTADO
        // =========================================================

        public bool ActualizarEstado(
            long idCita,
            int idEstadoCita)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();


            con.Open();


            string sql = @"
                UPDATE tbCitas

                SET
                    IdEstadoCita =
                        @IdEstadoCita,

                    FechaModificacion =
                        GETDATE()

                WHERE IdCita =
                      @IdCita

                  AND Activo = 1;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.AddWithValue(
                "@IdCita",
                idCita);


            cmd.Parameters.AddWithValue(
                "@IdEstadoCita",
                idEstadoCita);


            return cmd.ExecuteNonQuery() > 0;
        }


        // =========================================================
        // VERIFICAR HORARIO
        // =========================================================

        public bool ExisteHorario(
            long idMedico,
            DateTime fecha,
            TimeSpan hora,
            long? idCitaExcluir = null)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();


            con.Open();


            return ExisteHorario(
                con,
                null,
                idMedico,
                fecha,
                hora,
                idCitaExcluir);
        }


        private bool ExisteHorario(
            SqlConnection con,
            SqlTransaction? trans,
            long idMedico,
            DateTime fecha,
            TimeSpan hora,
            long? idCitaExcluir)
        {
            string sql = @"
                SELECT COUNT(*)

                FROM tbCitas

                WHERE IdMedico =
                      @IdMedico

                  AND FechaCita =
                      @FechaCita

                  AND HoraCita =
                      @HoraCita

                  AND Activo = 1

                  AND IdEstadoCita
                      IN (1, 2)

                  AND
                  (
                      @IdCitaExcluir IS NULL

                      OR

                      IdCita <>
                      @IdCitaExcluir
                  );
            ";


            using SqlCommand cmd =
                trans == null
                ? new SqlCommand(
                    sql,
                    con)
                : new SqlCommand(
                    sql,
                    con,
                    trans);


            cmd.Parameters.AddWithValue(
                "@IdMedico",
                idMedico);


            cmd.Parameters.AddWithValue(
                "@FechaCita",
                fecha.Date);


            SqlParameter parametroHora =
                cmd.Parameters.Add(
                    "@HoraCita",
                    SqlDbType.Time);


            parametroHora.Value =
                hora;


            cmd.Parameters.AddWithValue(
                "@IdCitaExcluir",
                idCitaExcluir.HasValue
                    ? idCitaExcluir.Value
                    : DBNull.Value);


            int cantidad =
                Convert.ToInt32(
                    cmd.ExecuteScalar());


            return cantidad > 0;
        }


        // =========================================================
        // GENERAR CÓDIGO
        // =========================================================

        private string GenerarCodigoCita(
            SqlConnection con,
            SqlTransaction trans)
        {
            string sql = @"
                SELECT
                    ISNULL(
                        MAX(IdCita),
                        0
                    ) + 1

                FROM tbCitas;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con,
                    trans);


            long siguiente =
                Convert.ToInt64(
                    cmd.ExecuteScalar());


            return
                $"CI{siguiente:000000}";
        }


        // =========================================================
        // MAPEAR READER
        // =========================================================

        private Cita LeerCita(
            SqlDataReader reader)
        {
            Cita cita =
                new Cita
                {
                    IdCita =
                        Convert.ToInt64(
                            reader["IdCita"]),

                    CodigoCita =
                        reader["CodigoCita"]
                            .ToString() ?? "",

                    IdPaciente =
                        Convert.ToInt32(
                            reader["IdPaciente"]),

                    NombrePaciente =
                        reader["Paciente"]
                            .ToString() ?? "",

                    IdMedico =
                        Convert.ToInt64(
                            reader["IdMedico"]),

                    NombreMedico =
                        reader["Medico"]
                            .ToString() ?? "",

                    IdEspecialidad =
                        Convert.ToInt64(
                            reader["IdEspecialidad"]),

                    Especialidad =
                        reader["Especialidad"]
                            .ToString() ?? "",

                    IdEstadoCita =
                        Convert.ToInt32(
                            reader["IdEstadoCita"]),

                    Estado =
                        reader["Estado"]
                            .ToString() ?? "",

                    FechaCita =
                        Convert.ToDateTime(
                            reader["FechaCita"]),

                    HoraCita =
                        (TimeSpan)
                        reader["HoraCita"],

                    Motivo =
                        reader["Motivo"]
                            .ToString() ?? "",

                    Observaciones =
                        reader["Observaciones"]
                            == DBNull.Value

                            ? ""

                            : reader["Observaciones"]
                                .ToString() ?? "",

                    FechaCreacion =
                        Convert.ToDateTime(
                            reader["FechaCreacion"]),

                    FechaModificacion =
                        reader["FechaModificacion"]
                            == DBNull.Value

                            ? null

                            : Convert.ToDateTime(
                                reader["FechaModificacion"]),

                    Activo =
                        Convert.ToBoolean(
                            reader["Activo"])
                };


            return cita;
        }
        // =========================================================
        // OBTENER ESPECIALIDADES
        // =========================================================

        public List<EspecialidadCitaOpcion> ObtenerEspecialidades()
        {
            List<EspecialidadCitaOpcion> lista =
                new List<EspecialidadCitaOpcion>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
        SELECT
            IdEspecialidad,
            Nombre

        FROM tbEspecialidades

        ORDER BY Nombre;
    ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                lista.Add(
                    new EspecialidadCitaOpcion
                    {
                        IdEspecialidad =
                            Convert.ToInt64(
                                reader["IdEspecialidad"]),

                        Nombre =
                            reader["Nombre"]
                                .ToString() ?? ""
                    });
            }


            return lista;
        }


        // =========================================================
        // OBTENER MÉDICOS
        // =========================================================

        public List<MedicoCitaOpcion> ObtenerMedicos()
        {
            List<MedicoCitaOpcion> lista =
                new List<MedicoCitaOpcion>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                            SELECT
                                M.IdMedico,
                                M.IdEspecialidad,

                                LTRIM(
                                    RTRIM(
                                        CONCAT(
                                            M.Nombre,
                                            ' ',
                                            M.Apellido
                                        )
                                    )
                                ) AS NombreCompleto,

                                E.Nombre AS Especialidad

                            FROM tbMedicos M

                            INNER JOIN tbEspecialidades E
                                ON M.IdEspecialidad =
                                   E.IdEspecialidad

                            ORDER BY
                                E.Nombre,
                                M.Nombre,
                                M.Apellido;
                        ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                lista.Add(
                    new MedicoCitaOpcion
                    {
                        IdMedico =
                            Convert.ToInt64(
                                reader["IdMedico"]),

                        IdEspecialidad =
                            Convert.ToInt64(
                                reader["IdEspecialidad"]),

                        NombreCompleto =
                            reader["NombreCompleto"]
                                .ToString() ?? "",

                        Especialidad =
                            reader["Especialidad"]
                                .ToString() ?? ""
                    });
            }


            return lista;
        }
    }
}