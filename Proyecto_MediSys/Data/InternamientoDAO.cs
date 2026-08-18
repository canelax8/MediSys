using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Models;
using System;
using System.Collections.Generic;
using System.Data;
using static Proyecto_MediSys.Models.Cama;

namespace Proyecto_MediSys.Data
{
    public class InternamientoDAO
    {
        private readonly Conexion conexion =
            new Conexion();


        // =========================================================
        // OBTENER TODOS
        // =========================================================

        public List<Internamiento> ObtenerTodos()
        {
            List<Internamiento> lista =
                new List<Internamiento>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT
                    I.IdInternamiento,
                    I.CodigoInternamiento,

                    I.IdPaciente,

                    LTRIM(RTRIM(CONCAT(
                        P.Nombre, ' ',
                        P.SegundoNombre, ' ',
                        P.Apellido, ' ',
                        P.SegundoApellido
                    ))) AS NombrePaciente,

                    P.CodigoPaciente,

                    CASE
                        WHEN P.Indocumentado = 1
                            THEN ISNULL(P.CodigoTemporal, '')
                        ELSE ISNULL(P.NumeroDocumento, '')
                    END AS DocumentoPaciente,

                    ISNULL(P.Telefono, '') AS TelefonoPaciente,

                    ISNULL(S.Nombre, '') AS SeguroPaciente,

                    I.IdEmergenciaOrigen,

                    ISNULL(E.CodigoEmergencia, '')
                        AS CodigoEmergenciaOrigen,

                    I.IdMedicoResponsable,

                    LTRIM(RTRIM(CONCAT(
                        M.Nombre, ' ',
                        M.Apellido
                    ))) AS NombreMedico,

                    I.IdEspecialidad,

                    ES.Nombre AS Especialidad,

                    I.IdTipoInternamiento,

                    TI.Nombre AS TipoInternamiento,

                    I.IdEstadoInternamiento,

                    EI.Nombre AS Estado,

                    I.IdCama,

                    C.CodigoCama,

                    H.IdHabitacion,

                    H.NumeroHabitacion AS Habitacion,

                    A.IdArea,

                    A.Nombre AS Area,

                    H.Piso,

                    I.FechaIngreso,

                    I.MotivoInternamiento,

                    I.DiagnosticoIngreso,

                    I.ObservacionesIngreso,

                    I.FechaAlta,

                    I.ObservacionesAlta,

                    I.FechaCreacion,

                    I.FechaModificacion,

                    I.Activo

                FROM tbInternamientos I

                INNER JOIN tbPacientes P
                    ON I.IdPaciente = P.IdPaciente

                LEFT JOIN tbSeguros S
                    ON P.IdSeguro = S.IdSeguro

                LEFT JOIN tbEmergencias E
                    ON I.IdEmergenciaOrigen =
                       E.IdEmergencia

                INNER JOIN tbMedicos M
                    ON I.IdMedicoResponsable =
                       M.IdMedico

                INNER JOIN tbEspecialidades ES
                    ON I.IdEspecialidad =
                       ES.IdEspecialidad

                INNER JOIN tbTiposInternamiento TI
                    ON I.IdTipoInternamiento =
                       TI.IdTipoInternamiento

                INNER JOIN tbEstadosInternamiento EI
                    ON I.IdEstadoInternamiento =
                       EI.IdEstadoInternamiento

                INNER JOIN tbCamas C
                    ON I.IdCama =
                       C.IdCama

                INNER JOIN tbHabitaciones H
                    ON C.IdHabitacion =
                       H.IdHabitacion

                INNER JOIN tbAreasHospitalarias A
                    ON H.IdArea =
                       A.IdArea

                WHERE I.Activo = 1

                ORDER BY
                    CASE
                        WHEN I.IdEstadoInternamiento = 1
                            THEN 0
                        ELSE 1
                    END,
                    I.FechaIngreso DESC;
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
                    LeerInternamiento(
                        reader));
            }


            return lista;
        }


        // =========================================================
        // OBTENER POR ID
        // =========================================================

        public Internamiento? ObtenerPorId(
            long idInternamiento)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT
                    I.IdInternamiento,
                    I.CodigoInternamiento,

                    I.IdPaciente,

                    LTRIM(RTRIM(CONCAT(
                        P.Nombre, ' ',
                        P.SegundoNombre, ' ',
                        P.Apellido, ' ',
                        P.SegundoApellido
                    ))) AS NombrePaciente,

                    P.CodigoPaciente,

                    CASE
                        WHEN P.Indocumentado = 1
                            THEN ISNULL(P.CodigoTemporal, '')
                        ELSE ISNULL(P.NumeroDocumento, '')
                    END AS DocumentoPaciente,

                    ISNULL(P.Telefono, '') AS TelefonoPaciente,

                    ISNULL(S.Nombre, '') AS SeguroPaciente,

                    I.IdEmergenciaOrigen,

                    ISNULL(E.CodigoEmergencia, '')
                        AS CodigoEmergenciaOrigen,

                    I.IdMedicoResponsable,

                    LTRIM(RTRIM(CONCAT(
                        M.Nombre, ' ',
                        M.Apellido
                    ))) AS NombreMedico,

                    I.IdEspecialidad,

                    ES.Nombre AS Especialidad,

                    I.IdTipoInternamiento,

                    TI.Nombre AS TipoInternamiento,

                    I.IdEstadoInternamiento,

                    EI.Nombre AS Estado,

                    I.IdCama,

                    C.CodigoCama,

                    H.IdHabitacion,

                    H.NumeroHabitacion AS Habitacion,

                    A.IdArea,

                    A.Nombre AS Area,

                    H.Piso,

                    I.FechaIngreso,

                    I.MotivoInternamiento,

                    I.DiagnosticoIngreso,

                    I.ObservacionesIngreso,

                    I.FechaAlta,

                    I.ObservacionesAlta,

                    I.FechaCreacion,

                    I.FechaModificacion,

                    I.Activo

                FROM tbInternamientos I

                INNER JOIN tbPacientes P
                    ON I.IdPaciente = P.IdPaciente

                LEFT JOIN tbSeguros S
                    ON P.IdSeguro = S.IdSeguro

                LEFT JOIN tbEmergencias E
                    ON I.IdEmergenciaOrigen =
                       E.IdEmergencia

                INNER JOIN tbMedicos M
                    ON I.IdMedicoResponsable =
                       M.IdMedico

                INNER JOIN tbEspecialidades ES
                    ON I.IdEspecialidad =
                       ES.IdEspecialidad

                INNER JOIN tbTiposInternamiento TI
                    ON I.IdTipoInternamiento =
                       TI.IdTipoInternamiento

                INNER JOIN tbEstadosInternamiento EI
                    ON I.IdEstadoInternamiento =
                       EI.IdEstadoInternamiento

                INNER JOIN tbCamas C
                    ON I.IdCama =
                       C.IdCama

                INNER JOIN tbHabitaciones H
                    ON C.IdHabitacion =
                       H.IdHabitacion

                INNER JOIN tbAreasHospitalarias A
                    ON H.IdArea =
                       A.IdArea

                WHERE
                    I.IdInternamiento =
                    @IdInternamiento;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.AddWithValue(
                "@IdInternamiento",
                idInternamiento);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            if (!reader.Read())
                return null;


            return LeerInternamiento(
                reader);
        }


        // =========================================================
        // TIPOS DE INTERNAMIENTO
        // =========================================================

        public List<TipoInternamiento>
            ObtenerTiposInternamiento()
        {
            List<TipoInternamiento> lista =
                new List<TipoInternamiento>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT
                    IdTipoInternamiento,
                    Nombre,
                    Activo

                FROM tbTiposInternamiento

                WHERE Activo = 1

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
                    new TipoInternamiento
                    {
                        IdTipoInternamiento =
                            Convert.ToInt32(
                                reader[
                                    "IdTipoInternamiento"]),

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
        // ESTADOS
        // =========================================================

        public List<EstadoInternamiento>
            ObtenerEstados()
        {
            List<EstadoInternamiento> lista =
                new List<EstadoInternamiento>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT
                    IdEstadoInternamiento,
                    Nombre,
                    Activo

                FROM tbEstadosInternamiento

                WHERE Activo = 1

                ORDER BY IdEstadoInternamiento;
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
                    new EstadoInternamiento
                    {
                        IdEstadoInternamiento =
                            Convert.ToInt32(
                                reader[
                                    "IdEstadoInternamiento"]),

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
        // ÁREAS
        // =========================================================

        public List<AreaHospitalaria>
            ObtenerAreas()
        {
            List<AreaHospitalaria> lista =
                new List<AreaHospitalaria>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT
                    IdArea,
                    Nombre,
                    Descripcion,
                    Activo

                FROM tbAreasHospitalarias

                WHERE Activo = 1

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
                    new AreaHospitalaria
                    {
                        IdArea =
                            Convert.ToInt64(
                                reader["IdArea"]),

                        Nombre =
                            reader["Nombre"]
                                .ToString() ?? "",

                        Descripcion =
                            reader["Descripcion"] ==
                            DBNull.Value
                                ? ""
                                : reader[
                                    "Descripcion"]
                                    .ToString() ?? "",

                        Activo =
                            Convert.ToBoolean(
                                reader["Activo"])
                    });
            }


            return lista;
        }


        // =========================================================
        // HABITACIONES POR ÁREA
        // =========================================================

        public List<Habitacion>
            ObtenerHabitacionesPorArea(
                long idArea)
        {
            List<Habitacion> lista =
                new List<Habitacion>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT
                    H.IdHabitacion,
                    H.CodigoHabitacion,
                    H.NumeroHabitacion,
                    H.IdArea,
                    A.Nombre AS Area,
                    H.Piso,
                    H.Descripcion,
                    H.Activo

                FROM tbHabitaciones H

                INNER JOIN tbAreasHospitalarias A
                    ON H.IdArea = A.IdArea

                WHERE
                    H.Activo = 1
                    AND H.IdArea = @IdArea

                ORDER BY H.NumeroHabitacion;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.AddWithValue(
                "@IdArea",
                idArea);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                lista.Add(
                    new Habitacion
                    {
                        IdHabitacion =
                            Convert.ToInt64(
                                reader[
                                    "IdHabitacion"]),

                        CodigoHabitacion =
                            reader[
                                "CodigoHabitacion"]
                                .ToString() ?? "",

                        NumeroHabitacion =
                            reader[
                                "NumeroHabitacion"]
                                .ToString() ?? "",

                        IdArea =
                            Convert.ToInt64(
                                reader["IdArea"]),

                        Area =
                            reader["Area"]
                                .ToString() ?? "",

                        Piso =
                            reader["Piso"] ==
                            DBNull.Value
                                ? null
                                : Convert.ToInt32(
                                    reader["Piso"]),

                        Descripcion =
                            reader["Descripcion"] ==
                            DBNull.Value
                                ? ""
                                : reader[
                                    "Descripcion"]
                                    .ToString() ?? "",

                        Activo =
                            Convert.ToBoolean(
                                reader["Activo"])
                    });
            }


            return lista;
        }


        // =========================================================
        // CAMAS DISPONIBLES POR HABITACIÓN
        //
        // idCamaActual permite mostrar la cama actual cuando estamos
        // editando un internamiento.
        // =========================================================

        public List<Cama>
            ObtenerCamasDisponibles(
                long idHabitacion,
                long? idCamaActual = null)
        {
            List<Cama> lista =
                new List<Cama>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT
                    C.IdCama,
                    C.CodigoCama,
                    C.IdHabitacion,

                    H.NumeroHabitacion
                        AS Habitacion,

                    H.IdArea,

                    A.Nombre AS Area,

                    C.IdEstadoCama,

                    EC.Nombre AS Estado,

                    C.Descripcion,

                    C.Activo

                FROM tbCamas C

                INNER JOIN tbHabitaciones H
                    ON C.IdHabitacion =
                       H.IdHabitacion

                INNER JOIN tbAreasHospitalarias A
                    ON H.IdArea =
                       A.IdArea

                INNER JOIN tbEstadosCama EC
                    ON C.IdEstadoCama =
                       EC.IdEstadoCama

                WHERE
                    C.Activo = 1

                    AND C.IdHabitacion =
                        @IdHabitacion

                    AND
                    (
                        C.IdEstadoCama = 1

                        OR

                        C.IdCama =
                        @IdCamaActual
                    )

                ORDER BY C.CodigoCama;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.AddWithValue(
                "@IdHabitacion",
                idHabitacion);


            cmd.Parameters.Add(
                "@IdCamaActual",
                SqlDbType.BigInt)
                .Value =
                    idCamaActual.HasValue
                        ? idCamaActual.Value
                        : DBNull.Value;


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                lista.Add(
                    new Cama
                    {
                        IdCama =
                            Convert.ToInt64(
                                reader["IdCama"]),

                        CodigoCama =
                            reader["CodigoCama"]
                                .ToString() ?? "",

                        IdHabitacion =
                            Convert.ToInt64(
                                reader[
                                    "IdHabitacion"]),

                        Habitacion =
                            reader["Habitacion"]
                                .ToString() ?? "",

                        IdArea =
                            Convert.ToInt64(
                                reader["IdArea"]),

                        Area =
                            reader["Area"]
                                .ToString() ?? "",

                        IdEstadoCama =
                            Convert.ToInt32(
                                reader[
                                    "IdEstadoCama"]),

                        Estado =
                            reader["Estado"]
                                .ToString() ?? "",

                        Descripcion =
                            reader["Descripcion"] ==
                            DBNull.Value
                                ? ""
                                : reader[
                                    "Descripcion"]
                                    .ToString() ?? "",

                        Activo =
                            Convert.ToBoolean(
                                reader["Activo"])
                    });
            }


            return lista;
        }


        // =========================================================
        // VERIFICAR CAMA
        // =========================================================

        public bool CamaDisponible(
            long idCama,
            long? idInternamientoExcluir = null)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            return CamaOcupada(
                con,
                null,
                idCama,
                idInternamientoExcluir)
                == false;
        }


        // =========================================================
        // VERIFICAR PACIENTE INTERNADO
        // =========================================================

        public bool PacienteTieneInternamientoActivo(
            int idPaciente,
            long? idInternamientoExcluir = null)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT COUNT(*)

                FROM tbInternamientos

                WHERE
                    IdPaciente =
                    @IdPaciente

                    AND Activo = 1

                    AND IdEstadoInternamiento = 1

                    AND
                    (
                        @IdInternamientoExcluir IS NULL

                        OR

                        IdInternamiento <>
                        @IdInternamientoExcluir
                    );
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.AddWithValue(
                "@IdPaciente",
                idPaciente);


            cmd.Parameters.Add(
                "@IdInternamientoExcluir",
                SqlDbType.BigInt)
                .Value =
                    idInternamientoExcluir.HasValue
                        ? idInternamientoExcluir.Value
                        : DBNull.Value;


            int cantidad =
                Convert.ToInt32(
                    cmd.ExecuteScalar());


            return cantidad > 0;
        }


        // =========================================================
        // INSERTAR INTERNAMIENTO
        // =========================================================

        public bool Insertar(
            Internamiento internamiento)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            using SqlTransaction trans =
                con.BeginTransaction();


            try
            {
                // =================================================
                // PACIENTE YA INTERNADO
                // =================================================

                if (PacienteInternado(
                    con,
                    trans,
                    internamiento.IdPaciente,
                    null))
                {
                    throw new InvalidOperationException(
                        "El paciente ya posee un internamiento activo.");
                }


                // =================================================
                // CAMA OCUPADA
                // =================================================

                if (CamaOcupada(
                    con,
                    trans,
                    internamiento.IdCama,
                    null))
                {
                    throw new InvalidOperationException(
                        "La cama seleccionada ya se encuentra ocupada.");
                }


                // =================================================
                // GENERAR CÓDIGO
                // =================================================

                internamiento.CodigoInternamiento =
                    GenerarCodigo(
                        con,
                        trans);


                if (internamiento
                    .IdEstadoInternamiento <= 0)
                {
                    internamiento
                        .IdEstadoInternamiento = 1;
                }


                // =================================================
                // INSERTAR
                // =================================================

                string sql = @"
                    INSERT INTO tbInternamientos
                    (
                        CodigoInternamiento,
                        IdPaciente,
                        IdEmergenciaOrigen,
                        IdMedicoResponsable,
                        IdEspecialidad,
                        IdTipoInternamiento,
                        IdEstadoInternamiento,
                        IdCama,
                        FechaIngreso,
                        MotivoInternamiento,
                        DiagnosticoIngreso,
                        ObservacionesIngreso,
                        FechaCreacion,
                        Activo
                    )

                    VALUES
                    (
                        @CodigoInternamiento,
                        @IdPaciente,
                        @IdEmergenciaOrigen,
                        @IdMedicoResponsable,
                        @IdEspecialidad,
                        @IdTipoInternamiento,
                        @IdEstadoInternamiento,
                        @IdCama,
                        @FechaIngreso,
                        @MotivoInternamiento,
                        @DiagnosticoIngreso,
                        @ObservacionesIngreso,
                        GETDATE(),
                        1
                    );

                    SELECT
                        CAST(
                            SCOPE_IDENTITY()
                            AS BIGINT
                        );
                ";


                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        con,
                        trans);


                cmd.Parameters.AddWithValue(
                    "@CodigoInternamiento",
                    internamiento
                        .CodigoInternamiento);


                cmd.Parameters.AddWithValue(
                    "@IdPaciente",
                    internamiento.IdPaciente);


                cmd.Parameters.Add(
                    "@IdEmergenciaOrigen",
                    SqlDbType.BigInt)
                    .Value =
                        internamiento
                            .IdEmergenciaOrigen
                            .HasValue

                            ? internamiento
                                .IdEmergenciaOrigen
                                .Value

                            : DBNull.Value;


                cmd.Parameters.AddWithValue(
                    "@IdMedicoResponsable",
                    internamiento
                        .IdMedicoResponsable);


                cmd.Parameters.AddWithValue(
                    "@IdEspecialidad",
                    internamiento
                        .IdEspecialidad);


                cmd.Parameters.AddWithValue(
                    "@IdTipoInternamiento",
                    internamiento
                        .IdTipoInternamiento);


                cmd.Parameters.AddWithValue(
                    "@IdEstadoInternamiento",
                    internamiento
                        .IdEstadoInternamiento);


                cmd.Parameters.AddWithValue(
                    "@IdCama",
                    internamiento.IdCama);


                cmd.Parameters.Add(
                    "@FechaIngreso",
                    SqlDbType.DateTime2)
                    .Value =
                        internamiento.FechaIngreso;


                cmd.Parameters.AddWithValue(
                    "@MotivoInternamiento",
                    internamiento
                        .MotivoInternamiento);


                cmd.Parameters.AddWithValue(
                    "@DiagnosticoIngreso",
                    string.IsNullOrWhiteSpace(
                        internamiento
                            .DiagnosticoIngreso)

                        ? DBNull.Value

                        : internamiento
                            .DiagnosticoIngreso);


                cmd.Parameters.AddWithValue(
                    "@ObservacionesIngreso",
                    string.IsNullOrWhiteSpace(
                        internamiento
                            .ObservacionesIngreso)

                        ? DBNull.Value

                        : internamiento
                            .ObservacionesIngreso);


                internamiento.IdInternamiento =
                    Convert.ToInt64(
                        cmd.ExecuteScalar());


                // =================================================
                // OCUPAR CAMA
                // =================================================

                ActualizarEstadoCama(
                    con,
                    trans,
                    internamiento.IdCama,
                    2);


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
        // ACTUALIZAR INTERNAMIENTO
        // =========================================================

        public bool Actualizar(
            Internamiento internamiento)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            using SqlTransaction trans =
                con.BeginTransaction();


            try
            {
                InternamientoBasico? actual =
                    ObtenerBasico(
                        con,
                        trans,
                        internamiento
                            .IdInternamiento);


                if (actual == null)
                {
                    throw new InvalidOperationException(
                        "No fue posible encontrar el internamiento.");
                }


                if (actual.IdEstadoInternamiento != 1)
                {
                    throw new InvalidOperationException(
                        "Solo los internamientos activos pueden modificarse.");
                }


                if (PacienteInternado(
                    con,
                    trans,
                    internamiento.IdPaciente,
                    internamiento.IdInternamiento))
                {
                    throw new InvalidOperationException(
                        "El paciente ya posee otro internamiento activo.");
                }


                // =================================================
                // SI CAMBIÓ LA CAMA
                // =================================================

                if (actual.IdCama !=
                    internamiento.IdCama)
                {
                    if (CamaOcupada(
                        con,
                        trans,
                        internamiento.IdCama,
                        internamiento
                            .IdInternamiento))
                    {
                        throw new InvalidOperationException(
                            "La nueva cama seleccionada ya se encuentra ocupada.");
                    }


                    // Liberar anterior
                    ActualizarEstadoCama(
                        con,
                        trans,
                        actual.IdCama,
                        1);


                    // Ocupar nueva
                    ActualizarEstadoCama(
                        con,
                        trans,
                        internamiento.IdCama,
                        2);
                }


                string sql = @"
                    UPDATE tbInternamientos

                    SET
                        IdPaciente =
                            @IdPaciente,

                        IdEmergenciaOrigen =
                            @IdEmergenciaOrigen,

                        IdMedicoResponsable =
                            @IdMedicoResponsable,

                        IdEspecialidad =
                            @IdEspecialidad,

                        IdTipoInternamiento =
                            @IdTipoInternamiento,

                        IdCama =
                            @IdCama,

                        FechaIngreso =
                            @FechaIngreso,

                        MotivoInternamiento =
                            @MotivoInternamiento,

                        DiagnosticoIngreso =
                            @DiagnosticoIngreso,

                        ObservacionesIngreso =
                            @ObservacionesIngreso,

                        FechaModificacion =
                            GETDATE()

                    WHERE
                        IdInternamiento =
                        @IdInternamiento;
                ";


                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        con,
                        trans);


                cmd.Parameters.AddWithValue(
                    "@IdInternamiento",
                    internamiento
                        .IdInternamiento);


                cmd.Parameters.AddWithValue(
                    "@IdPaciente",
                    internamiento
                        .IdPaciente);


                cmd.Parameters.Add(
                    "@IdEmergenciaOrigen",
                    SqlDbType.BigInt)
                    .Value =
                        internamiento
                            .IdEmergenciaOrigen
                            .HasValue

                            ? internamiento
                                .IdEmergenciaOrigen
                                .Value

                            : DBNull.Value;


                cmd.Parameters.AddWithValue(
                    "@IdMedicoResponsable",
                    internamiento
                        .IdMedicoResponsable);


                cmd.Parameters.AddWithValue(
                    "@IdEspecialidad",
                    internamiento
                        .IdEspecialidad);


                cmd.Parameters.AddWithValue(
                    "@IdTipoInternamiento",
                    internamiento
                        .IdTipoInternamiento);


                cmd.Parameters.AddWithValue(
                    "@IdCama",
                    internamiento
                        .IdCama);


                cmd.Parameters.Add(
                    "@FechaIngreso",
                    SqlDbType.DateTime2)
                    .Value =
                        internamiento
                            .FechaIngreso;


                cmd.Parameters.AddWithValue(
                    "@MotivoInternamiento",
                    internamiento
                        .MotivoInternamiento);


                cmd.Parameters.AddWithValue(
                    "@DiagnosticoIngreso",
                    string.IsNullOrWhiteSpace(
                        internamiento
                            .DiagnosticoIngreso)

                        ? DBNull.Value

                        : internamiento
                            .DiagnosticoIngreso);


                cmd.Parameters.AddWithValue(
                    "@ObservacionesIngreso",
                    string.IsNullOrWhiteSpace(
                        internamiento
                            .ObservacionesIngreso)

                        ? DBNull.Value

                        : internamiento
                            .ObservacionesIngreso);


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
        // CAMBIAR CAMA
        // =========================================================

        public bool CambiarCama(
            long idInternamiento,
            long nuevaCama)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            using SqlTransaction trans =
                con.BeginTransaction();


            try
            {
                InternamientoBasico? actual =
                    ObtenerBasico(
                        con,
                        trans,
                        idInternamiento);


                if (actual == null)
                {
                    throw new InvalidOperationException(
                        "No fue posible encontrar el internamiento.");
                }


                if (actual.IdEstadoInternamiento != 1)
                {
                    throw new InvalidOperationException(
                        "Solo un internamiento activo puede cambiar de cama.");
                }


                if (actual.IdCama ==
                    nuevaCama)
                {
                    trans.Rollback();

                    return true;
                }


                if (CamaOcupada(
                    con,
                    trans,
                    nuevaCama,
                    idInternamiento))
                {
                    throw new InvalidOperationException(
                        "La cama seleccionada ya se encuentra ocupada.");
                }


                // =================================================
                // LIBERAR CAMA ANTERIOR
                // =================================================

                ActualizarEstadoCama(
                    con,
                    trans,
                    actual.IdCama,
                    1);


                // =================================================
                // OCUPAR NUEVA
                // =================================================

                ActualizarEstadoCama(
                    con,
                    trans,
                    nuevaCama,
                    2);


                // =================================================
                // ACTUALIZAR INTERNAMIENTO
                // =================================================

                string sql = @"
                    UPDATE tbInternamientos

                    SET
                        IdCama = @IdCama,
                        FechaModificacion = GETDATE()

                    WHERE
                        IdInternamiento =
                        @IdInternamiento;
                ";


                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        con,
                        trans);


                cmd.Parameters.AddWithValue(
                    "@IdCama",
                    nuevaCama);


                cmd.Parameters.AddWithValue(
                    "@IdInternamiento",
                    idInternamiento);


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
        // DAR DE ALTA
        // =========================================================

        public bool DarAlta(
            long idInternamiento,
            string observacionesAlta)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            using SqlTransaction trans =
                con.BeginTransaction();


            try
            {
                InternamientoBasico? actual =
                    ObtenerBasico(
                        con,
                        trans,
                        idInternamiento);


                if (actual == null)
                {
                    throw new InvalidOperationException(
                        "No fue posible encontrar el internamiento.");
                }


                if (actual.IdEstadoInternamiento != 1)
                {
                    throw new InvalidOperationException(
                        "El internamiento ya no se encuentra activo.");
                }


                string sql = @"
                    UPDATE tbInternamientos

                    SET
                        IdEstadoInternamiento = 2,

                        FechaAlta = GETDATE(),

                        ObservacionesAlta =
                            @ObservacionesAlta,

                        FechaModificacion =
                            GETDATE()

                    WHERE
                        IdInternamiento =
                        @IdInternamiento;
                ";


                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        con,
                        trans);


                cmd.Parameters.AddWithValue(
                    "@IdInternamiento",
                    idInternamiento);


                cmd.Parameters.AddWithValue(
                    "@ObservacionesAlta",
                    string.IsNullOrWhiteSpace(
                        observacionesAlta)

                        ? DBNull.Value

                        : observacionesAlta);


                int filas =
                    cmd.ExecuteNonQuery();


                if (filas <= 0)
                {
                    trans.Rollback();

                    return false;
                }


                // =================================================
                // LIBERAR CAMA
                // =================================================

                ActualizarEstadoCama(
                    con,
                    trans,
                    actual.IdCama,
                    1);


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
        // CANCELAR INTERNAMIENTO
        // =========================================================

        public bool Cancelar(
            long idInternamiento)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            using SqlTransaction trans =
                con.BeginTransaction();


            try
            {
                InternamientoBasico? actual =
                    ObtenerBasico(
                        con,
                        trans,
                        idInternamiento);


                if (actual == null)
                {
                    throw new InvalidOperationException(
                        "No fue posible encontrar el internamiento.");
                }


                if (actual.IdEstadoInternamiento != 1)
                {
                    throw new InvalidOperationException(
                        "Solo puede cancelarse un internamiento activo.");
                }


                string sql = @"
                    UPDATE tbInternamientos

                    SET
                        IdEstadoInternamiento = 4,
                        FechaModificacion = GETDATE()

                    WHERE
                        IdInternamiento =
                        @IdInternamiento;
                ";


                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        con,
                        trans);


                cmd.Parameters.AddWithValue(
                    "@IdInternamiento",
                    idInternamiento);


                int filas =
                    cmd.ExecuteNonQuery();


                if (filas <= 0)
                {
                    trans.Rollback();

                    return false;
                }


                ActualizarEstadoCama(
                    con,
                    trans,
                    actual.IdCama,
                    1);


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
        // CONTAR CAMAS DISPONIBLES
        // =========================================================

        public int ContarCamasDisponibles()
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT COUNT(*)

                FROM tbCamas

                WHERE
                    Activo = 1

                    AND IdEstadoCama = 1;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            return Convert.ToInt32(
                cmd.ExecuteScalar());
        }


        // =========================================================
        // CONTAR CAMAS OCUPADAS
        // =========================================================

        public int ContarCamasOcupadas()
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
                SELECT COUNT(*)

                FROM tbCamas

                WHERE
                    Activo = 1

                    AND IdEstadoCama = 2;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            return Convert.ToInt32(
                cmd.ExecuteScalar());
        }


        // =========================================================
        // GENERAR CÓDIGO
        // =========================================================

        private string GenerarCodigo(
            SqlConnection con,
            SqlTransaction trans)
        {
            string sql = @"
                SELECT
                    ISNULL(
                        MAX(IdInternamiento),
                        0
                    ) + 1

                FROM tbInternamientos;
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
                $"INT{siguiente:000000}";
        }


        // =========================================================
        // CAMBIAR ESTADO DE CAMA
        // =========================================================

        private void ActualizarEstadoCama(
            SqlConnection con,
            SqlTransaction trans,
            long idCama,
            int idEstadoCama)
        {
            string sql = @"
                UPDATE tbCamas

                SET
                    IdEstadoCama =
                    @IdEstadoCama

                WHERE
                    IdCama =
                    @IdCama;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con,
                    trans);


            cmd.Parameters.AddWithValue(
                "@IdEstadoCama",
                idEstadoCama);


            cmd.Parameters.AddWithValue(
                "@IdCama",
                idCama);


            cmd.ExecuteNonQuery();
        }


        // =========================================================
        // COMPROBAR CAMA OCUPADA
        // =========================================================

        private bool CamaOcupada(
            SqlConnection con,
            SqlTransaction? trans,
            long idCama,
            long? idInternamientoExcluir)
        {
            string sql = @"
                SELECT COUNT(*)

                FROM tbInternamientos I

                WHERE
                    I.IdCama = @IdCama

                    AND I.Activo = 1

                    AND I.IdEstadoInternamiento = 1

                    AND
                    (
                        @IdInternamientoExcluir IS NULL

                        OR

                        I.IdInternamiento <>
                        @IdInternamientoExcluir
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
                "@IdCama",
                idCama);


            cmd.Parameters.Add(
                "@IdInternamientoExcluir",
                SqlDbType.BigInt)
                .Value =
                    idInternamientoExcluir.HasValue
                        ? idInternamientoExcluir.Value
                        : DBNull.Value;


            int cantidad =
                Convert.ToInt32(
                    cmd.ExecuteScalar());


            return cantidad > 0;
        }


        // =========================================================
        // COMPROBAR PACIENTE INTERNADO
        // =========================================================

        private bool PacienteInternado(
            SqlConnection con,
            SqlTransaction trans,
            int idPaciente,
            long? idInternamientoExcluir)
        {
            string sql = @"
                SELECT COUNT(*)

                FROM tbInternamientos

                WHERE
                    IdPaciente = @IdPaciente

                    AND Activo = 1

                    AND IdEstadoInternamiento = 1

                    AND
                    (
                        @IdInternamientoExcluir IS NULL

                        OR

                        IdInternamiento <>
                        @IdInternamientoExcluir
                    );
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con,
                    trans);


            cmd.Parameters.AddWithValue(
                "@IdPaciente",
                idPaciente);


            cmd.Parameters.Add(
                "@IdInternamientoExcluir",
                SqlDbType.BigInt)
                .Value =
                    idInternamientoExcluir.HasValue
                        ? idInternamientoExcluir.Value
                        : DBNull.Value;


            return Convert.ToInt32(
                cmd.ExecuteScalar()) > 0;
        }


        // =========================================================
        // OBTENER INFORMACIÓN BÁSICA
        // =========================================================

        private InternamientoBasico?
            ObtenerBasico(
                SqlConnection con,
                SqlTransaction trans,
                long idInternamiento)
        {
            string sql = @"
                SELECT
                    IdInternamiento,
                    IdCama,
                    IdEstadoInternamiento

                FROM tbInternamientos

                WHERE
                    IdInternamiento =
                    @IdInternamiento;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con,
                    trans);


            cmd.Parameters.AddWithValue(
                "@IdInternamiento",
                idInternamiento);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            if (!reader.Read())
                return null;


            return new InternamientoBasico
            {
                IdInternamiento =
                    Convert.ToInt64(
                        reader[
                            "IdInternamiento"]),

                IdCama =
                    Convert.ToInt64(
                        reader["IdCama"]),

                IdEstadoInternamiento =
                    Convert.ToInt32(
                        reader[
                            "IdEstadoInternamiento"])
            };
        }


        // =========================================================
        // MAPEAR INTERNAMIENTO
        // =========================================================

        private Internamiento LeerInternamiento(
            SqlDataReader reader)
        {
            return new Internamiento
            {
                IdInternamiento =
                    Convert.ToInt64(
                        reader[
                            "IdInternamiento"]),

                CodigoInternamiento =
                    reader[
                        "CodigoInternamiento"]
                        .ToString() ?? "",


                IdPaciente =
                    Convert.ToInt32(
                        reader["IdPaciente"]),

                NombrePaciente =
                    reader[
                        "NombrePaciente"]
                        .ToString() ?? "",

                CodigoPaciente =
                    reader[
                        "CodigoPaciente"]
                        .ToString() ?? "",

                DocumentoPaciente =
                    reader[
                        "DocumentoPaciente"]
                        .ToString() ?? "",

                TelefonoPaciente =
                    reader[
                        "TelefonoPaciente"]
                        .ToString() ?? "",

                SeguroPaciente =
                    reader[
                        "SeguroPaciente"]
                        .ToString() ?? "",


                IdEmergenciaOrigen =
                    reader[
                        "IdEmergenciaOrigen"] ==
                    DBNull.Value

                        ? null

                        : Convert.ToInt64(
                            reader[
                                "IdEmergenciaOrigen"]),

                CodigoEmergenciaOrigen =
                    reader[
                        "CodigoEmergenciaOrigen"]
                        .ToString() ?? "",


                IdMedicoResponsable =
                    Convert.ToInt64(
                        reader[
                            "IdMedicoResponsable"]),

                NombreMedico =
                    reader[
                        "NombreMedico"]
                        .ToString() ?? "",


                IdEspecialidad =
                    Convert.ToInt64(
                        reader[
                            "IdEspecialidad"]),

                Especialidad =
                    reader[
                        "Especialidad"]
                        .ToString() ?? "",


                IdTipoInternamiento =
                    Convert.ToInt32(
                        reader[
                            "IdTipoInternamiento"]),

                TipoInternamiento =
                    reader[
                        "TipoInternamiento"]
                        .ToString() ?? "",


                IdEstadoInternamiento =
                    Convert.ToInt32(
                        reader[
                            "IdEstadoInternamiento"]),

                Estado =
                    reader["Estado"]
                        .ToString() ?? "",


                IdCama =
                    Convert.ToInt64(
                        reader["IdCama"]),

                CodigoCama =
                    reader[
                        "CodigoCama"]
                        .ToString() ?? "",


                IdHabitacion =
                    Convert.ToInt64(
                        reader[
                            "IdHabitacion"]),

                Habitacion =
                    reader[
                        "Habitacion"]
                        .ToString() ?? "",


                IdArea =
                    Convert.ToInt64(
                        reader["IdArea"]),

                Area =
                    reader["Area"]
                        .ToString() ?? "",


                Piso =
                    reader["Piso"] ==
                    DBNull.Value

                        ? null

                        : Convert.ToInt32(
                            reader["Piso"]),


                FechaIngreso =
                    Convert.ToDateTime(
                        reader[
                            "FechaIngreso"]),


                MotivoInternamiento =
                    reader[
                        "MotivoInternamiento"]
                        .ToString() ?? "",


                DiagnosticoIngreso =
                    reader[
                        "DiagnosticoIngreso"] ==
                    DBNull.Value

                        ? ""

                        : reader[
                            "DiagnosticoIngreso"]
                            .ToString() ?? "",


                ObservacionesIngreso =
                    reader[
                        "ObservacionesIngreso"] ==
                    DBNull.Value

                        ? ""

                        : reader[
                            "ObservacionesIngreso"]
                            .ToString() ?? "",


                FechaAlta =
                    reader["FechaAlta"] ==
                    DBNull.Value

                        ? null

                        : Convert.ToDateTime(
                            reader[
                                "FechaAlta"]),


                ObservacionesAlta =
                    reader[
                        "ObservacionesAlta"] ==
                    DBNull.Value

                        ? ""

                        : reader[
                            "ObservacionesAlta"]
                            .ToString() ?? "",


                FechaCreacion =
                    Convert.ToDateTime(
                        reader[
                            "FechaCreacion"]),


                FechaModificacion =
                    reader[
                        "FechaModificacion"] ==
                    DBNull.Value

                        ? null

                        : Convert.ToDateTime(
                            reader[
                                "FechaModificacion"]),


                Activo =
                    Convert.ToBoolean(
                        reader["Activo"])
            };
        }

        // =========================================================
        // EMERGENCIAS DISPONIBLES PARA INTERNAMIENTO
        // =========================================================

        public List<EmergenciaInternamientoOpcion>
            ObtenerEmergenciasDisponibles(
                long? idInternamientoActual = null)
        {
            List<EmergenciaInternamientoOpcion> lista =
                new List<EmergenciaInternamientoOpcion>();


            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
        SELECT TOP 200

            E.IdEmergencia,

            E.CodigoEmergencia,

            E.IdPaciente,

            LTRIM(RTRIM(CONCAT(
                P.Nombre, ' ',
                P.SegundoNombre, ' ',
                P.Apellido, ' ',
                P.SegundoApellido
            ))) AS NombrePaciente,

            E.IdMedico,

            E.IdEspecialidad,

            ISNULL(
                E.MotivoConsulta,
                ''
            ) AS MotivoConsulta

        FROM tbEmergencias E

        INNER JOIN tbPacientes P
            ON E.IdPaciente =
               P.IdPaciente

        WHERE

            (
                @IdInternamientoActual IS NOT NULL

                AND EXISTS
                (
                    SELECT 1

                    FROM tbInternamientos IA

                    WHERE
                        IA.IdInternamiento =
                            @IdInternamientoActual

                        AND IA.IdEmergenciaOrigen =
                            E.IdEmergencia
                )
            )

            OR

            NOT EXISTS
            (
                SELECT 1

                FROM tbInternamientos I

                WHERE
                    I.IdEmergenciaOrigen =
                        E.IdEmergencia

                    AND I.Activo = 1                   
            )

        ORDER BY
            E.IdEmergencia DESC;
    ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.Add(
                "@IdInternamientoActual",
                SqlDbType.BigInt)
                .Value =
                    idInternamientoActual.HasValue

                        ? idInternamientoActual.Value

                        : DBNull.Value;


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                lista.Add(
                    new EmergenciaInternamientoOpcion
                    {
                        IdEmergencia =
                            Convert.ToInt64(
                                reader["IdEmergencia"]),

                        CodigoEmergencia =
                            reader["CodigoEmergencia"]
                                .ToString() ?? "",

                        IdPaciente =
                            Convert.ToInt32(
                                reader["IdPaciente"]),

                        NombrePaciente =
                            reader["NombrePaciente"]
                                .ToString() ?? "",

                        IdMedico =
                            Convert.ToInt64(
                                reader["IdMedico"]),

                        IdEspecialidad =
                            Convert.ToInt64(
                                reader["IdEspecialidad"]),

                        MotivoConsulta =
                            reader["MotivoConsulta"]
                                .ToString() ?? ""
                    });
            }


            return lista;
        }


        // =========================================================
        // VERIFICAR SI UNA EMERGENCIA YA GENERÓ INTERNAMIENTO
        // =========================================================

        public bool ExisteInternamientoPorEmergencia(
            long idEmergencia)
        {
            using SqlConnection con =
                conexion.ObtenerConexion();

            con.Open();


            string sql = @"
        SELECT COUNT(*)

        FROM tbInternamientos

        WHERE
            IdEmergenciaOrigen =
                @IdEmergencia

            AND Activo = 1;
    ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            cmd.Parameters.AddWithValue(
                "@IdEmergencia",
                idEmergencia);


            int cantidad =
                Convert.ToInt32(
                    cmd.ExecuteScalar());


            return cantidad > 0;
        }

        // =========================================================
        // CLASE INTERNA
        // =========================================================

        private class InternamientoBasico
        {
            public long IdInternamiento { get; set; }

            public long IdCama { get; set; }

            public int IdEstadoInternamiento { get; set; }
        }
    }
}