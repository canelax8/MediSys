using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Helpers;
using Proyecto_MediSys.Models;
using System.Data;
using System.Linq;
using System.Windows;

namespace Proyecto_MediSys.Data
{
    public class EmergenciaDAO
    {
        private readonly Conexion conexion = new Conexion();


        public List<Emergencia> ObtenerTodos()
        {
            List<Emergencia> lista = new();

            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
                                SELECT
                                    e.IdEmergencia,
                                    e.CodigoEmergencia,
                                    e.IdEstadoEmergencia,
                                    e.FechaIngreso,

                                    p.Nombre + ' ' + p.Apellido AS Paciente,

                                    m.Nombre + ' ' + m.Apellido AS Medico,

                                    es.Nombre AS Especialidad,

                                    ee.Nombre AS Estado

                                FROM tbEmergencias e

                                INNER JOIN tbPacientes p
                                ON e.IdPaciente = p.IdPaciente

                                INNER JOIN tbMedicos m
                                ON e.IdMedico = m.IdMedico

                                INNER JOIN tbEspecialidades es
                                ON e.IdEspecialidad = es.IdEspecialidad

                                INNER JOIN tbEstadoEmergencia ee
                                ON e.IdEstadoEmergencia = ee.IdEstadoEmergencia

                                ORDER BY e.FechaIngreso DESC";

                SqlCommand cmd = new(sql, con);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Emergencia
                    {
                        IdEmergencia = Convert.ToInt64(dr["IdEmergencia"]),
                        CodigoEmergencia = dr["CodigoEmergencia"].ToString()!,
                        IdEstadoEmergencia = Convert.ToInt64(dr["IdEstadoEmergencia"]),
                        FechaIngreso = Convert.ToDateTime(dr["FechaIngreso"]),
                        NombrePaciente = dr["Paciente"].ToString()!,

                        NombreMedico = dr["Medico"].ToString()!,
                        Especialidad = dr["Especialidad"].ToString()!,

                        Estado = dr["Estado"].ToString()!
                    });
                }
            }

            return lista;

        }
        //-------------------------------------------------
        // Guardar Emergencia
        //-------------------------------------------------

        public bool GuardarEmergenciaCompleta(ProcesoEmergencia proceso)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    Emergencia emergencia = new Emergencia
                    {
                        CodigoEmergencia = GenerarCodigoEmergencia(con, trans),

                        IdPaciente = proceso.Paciente.IdPaciente,

                        IdMedico = SesionActual.Usuario!.IdMedico!.Value,

                        IdEspecialidad = SesionActual.Usuario.IdEspecialidad!.Value,

                        FechaIngreso = proceso.FechaRegistro,

                        MotivoConsulta = proceso.InformacionClinica.MotivoConsulta,

                        IdEstadoEmergencia = 1
                    };

                    long idEmergencia = InsertarEmergencia(
                         con,
                         trans,
                         emergencia);

                    InsertarEvaluacionInicial(
                        con,
                        trans,
                        idEmergencia,
                        proceso.Evaluacion);

                    InsertarInformacionClinica(
                        con,
                        trans,
                        idEmergencia,
                        proceso.InformacionClinica);

                    InsertarDiagnostico(
                        con,
                        trans,
                        idEmergencia,
                        proceso.Diagnostico);


                    // ======================================================
                    // NUEVO: GUARDAR DIAGNÓSTICOS CIE-10 Y MANUALES
                    // ======================================================

                    InsertarDiagnosticosCIE10(
                        con,
                        trans,
                        idEmergencia,
                        proceso);


                    InsertarProcedimientos(
                         con,
                         trans,
                         idEmergencia,
                         proceso.Procedimientos);


                    // ======================================================
                    // NUEVO: ITEMS CLÍNICOS Y FACTURABLES
                    // ======================================================

                    InsertarItemsClinicos(
                        con,
                        trans,
                        idEmergencia,
                        proceso.ItemsClinicos);


                    InsertarDestino(
                         con,
                         trans,
                         idEmergencia,
                         proceso.Destino);

                    ActualizarEstadoEmergencia(
                        con,
                        trans,
                        idEmergencia,
                        proceso.Destino.IdEstadoEmergenciaResultado);

                    trans.Commit();

                    return true;
                }
                catch (Exception)
                {
                    trans.Rollback();

                    throw;
                }
            }
        }

        //-------------------------------------------------
        // Insertar Emergencia
        //-------------------------------------------------

        private long InsertarEmergencia(

             SqlConnection con,
             SqlTransaction trans,
             Emergencia emergencia)
        {

            string sql = @"
            INSERT INTO tbEmergencias
            (
                CodigoEmergencia,
                IdPaciente,
                IdMedico,
                IdEspecialidad,
                IdEstadoEmergencia,
                FechaIngreso,
                MotivoConsulta
            )
            VALUES
            (
                @CodigoEmergencia,
                @IdPaciente,
                @IdMedico,
                @IdEspecialidad,
                @IdEstadoEmergencia,
                @FechaIngreso,
                @MotivoConsulta
            );

            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
            ";

            SqlCommand cmd = new SqlCommand(sql, con, trans);

            cmd.Parameters.AddWithValue("@CodigoEmergencia", emergencia.CodigoEmergencia);
            cmd.Parameters.AddWithValue("@IdPaciente", emergencia.IdPaciente);
            cmd.Parameters.AddWithValue("@IdMedico", emergencia.IdMedico);
            cmd.Parameters.AddWithValue("@IdEspecialidad", emergencia.IdEspecialidad);
            cmd.Parameters.AddWithValue("@IdEstadoEmergencia", emergencia.IdEstadoEmergencia);
            cmd.Parameters.AddWithValue("@FechaIngreso", emergencia.FechaIngreso);
            cmd.Parameters.AddWithValue("@MotivoConsulta", emergencia.MotivoConsulta);

            return Convert.ToInt64(cmd.ExecuteScalar());
        }

        // =========================================================
        // ACTUALIZAR ESTADO FINAL DE LA EMERGENCIA
        // =========================================================

        private void ActualizarEstadoEmergencia(
            SqlConnection con,
            SqlTransaction trans,
            long idEmergencia,
            long idEstadoEmergencia)
        {
            string sql = @"
        UPDATE tbEmergencias

        SET
            IdEstadoEmergencia = @IdEstadoEmergencia,
            FechaModificacion = GETDATE()

        WHERE IdEmergencia = @IdEmergencia;
    ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con,
                    trans);


            cmd.Parameters.AddWithValue(
                "@IdEmergencia",
                idEmergencia);


            cmd.Parameters.AddWithValue(
                "@IdEstadoEmergencia",
                idEstadoEmergencia);


            cmd.ExecuteNonQuery();
        }

        private void InsertarEvaluacionInicial(
        SqlConnection con,
        SqlTransaction trans,
        long idEmergencia,
        EvaluacionInicial evaluacion)
        {
            string sql = @"
                INSERT INTO tbEvaluacionInicial
                (
                    IdEmergencia,
                    NivelTriage,
                    Temperatura,
                    PresionArterial,
                    FrecuenciaCardiaca,
                    FrecuenciaRespiratoria,
                    Saturacion,
                    Glucemia,
                    Peso,
                    Talla
                )

                VALUES
                (
                    @IdEmergencia,
                    @NivelTriage,
                    @Temperatura,
                    @PresionArterial,
                    @FrecuenciaCardiaca,
                    @FrecuenciaRespiratoria,
                    @Saturacion,
                    @Glucemia,
                    @Peso,
                    @Talla
                );
                ";

            SqlCommand cmd = new(sql, con, trans);

            cmd.Parameters.AddWithValue("@IdEmergencia", idEmergencia);

            cmd.Parameters.AddWithValue("@NivelTriage", evaluacion.NivelTriage);

            cmd.Parameters.AddWithValue("@Temperatura",
                evaluacion.Temperatura ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@PresionArterial",
                evaluacion.PresionArterial);

            cmd.Parameters.AddWithValue("@FrecuenciaCardiaca",
                evaluacion.FrecuenciaCardiaca ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@FrecuenciaRespiratoria",
                evaluacion.FrecuenciaRespiratoria ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@Saturacion",
                evaluacion.Saturacion ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@Glucemia",
                evaluacion.Glucemia ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@Peso",
                evaluacion.Peso ?? (object)DBNull.Value);

            cmd.Parameters.AddWithValue("@Talla",
                evaluacion.Talla ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }



        private void InsertarInformacionClinica(
                        SqlConnection con,
                        SqlTransaction trans,
                        long idEmergencia,
                        InformacionClinica informacion)
        {
            string sql = @"
                        INSERT INTO tbInformacionClinica
                        (
                            IdEmergencia,
                            MotivoConsulta,
                            Diabetes,
                            Hipertension,
                            Asma,
                            Cardiopatia,
                            Embarazo,
                            Ninguno,
                            Alergias,
                            MedicamentosActuales,
                            Observaciones
                        )
                        VALUES
                        (
                            @IdEmergencia,
                            @MotivoConsulta,
                            @Diabetes,
                            @Hipertension,
                            @Asma,
                            @Cardiopatia,
                            @Embarazo,
                            @Ninguno,
                            @Alergias,
                            @MedicamentosActuales,
                            @Observaciones
                        );
                        ";

            SqlCommand cmd = new(sql, con, trans);

            cmd.Parameters.AddWithValue("@IdEmergencia", idEmergencia);
            cmd.Parameters.AddWithValue("@MotivoConsulta", informacion.MotivoConsulta);
            cmd.Parameters.AddWithValue("@Diabetes", informacion.Diabetes);
            cmd.Parameters.AddWithValue("@Hipertension", informacion.Hipertension);
            cmd.Parameters.AddWithValue("@Asma", informacion.Asma);
            cmd.Parameters.AddWithValue("@Cardiopatia", informacion.Cardiopatia);
            cmd.Parameters.AddWithValue("@Embarazo", informacion.Embarazo);
            cmd.Parameters.AddWithValue("@Ninguno", informacion.Ninguno);
            cmd.Parameters.AddWithValue("@Alergias", informacion.Alergias);
            cmd.Parameters.AddWithValue("@MedicamentosActuales", informacion.MedicamentosActuales);
            cmd.Parameters.AddWithValue("@Observaciones", informacion.Observaciones);

            cmd.ExecuteNonQuery();
        }

        private void InsertarDiagnostico(
                    SqlConnection con,
                    SqlTransaction trans,
                    long idEmergencia,
                    DiagnosticoEmergencia diagnostico)
        {
            string sql = @"
                    INSERT INTO tbDiagnosticoEmergencia
                    (
                        IdEmergencia,
                        DiagnosticoPrincipal,
                        DiagnosticoSecundario,
                        ImpresionClinica,
                        Observaciones
                    )
                    VALUES
                    (
                        @IdEmergencia,
                        @DiagnosticoPrincipal,
                        @DiagnosticoSecundario,
                        @ImpresionClinica,
                        @Observaciones
                    );
                    ";

            SqlCommand cmd = new(sql, con, trans);

            cmd.Parameters.AddWithValue("@IdEmergencia", idEmergencia);
            cmd.Parameters.AddWithValue("@DiagnosticoPrincipal", diagnostico.DiagnosticoPrincipal);
            cmd.Parameters.AddWithValue("@DiagnosticoSecundario", diagnostico.DiagnosticoSecundario);
            cmd.Parameters.AddWithValue("@ImpresionClinica", diagnostico.ImpresionClinica);
            cmd.Parameters.AddWithValue("@Observaciones", diagnostico.Observaciones);

            cmd.ExecuteNonQuery();
        }

        // =========================================================
        // INSERTAR DIAGNÓSTICOS CIE-10 DE LA EMERGENCIA
        // =========================================================

        private void InsertarDiagnosticosCIE10(
            SqlConnection con,
            SqlTransaction trans,
            long idEmergencia,
            ProcesoEmergencia proceso)
        {
            // =====================================================
            // 1. DIAGNÓSTICOS SELECCIONADOS DEL CATÁLOGO CIE-10
            // =====================================================

            if (proceso.DiagnosticosSeleccionados != null)
            {
                foreach (CIE10 cie10 in proceso.DiagnosticosSeleccionados)
                {
                    bool esPrincipal =
                        proceso.DiagnosticoPrincipalCIE10 != null &&
                        proceso.DiagnosticoPrincipalCIE10.IdCIE10
                            == cie10.IdCIE10;


                    string sql = @"
                INSERT INTO tbEmergenciaDiagnosticos
                (
                    IdEmergencia,
                    IdCIE10,
                    DiagnosticoTexto,
                    EsPrincipal,
                    Observaciones,
                    Activo,
                    FechaRegistro
                )
                VALUES
                (
                    @IdEmergencia,
                    @IdCIE10,
                    NULL,
                    @EsPrincipal,
                    NULL,
                    1,
                    GETDATE()
                );
            ";


                    using SqlCommand cmd =
                        new SqlCommand(sql, con, trans);


                    cmd.Parameters.AddWithValue(
                        "@IdEmergencia",
                        idEmergencia);


                    cmd.Parameters.AddWithValue(
                        "@IdCIE10",
                        cie10.IdCIE10);


                    cmd.Parameters.AddWithValue(
                        "@EsPrincipal",
                        esPrincipal);


                    cmd.ExecuteNonQuery();
                }
            }


            // =====================================================
            // 2. DIAGNÓSTICOS MANUALES
            // =====================================================

            if (proceso.DiagnosticosManuales != null)
            {
                foreach (string diagnosticoManual
                         in proceso.DiagnosticosManuales)
                {
                    if (string.IsNullOrWhiteSpace(
                        diagnosticoManual))
                    {
                        continue;
                    }


                    // Si no existe principal CIE-10,
                    // el primer diagnóstico manual se considera principal.
                    bool esPrincipal = false;


                    if (proceso.DiagnosticoPrincipalCIE10 == null)
                    {
                        esPrincipal =
                            diagnosticoManual ==
                            proceso.DiagnosticosManuales.FirstOrDefault();
                    }


                    string sql = @"
                INSERT INTO tbEmergenciaDiagnosticos
                (
                    IdEmergencia,
                    IdCIE10,
                    DiagnosticoTexto,
                    EsPrincipal,
                    Observaciones,
                    Activo,
                    FechaRegistro
                )
                VALUES
                (
                    @IdEmergencia,
                    NULL,
                    @DiagnosticoTexto,
                    @EsPrincipal,
                    NULL,
                    1,
                    GETDATE()
                );
            ";


                    using SqlCommand cmd =
                        new SqlCommand(sql, con, trans);


                    cmd.Parameters.AddWithValue(
                        "@IdEmergencia",
                        idEmergencia);


                    cmd.Parameters.AddWithValue(
                        "@DiagnosticoTexto",
                        diagnosticoManual.Trim());


                    cmd.Parameters.AddWithValue(
                        "@EsPrincipal",
                        esPrincipal);


                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void InsertarProcedimientos(
                SqlConnection con,
                SqlTransaction trans,
                long idEmergencia,
                ProcedimientoEmergencia procedimiento)
        {
            string sql = @"
                INSERT INTO tbProcedimientosEmergencia
                (
                    IdEmergencia,
                    Medicamentos,
                    Procedimientos,
                    Laboratorios,
                    Imagenes
                )
                VALUES
                (
                    @IdEmergencia,
                    @Medicamentos,
                    @Procedimientos,
                    @Laboratorios,
                    @Imagenes
                );
                ";

            SqlCommand cmd = new(sql, con, trans);

            cmd.Parameters.AddWithValue("@IdEmergencia", idEmergencia);
            cmd.Parameters.AddWithValue("@Medicamentos", procedimiento.Medicamentos);
            cmd.Parameters.AddWithValue("@Procedimientos", procedimiento.Procedimientos);
            cmd.Parameters.AddWithValue("@Laboratorios", procedimiento.Laboratorios);
            cmd.Parameters.AddWithValue("@Imagenes", procedimiento.Imagenes);

            cmd.ExecuteNonQuery();
        }

        // =========================================================
        // INSERTAR ITEMS CLÍNICOS DE LA EMERGENCIA
        // =========================================================

        private void InsertarItemsClinicos(
            SqlConnection con,
            SqlTransaction trans,
            long idEmergencia,
            List<EmergenciaItem> items)
        {
            if (items == null || items.Count == 0)
                return;


            foreach (EmergenciaItem item in items)
            {
                string sql = @"
            INSERT INTO tbEmergenciaItems
            (
                IdEmergencia,
                IdItemClinico,
                Cantidad,
                PrecioUnitarioAplicado,
                IdPlanTarifarioAplicado,
                Estado,
                Observaciones,
                Activo,
                FechaRegistro
            )
            VALUES
            (
                @IdEmergencia,
                @IdItemClinico,
                @Cantidad,
                @PrecioUnitarioAplicado,
                @IdPlanTarifarioAplicado,
                @Estado,
                @Observaciones,
                1,
                GETDATE()
            );

            SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
        ";


                using SqlCommand cmd =
                    new SqlCommand(
                        sql,
                        con,
                        trans);


                cmd.Parameters.AddWithValue(
                    "@IdEmergencia",
                    idEmergencia);


                cmd.Parameters.AddWithValue(
                    "@IdItemClinico",
                    item.IdItemClinico);


                cmd.Parameters.AddWithValue(
                    "@Cantidad",
                    item.Cantidad);


                cmd.Parameters.AddWithValue(
                    "@PrecioUnitarioAplicado",
                    item.PrecioUnitarioAplicado);


                cmd.Parameters.AddWithValue(
                    "@IdPlanTarifarioAplicado",
                    item.IdPlanTarifarioAplicado
                        ?? (object)DBNull.Value);


                cmd.Parameters.AddWithValue(
                    "@Estado",
                    string.IsNullOrWhiteSpace(item.Estado)
                        ? "Registrado"
                        : item.Estado);


                cmd.Parameters.AddWithValue(
                    "@Observaciones",
                    string.IsNullOrWhiteSpace(
                        item.Observaciones)
                        ? DBNull.Value
                        : item.Observaciones);


                long idEmergenciaItem =
                    Convert.ToInt64(
                        cmd.ExecuteScalar());


                // =================================================
                // SI ES MEDICAMENTO, GUARDAR DATOS ESPECÍFICOS
                // =================================================

                if (item.TipoItem.Equals(
                    "Medicamento",
                    StringComparison.OrdinalIgnoreCase))
                {
                    InsertarDetalleMedicamento(
                        con,
                        trans,
                        idEmergenciaItem,
                        item);
                }
            }
        }

        // =========================================================
        // DETALLE ESPECÍFICO DEL MEDICAMENTO
        // =========================================================

        private void InsertarDetalleMedicamento(
            SqlConnection con,
            SqlTransaction trans,
            long idEmergenciaItem,
            EmergenciaItem item)
        {
            string sql = @"
        INSERT INTO tbEmergenciaMedicamentos
        (
            IdEmergenciaItem,
            Dosis,
            ViaAdministracion,
            Frecuencia,
            Indicaciones
        )
        VALUES
        (
            @IdEmergenciaItem,
            @Dosis,
            @ViaAdministracion,
            @Frecuencia,
            @Indicaciones
        );
    ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con,
                    trans);


            cmd.Parameters.AddWithValue(
                "@IdEmergenciaItem",
                idEmergenciaItem);


            cmd.Parameters.AddWithValue(
                "@Dosis",
                string.IsNullOrWhiteSpace(item.Dosis)
                    ? DBNull.Value
                    : item.Dosis);


            cmd.Parameters.AddWithValue(
                "@ViaAdministracion",
                string.IsNullOrWhiteSpace(
                    item.ViaAdministracion)
                    ? DBNull.Value
                    : item.ViaAdministracion);


            cmd.Parameters.AddWithValue(
                "@Frecuencia",
                string.IsNullOrWhiteSpace(
                    item.Frecuencia)
                    ? DBNull.Value
                    : item.Frecuencia);


            cmd.Parameters.AddWithValue(
                "@Indicaciones",
                string.IsNullOrWhiteSpace(
                    item.Indicaciones)
                    ? DBNull.Value
                    : item.Indicaciones);


            cmd.ExecuteNonQuery();
        }
        private void InsertarDestino(
                    SqlConnection con,
                    SqlTransaction trans,
                    long idEmergencia,
                    DestinoEmergencia destino)
        {
            string sql = @"

                    INSERT INTO tbDestinoEmergencia
                    (
                        IdEmergencia,
                        Destino,
                        ObservacionesFinales,
                        FechaSalida
                    )

                    VALUES
                    (
                        @IdEmergencia,
                        @Destino,
                        @ObservacionesFinales,
                        @FechaSalida
                    );

                    ";

            SqlCommand cmd = new(sql, con, trans);

            cmd.Parameters.AddWithValue("@IdEmergencia", idEmergencia);

            cmd.Parameters.AddWithValue("@Destino",
                destino.Destino);

            cmd.Parameters.AddWithValue("@ObservacionesFinales",
                destino.ObservacionesFinales);

            cmd.Parameters.AddWithValue("@FechaSalida",
                destino.FechaSalida ?? (object)DBNull.Value);

            cmd.ExecuteNonQuery();
        }


        private string GenerarCodigoEmergencia(SqlConnection con, SqlTransaction trans)
        {
            string sql = @"
        SELECT ISNULL(MAX(IdEmergencia),0) + 1
        FROM tbEmergencias";

            SqlCommand cmd = new(sql, con, trans);

            long siguiente = Convert.ToInt64(cmd.ExecuteScalar());

            return $"EM{siguiente:000000}";
        }


        public List<Emergencia> ObtenerEmergencias()
        {
            List<Emergencia> lista = new();

            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
        SELECT
            E.IdEmergencia,
            E.CodigoEmergencia,
            P.Nombre + ' ' + P.Apellido AS NombrePaciente,
            M.Nombre + ' ' + M.Apellido AS NombreMedico,
            ES.Nombre AS Especialidad,
            EE.Nombre AS Estado,
            E.FechaIngreso,
            E.MotivoConsulta
        FROM tbEmergencias E
            INNER JOIN tbPacientes P
                ON E.IdPaciente = P.IdPaciente
            INNER JOIN tbMedicos M
                ON E.IdMedico = M.IdMedico
            INNER JOIN tbEspecialidades ES
                ON E.IdEspecialidad = ES.IdEspecialidad
            INNER JOIN tbEstadoEmergencias EE
                ON E.IdEstadoEmergencia = EE.IdEstadoEmergencia
        ORDER BY E.IdEmergencia DESC";

                SqlCommand cmd = new(sql, con);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new Emergencia
                    {
                        IdEmergencia = Convert.ToInt64(dr["IdEmergencia"]),
                        CodigoEmergencia = dr["CodigoEmergencia"].ToString()!,
                        NombrePaciente = dr["NombrePaciente"].ToString()!,
                        NombreMedico = dr["NombreMedico"].ToString()!,
                        Especialidad = dr["Especialidad"].ToString()!,
                        Estado = dr["Estado"].ToString()!,
                        FechaIngreso = Convert.ToDateTime(dr["FechaIngreso"]),
                        MotivoConsulta = dr["MotivoConsulta"].ToString()!
                    });
                }
            }

            return lista;



        }

        public (Emergencia? Emergencia, ProcesoEmergencia? Proceso)
    ObtenerPorId(long idEmergencia)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();


                // =========================================================
                // EMERGENCIA + PACIENTE + MÉDICO + ESPECIALIDAD + ESTADO
                // =========================================================

                string sql = @"
            SELECT
                -- EMERGENCIA
                E.IdEmergencia,
                E.CodigoEmergencia,
                E.IdPaciente,
                E.IdMedico,
                E.IdEspecialidad,
                E.FechaIngreso,
                E.IdEstadoEmergencia,
                E.FechaCreacion,
                E.FechaModificacion,
                E.MotivoConsulta,

                -- PACIENTE
                P.IdPaciente AS Paciente_IdPaciente,
                P.CodigoPaciente AS Paciente_CodigoPaciente,

                LTRIM(RTRIM(
                    CONCAT(
                        P.Nombre, ' ',
                        P.SegundoNombre, ' ',
                        P.Apellido, ' ',
                        P.SegundoApellido
                    )
                )) AS NombrePaciente,

                P.Nombre AS Paciente_Nombre,
                P.SegundoNombre AS Paciente_SegundoNombre,
                P.Apellido AS Paciente_Apellido,
                P.SegundoApellido AS Paciente_SegundoApellido,
                P.TipoDocumento AS Paciente_TipoDocumento,
                P.NumeroDocumento AS Paciente_NumeroDocumento,
                P.CodigoTemporal AS Paciente_CodigoTemporal,
                P.Indocumentado AS Paciente_Indocumentado,
                P.FechaNacimiento AS Paciente_FechaNacimiento,
                P.Sexo AS Paciente_Sexo,
                P.Telefono AS Paciente_Telefono,
                P.Correo AS Paciente_Correo,
                P.Direccion AS Paciente_Direccion,
                P.IdTipoPaciente AS Paciente_IdTipoPaciente,
                P.IdSeguro AS Paciente_IdSeguro,
                S.Nombre AS Paciente_NombreSeguro,
                P.IdEstadoPaciente AS Paciente_IdEstadoPaciente,
                P.FechaCreacion AS Paciente_FechaCreacion,
                P.FechaModificacion AS Paciente_FechaModificacion,
                P.Activo AS Paciente_Activo,

                -- MÉDICO
                LTRIM(RTRIM(
                    CONCAT(M.Nombre, ' ', M.Apellido)
                )) AS NombreMedico,

                -- ESPECIALIDAD
                ES.Nombre AS Especialidad,

                -- ESTADO
                EE.Nombre AS Estado

            FROM tbEmergencias E

            INNER JOIN tbPacientes P
                ON E.IdPaciente = P.IdPaciente

            LEFT JOIN tbSeguros S
                ON P.IdSeguro = S.IdSeguro

            INNER JOIN tbMedicos M
                ON E.IdMedico = M.IdMedico

            INNER JOIN tbEspecialidades ES
                ON E.IdEspecialidad = ES.IdEspecialidad

            INNER JOIN tbEstadoEmergencia EE
                ON E.IdEstadoEmergencia = EE.IdEstadoEmergencia

            WHERE E.IdEmergencia = @IdEmergencia;
        ";


                using SqlCommand cmd = new(sql, con);

                cmd.Parameters.AddWithValue(
                    "@IdEmergencia",
                    idEmergencia);


                using SqlDataReader reader =
                    cmd.ExecuteReader();


                if (!reader.Read())
                {
                    return (null, null);
                }


                // =========================================================
                // EMERGENCIA
                // =========================================================

                Emergencia emergencia = new Emergencia
                {
                    IdEmergencia =
                        Convert.ToInt64(
                            reader["IdEmergencia"]),

                    CodigoEmergencia =
                        reader["CodigoEmergencia"]
                            .ToString() ?? "",

                    IdPaciente =
                        Convert.ToInt64(
                            reader["IdPaciente"]),

                    NombrePaciente =
                        reader["NombrePaciente"]
                            .ToString() ?? "",

                    IdMedico =
                        Convert.ToInt64(
                            reader["IdMedico"]),

                    NombreMedico =
                        reader["NombreMedico"]
                            .ToString() ?? "",

                    IdEspecialidad =
                        Convert.ToInt64(
                            reader["IdEspecialidad"]),

                    Especialidad =
                        reader["Especialidad"]
                            .ToString() ?? "",

                    FechaIngreso =
                        Convert.ToDateTime(
                            reader["FechaIngreso"]),

                    IdEstadoEmergencia =
                        Convert.ToInt64(
                            reader["IdEstadoEmergencia"]),

                    Estado =
                        reader["Estado"]
                            .ToString() ?? "",

                    FechaCreacion =
                        Convert.ToDateTime(
                            reader["FechaCreacion"]),

                    FechaModificacion =
                        reader["FechaModificacion"] == DBNull.Value
                            ? null
                            : Convert.ToDateTime(
                                reader["FechaModificacion"]),

                    MotivoConsulta =
                        reader["MotivoConsulta"]
                            .ToString() ?? ""
                };


                // =========================================================
                // PACIENTE
                // =========================================================

                Paciente paciente = new Paciente
                {
                    IdPaciente =
                        Convert.ToInt32(
                            reader["Paciente_IdPaciente"]),

                    CodigoPaciente =
                        reader["Paciente_CodigoPaciente"]
                            .ToString() ?? "",

                    Nombre =
                        reader["Paciente_Nombre"]
                            .ToString() ?? "",

                    SegundoNombre =
                        reader["Paciente_SegundoNombre"] == DBNull.Value
                            ? null
                            : reader["Paciente_SegundoNombre"].ToString(),

                    Apellido =
                        reader["Paciente_Apellido"]
                            .ToString() ?? "",

                    SegundoApellido =
                        reader["Paciente_SegundoApellido"] == DBNull.Value
                            ? null
                            : reader["Paciente_SegundoApellido"].ToString(),

                    TipoDocumento =
                        reader["Paciente_TipoDocumento"] == DBNull.Value
                            ? ""
                            : reader["Paciente_TipoDocumento"].ToString() ?? "",

                    NumeroDocumento =
                        reader["Paciente_NumeroDocumento"] == DBNull.Value
                            ? ""
                            : reader["Paciente_NumeroDocumento"].ToString() ?? "",

                    CodigoTemporal =
                        reader["Paciente_CodigoTemporal"] == DBNull.Value
                            ? ""
                            : reader["Paciente_CodigoTemporal"].ToString() ?? "",

                    Indocumentado =
                        reader["Paciente_Indocumentado"] != DBNull.Value &&
                        Convert.ToBoolean(
                            reader["Paciente_Indocumentado"]),

                    FechaNacimiento =
                        Convert.ToDateTime(
                            reader["Paciente_FechaNacimiento"]),

                    Sexo =
                        reader["Paciente_Sexo"] == DBNull.Value
                            ? ""
                            : reader["Paciente_Sexo"].ToString() ?? "",

                    Telefono =
                        reader["Paciente_Telefono"] == DBNull.Value
                            ? ""
                            : reader["Paciente_Telefono"].ToString() ?? "",

                    Correo =
                        reader["Paciente_Correo"] == DBNull.Value
                            ? null
                            : reader["Paciente_Correo"].ToString(),

                    Direccion =
                        reader["Paciente_Direccion"] == DBNull.Value
                            ? ""
                            : reader["Paciente_Direccion"].ToString() ?? "",

                    IdTipoPaciente =
                        Convert.ToInt32(
                            reader["Paciente_IdTipoPaciente"]),

                    IdSeguro =
                        reader["Paciente_IdSeguro"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(
                                reader["Paciente_IdSeguro"]),

                    NombreSeguro =
                        reader["Paciente_NombreSeguro"] == DBNull.Value
                            ? "Sin seguro"
                            : reader["Paciente_NombreSeguro"].ToString()
                                ?? "Sin seguro",

                    IdEstadoPaciente =
                        Convert.ToInt32(
                            reader["Paciente_IdEstadoPaciente"]),

                    FechaCreacion =
                        Convert.ToDateTime(
                            reader["Paciente_FechaCreacion"]),

                    FechaModificacion =
                        reader["Paciente_FechaModificacion"] == DBNull.Value
                            ? null
                            : Convert.ToDateTime(
                                reader["Paciente_FechaModificacion"]),

                    Activo =
                        Convert.ToBoolean(
                            reader["Paciente_Activo"])
                };


                reader.Close();


                // =========================================================
                // EVALUACIÓN INICIAL
                // =========================================================

                EvaluacionInicial evaluacion =
                    new EvaluacionInicial();


                string sqlEvaluacion = @"
            SELECT TOP 1
                IdEvaluacion,
                IdEmergencia,
                NivelTriage,
                Temperatura,
                PresionArterial,
                FrecuenciaCardiaca,
                FrecuenciaRespiratoria,
                Saturacion,
                Glucemia,
                Peso,
                Talla,
                FechaRegistro

            FROM tbEvaluacionInicial

            WHERE IdEmergencia =
                  @IdEmergencia

            ORDER BY IdEvaluacion DESC;
        ";


                using (SqlCommand cmdEvaluacion =
                       new SqlCommand(
                           sqlEvaluacion,
                           con))
                {
                    cmdEvaluacion.Parameters.AddWithValue(
                        "@IdEmergencia",
                        idEmergencia);


                    using SqlDataReader r =
                        cmdEvaluacion.ExecuteReader();


                    if (r.Read())
                    {
                        evaluacion.IdEvaluacion =
                            Convert.ToInt64(
                                r["IdEvaluacion"]);

                        evaluacion.IdEmergencia =
                            Convert.ToInt64(
                                r["IdEmergencia"]);

                        evaluacion.NivelTriage =
                            Convert.ToInt32(
                                r["NivelTriage"]);

                        evaluacion.Temperatura =
                            r["Temperatura"] == DBNull.Value
                                ? null
                                : Convert.ToDecimal(
                                    r["Temperatura"]);

                        evaluacion.PresionArterial =
                            r["PresionArterial"] == DBNull.Value
                                ? ""
                                : r["PresionArterial"].ToString() ?? "";

                        evaluacion.FrecuenciaCardiaca =
                            r["FrecuenciaCardiaca"] == DBNull.Value
                                ? null
                                : Convert.ToInt32(
                                    r["FrecuenciaCardiaca"]);

                        evaluacion.FrecuenciaRespiratoria =
                            r["FrecuenciaRespiratoria"] == DBNull.Value
                                ? null
                                : Convert.ToInt32(
                                    r["FrecuenciaRespiratoria"]);

                        evaluacion.Saturacion =
                            r["Saturacion"] == DBNull.Value
                                ? null
                                : Convert.ToInt32(
                                    r["Saturacion"]);

                        evaluacion.Glucemia =
                            r["Glucemia"] == DBNull.Value
                                ? null
                                : Convert.ToDecimal(
                                    r["Glucemia"]);

                        evaluacion.Peso =
                            r["Peso"] == DBNull.Value
                                ? null
                                : Convert.ToDecimal(
                                    r["Peso"]);

                        evaluacion.Talla =
                            r["Talla"] == DBNull.Value
                                ? null
                                : Convert.ToDecimal(
                                    r["Talla"]);

                        evaluacion.FechaRegistro =
                            Convert.ToDateTime(
                                r["FechaRegistro"]);
                    }
                }


                // =========================================================
                // INFORMACIÓN CLÍNICA
                // =========================================================

                InformacionClinica informacion =
                    new InformacionClinica();


                string sqlInformacion = @"
            SELECT TOP 1
                IdInformacion AS IdInformacionClinica,
                IdEmergencia,
                MotivoConsulta,
                Diabetes,
                Hipertension,
                Asma,
                Cardiopatia,
                Embarazo,
                Ninguno,
                Alergias,
                MedicamentosActuales,
                Observaciones

            FROM tbInformacionClinica

            WHERE IdEmergencia =
                  @IdEmergencia

            ORDER BY IdInformacion DESC;
        ";


                using (SqlCommand cmdInformacion =
                       new SqlCommand(
                           sqlInformacion,
                           con))
                {
                    cmdInformacion.Parameters.AddWithValue(
                        "@IdEmergencia",
                        idEmergencia);


                    using SqlDataReader r =
                        cmdInformacion.ExecuteReader();


                    if (r.Read())
                    {
                        informacion.IdInformacionClinica =
                            Convert.ToInt64(
                                r["IdInformacionClinica"]);

                        informacion.IdEmergencia =
                            Convert.ToInt64(
                                r["IdEmergencia"]);

                        informacion.MotivoConsulta =
                            r["MotivoConsulta"].ToString() ?? "";

                        informacion.Diabetes =
                            Convert.ToBoolean(
                                r["Diabetes"]);

                        informacion.Hipertension =
                            Convert.ToBoolean(
                                r["Hipertension"]);

                        informacion.Asma =
                            Convert.ToBoolean(
                                r["Asma"]);

                        informacion.Cardiopatia =
                            Convert.ToBoolean(
                                r["Cardiopatia"]);

                        informacion.Embarazo =
                            Convert.ToBoolean(
                                r["Embarazo"]);

                        informacion.Ninguno =
                            Convert.ToBoolean(
                                r["Ninguno"]);

                        informacion.Alergias =
                            r["Alergias"] == DBNull.Value
                                ? ""
                                : r["Alergias"].ToString() ?? "";

                        informacion.MedicamentosActuales =
                            r["MedicamentosActuales"] == DBNull.Value
                                ? ""
                                : r["MedicamentosActuales"].ToString() ?? "";

                        informacion.Observaciones =
                            r["Observaciones"] == DBNull.Value
                                ? ""
                                : r["Observaciones"].ToString() ?? "";
                    }
                }


                // =========================================================
                // DIAGNÓSTICO ANTIGUO / INFORMACIÓN MÉDICA
                // =========================================================

                DiagnosticoEmergencia diagnostico =
                    new DiagnosticoEmergencia();


                string sqlDiagnostico = @"
            SELECT TOP 1
                IdDiagnostico,
                IdEmergencia,
                DiagnosticoPrincipal,
                DiagnosticoSecundario,
                ImpresionClinica,
                Observaciones

            FROM tbDiagnosticoEmergencia

            WHERE IdEmergencia =
                  @IdEmergencia

            ORDER BY IdDiagnostico DESC;
        ";


                using (SqlCommand cmdDiagnostico =
                       new SqlCommand(
                           sqlDiagnostico,
                           con))
                {
                    cmdDiagnostico.Parameters.AddWithValue(
                        "@IdEmergencia",
                        idEmergencia);


                    using SqlDataReader r =
                        cmdDiagnostico.ExecuteReader();


                    if (r.Read())
                    {
                        diagnostico.IdDiagnostico =
                            Convert.ToInt64(
                                r["IdDiagnostico"]);

                        diagnostico.IdEmergencia =
                            Convert.ToInt64(
                                r["IdEmergencia"]);

                        diagnostico.DiagnosticoPrincipal =
                            r["DiagnosticoPrincipal"] == DBNull.Value
                                ? ""
                                : r["DiagnosticoPrincipal"].ToString() ?? "";

                        diagnostico.DiagnosticoSecundario =
                            r["DiagnosticoSecundario"] == DBNull.Value
                                ? ""
                                : r["DiagnosticoSecundario"].ToString() ?? "";

                        diagnostico.ImpresionClinica =
                            r["ImpresionClinica"] == DBNull.Value
                                ? ""
                                : r["ImpresionClinica"].ToString() ?? "";

                        diagnostico.Observaciones =
                            r["Observaciones"] == DBNull.Value
                                ? ""
                                : r["Observaciones"].ToString() ?? "";
                    }
                }


                // =========================================================
                // DIAGNÓSTICOS CIE-10 NORMALIZADOS
                // =========================================================

                List<CIE10> diagnosticosSeleccionados =
                    new List<CIE10>();


                CIE10? diagnosticoPrincipalCIE10 =
                    null;


                List<string> diagnosticosManuales =
                    new List<string>();


                string sqlCIE10 = @"
            SELECT
                ED.IdEmergenciaDiagnostico,
                ED.IdCIE10,
                ED.DiagnosticoTexto,
                ED.EsPrincipal,

                C.Codigo,
                C.Descripcion,
                C.Categoria,
                C.Activo

            FROM tbEmergenciaDiagnosticos ED

            LEFT JOIN tbCIE10 C
                ON ED.IdCIE10 =
                   C.IdCIE10

            WHERE ED.IdEmergencia =
                  @IdEmergencia

              AND ED.Activo = 1

            ORDER BY
                ED.EsPrincipal DESC,
                ED.IdEmergenciaDiagnostico;
        ";


                using (SqlCommand cmdCIE =
                       new SqlCommand(
                           sqlCIE10,
                           con))
                {
                    cmdCIE.Parameters.AddWithValue(
                        "@IdEmergencia",
                        idEmergencia);


                    using SqlDataReader r =
                        cmdCIE.ExecuteReader();


                    while (r.Read())
                    {
                        // CATÁLOGO CIE-10

                        if (r["IdCIE10"] != DBNull.Value)
                        {
                            CIE10 cie = new CIE10
                            {
                                IdCIE10 =
                                    Convert.ToInt64(
                                        r["IdCIE10"]),

                                Codigo =
                                    r["Codigo"] == DBNull.Value
                                        ? ""
                                        : r["Codigo"].ToString() ?? "",

                                Descripcion =
                                    r["Descripcion"] == DBNull.Value
                                        ? ""
                                        : r["Descripcion"].ToString() ?? "",

                                Categoria =
                                    r["Categoria"] == DBNull.Value
                                        ? ""
                                        : r["Categoria"].ToString() ?? "",

                                Activo =
                                    r["Activo"] != DBNull.Value &&
                                    Convert.ToBoolean(
                                        r["Activo"])
                            };


                            diagnosticosSeleccionados.Add(
                                cie);


                            bool esPrincipal =
                                r["EsPrincipal"] != DBNull.Value &&
                                Convert.ToBoolean(
                                    r["EsPrincipal"]);


                            if (esPrincipal)
                            {
                                diagnosticoPrincipalCIE10 =
                                    cie;
                            }
                        }

                        // DIAGNÓSTICO MANUAL

                        else if (
                            r["DiagnosticoTexto"] != DBNull.Value)
                        {
                            string texto =
                                r["DiagnosticoTexto"]
                                    .ToString() ?? "";


                            if (!string.IsNullOrWhiteSpace(
                                texto))
                            {
                                diagnosticosManuales.Add(
                                    texto.Trim());
                            }
                        }
                    }
                }


                // =========================================================
                // PROCEDIMIENTOS ANTIGUOS
                // =========================================================

                ProcedimientoEmergencia procedimientos =
                    new ProcedimientoEmergencia();


                string sqlProcedimientos = @"
            SELECT TOP 1
                IdProcedimiento,
                IdEmergencia,
                Medicamentos,
                Procedimientos,
                Laboratorios,
                Imagenes

            FROM tbProcedimientosEmergencia

            WHERE IdEmergencia =
                  @IdEmergencia

            ORDER BY IdProcedimiento DESC;
        ";


                using (SqlCommand cmdProcedimientos =
                       new SqlCommand(
                           sqlProcedimientos,
                           con))
                {
                    cmdProcedimientos.Parameters.AddWithValue(
                        "@IdEmergencia",
                        idEmergencia);


                    using SqlDataReader r =
                        cmdProcedimientos.ExecuteReader();


                    if (r.Read())
                    {
                        procedimientos.IdProcedimiento =
                            Convert.ToInt64(
                                r["IdProcedimiento"]);

                        procedimientos.IdEmergencia =
                            Convert.ToInt64(
                                r["IdEmergencia"]);

                        procedimientos.Medicamentos =
                            r["Medicamentos"] == DBNull.Value
                                ? ""
                                : r["Medicamentos"].ToString() ?? "";

                        procedimientos.Procedimientos =
                            r["Procedimientos"] == DBNull.Value
                                ? ""
                                : r["Procedimientos"].ToString() ?? "";

                        procedimientos.Laboratorios =
                            r["Laboratorios"] == DBNull.Value
                                ? ""
                                : r["Laboratorios"].ToString() ?? "";

                        procedimientos.Imagenes =
                            r["Imagenes"] == DBNull.Value
                                ? ""
                                : r["Imagenes"].ToString() ?? "";
                    }
                }


                // =========================================================
                // ITEMS CLÍNICOS / FACTURABLES
                // =========================================================

                List<EmergenciaItem> itemsClinicos =
                    new List<EmergenciaItem>();


                string sqlItems = @"
            SELECT
                EI.IdEmergenciaItem,
                EI.IdEmergencia,
                EI.IdItemClinico,
                EI.Cantidad,
                EI.PrecioUnitarioAplicado,
                EI.IdPlanTarifarioAplicado,
                EI.Estado,
                EI.Observaciones,

                I.Codigo,
                I.Nombre,

                T.Nombre AS TipoItem,

                PT.Nombre AS NombrePlanTarifario,

                EM.Dosis,
                EM.ViaAdministracion,
                EM.Frecuencia,
                EM.Indicaciones

            FROM tbEmergenciaItems EI

            INNER JOIN tbItemsClinicos I
                ON EI.IdItemClinico =
                   I.IdItemClinico

            INNER JOIN tbTiposItemClinico T
                ON I.IdTipoItem =
                   T.IdTipoItem

            LEFT JOIN tbPlanesTarifarios PT
                ON EI.IdPlanTarifarioAplicado =
                   PT.IdPlanTarifario

            LEFT JOIN tbEmergenciaMedicamentos EM
                ON EI.IdEmergenciaItem =
                   EM.IdEmergenciaItem

            WHERE EI.IdEmergencia =
                  @IdEmergencia

              AND EI.Activo = 1

            ORDER BY
                EI.IdEmergenciaItem;
        ";


                using (SqlCommand cmdItems =
                       new SqlCommand(
                           sqlItems,
                           con))
                {
                    cmdItems.Parameters.AddWithValue(
                        "@IdEmergencia",
                        idEmergencia);


                    using SqlDataReader r =
                        cmdItems.ExecuteReader();


                    while (r.Read())
                    {
                        EmergenciaItem item =
                            new EmergenciaItem
                            {
                                IdEmergenciaItem =
                                    Convert.ToInt64(
                                        r["IdEmergenciaItem"]),

                                IdEmergencia =
                                    Convert.ToInt64(
                                        r["IdEmergencia"]),

                                IdItemClinico =
                                    Convert.ToInt64(
                                        r["IdItemClinico"]),

                                Codigo =
                                    r["Codigo"] == DBNull.Value
                                        ? ""
                                        : r["Codigo"].ToString() ?? "",

                                Nombre =
                                    r["Nombre"] == DBNull.Value
                                        ? ""
                                        : r["Nombre"].ToString() ?? "",

                                TipoItem =
                                    r["TipoItem"] == DBNull.Value
                                        ? ""
                                        : r["TipoItem"].ToString() ?? "",

                                Cantidad =
                                    Convert.ToDecimal(
                                        r["Cantidad"]),

                                PrecioUnitarioAplicado =
                                    Convert.ToDecimal(
                                        r["PrecioUnitarioAplicado"]),

                                IdPlanTarifarioAplicado =
                                    r["IdPlanTarifarioAplicado"]
                                        == DBNull.Value
                                        ? null
                                        : Convert.ToInt32(
                                            r["IdPlanTarifarioAplicado"]),

                                NombrePlanTarifario =
                                    r["NombrePlanTarifario"]
                                        == DBNull.Value
                                        ? ""
                                        : r["NombrePlanTarifario"]
                                            .ToString() ?? "",

                                Estado =
                                    r["Estado"] == DBNull.Value
                                        ? "Registrado"
                                        : r["Estado"].ToString()
                                            ?? "Registrado",

                                Observaciones =
                                    r["Observaciones"] == DBNull.Value
                                        ? ""
                                        : r["Observaciones"]
                                            .ToString() ?? "",

                                Dosis =
                                    r["Dosis"] == DBNull.Value
                                        ? ""
                                        : r["Dosis"].ToString() ?? "",

                                ViaAdministracion =
                                    r["ViaAdministracion"] == DBNull.Value
                                        ? ""
                                        : r["ViaAdministracion"]
                                            .ToString() ?? "",

                                Frecuencia =
                                    r["Frecuencia"] == DBNull.Value
                                        ? ""
                                        : r["Frecuencia"]
                                            .ToString() ?? "",

                                Indicaciones =
                                    r["Indicaciones"] == DBNull.Value
                                        ? ""
                                        : r["Indicaciones"]
                                            .ToString() ?? ""
                            };


                        itemsClinicos.Add(
                            item);
                    }
                }


                // =========================================================
                // DESTINO
                // =========================================================

                DestinoEmergencia destino =
                    new DestinoEmergencia();


                string sqlDestino = @"
            SELECT TOP 1
                IdDestino,
                IdEmergencia,
                Destino,
                ObservacionesFinales,
                FechaSalida

            FROM tbDestinoEmergencia

            WHERE IdEmergencia =
                  @IdEmergencia

            ORDER BY IdDestino DESC;
        ";


                using (SqlCommand cmdDestino =
                       new SqlCommand(
                           sqlDestino,
                           con))
                {
                    cmdDestino.Parameters.AddWithValue(
                        "@IdEmergencia",
                        idEmergencia);


                    using SqlDataReader r =
                        cmdDestino.ExecuteReader();


                    if (r.Read())
                    {
                        destino.IdDestino =
                            Convert.ToInt64(
                                r["IdDestino"]);

                        destino.IdEmergencia =
                            Convert.ToInt64(
                                r["IdEmergencia"]);

                        destino.Destino =
                            r["Destino"] == DBNull.Value
                                ? ""
                                : r["Destino"].ToString() ?? "";

                        destino.ObservacionesFinales =
                            r["ObservacionesFinales"] == DBNull.Value
                                ? ""
                                : r["ObservacionesFinales"]
                                    .ToString() ?? "";

                        destino.FechaSalida =
                            r["FechaSalida"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(
                                    r["FechaSalida"]);
                    }
                }


                // =========================================================
                // ARMAR PROCESO COMPLETO
                // =========================================================

                ProcesoEmergencia proceso =
                    new ProcesoEmergencia
                    {
                        Paciente =
                            paciente,

                        Evaluacion =
                            evaluacion,

                        InformacionClinica =
                            informacion,

                        Diagnostico =
                            diagnostico,

                        Procedimientos =
                            procedimientos,

                        Destino =
                            destino,

                        DiagnosticosSeleccionados =
                            diagnosticosSeleccionados,

                        DiagnosticoPrincipalCIE10 =
                            diagnosticoPrincipalCIE10,

                        DiagnosticosManuales =
                            diagnosticosManuales,

                        ItemsClinicos =
                            itemsClinicos,

                        FechaRegistro =
                            emergencia.FechaIngreso
                    };


                return (
                    emergencia,
                    proceso);
            }
        }


        // =========================================================
        // ACTUALIZAR ESTADO DE LA EMERGENCIA
        // =========================================================

        public bool ActualizarEstado(
    long idEmergencia,
    long idEstadoEmergencia)
        {
            using (SqlConnection con = conexion.ObtenerConexion())
            {
                con.Open();

                string sql = @"
            UPDATE tbEmergencias
            SET
                IdEstadoEmergencia = @IdEstadoEmergencia,
                FechaModificacion = GETDATE()
            WHERE IdEmergencia = @IdEmergencia;
        ";

                using SqlCommand cmd = new(sql, con);

                cmd.Parameters.AddWithValue(
                    "@IdEmergencia",
                    idEmergencia);

                cmd.Parameters.AddWithValue(
                    "@IdEstadoEmergencia",
                    idEstadoEmergencia);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}