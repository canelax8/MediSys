using Microsoft.Data.SqlClient;
using Proyecto_MediSys.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Proyecto_MediSys.Pages.Dashboard
{
    public partial class DashboardPage : Page
    {
        // ============================================================
        // CONEXIÓN
        // ============================================================

        private readonly Conexion conexion =
            new Conexion();


        // ============================================================
        // FACTURACIÓN
        // ============================================================

        private readonly FacturacionDAO facturacionDAO =
            new FacturacionDAO();


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public DashboardPage()
        {
            InitializeComponent();

            CargarDashboard();
        }


        // ============================================================
        // CARGAR DASHBOARD COMPLETO
        // ============================================================

        private void CargarDashboard()
        {
            try
            {
                using SqlConnection con =
                    conexion.ObtenerConexion();

                con.Open();


                // ====================================================
                // PACIENTES
                // ====================================================

                int pacientes =
                    EjecutarEntero(
                        con,
                        @"
                        SELECT COUNT(*)
                        FROM tbPacientes
                        WHERE Activo = 1;
                        ");


                // ====================================================
                // EMERGENCIAS DE HOY
                // ====================================================

                int emergenciasHoy =
                    EjecutarEntero(
                        con,
                        @"
                        SELECT COUNT(*)
                        FROM tbEmergencias
                        WHERE
                            CAST(FechaIngreso AS DATE) =
                            CAST(GETDATE() AS DATE);
                        ");


                // ====================================================
                // CITAS DE HOY
                // ====================================================

                int citasHoy =
                    EjecutarEntero(
                        con,
                        @"
                        SELECT COUNT(*)
                        FROM tbCitas
                        WHERE
                            CAST(FechaCita AS DATE) =
                            CAST(GETDATE() AS DATE)

                            AND Activo = 1;
                        ");


                // ====================================================
                // INTERNADOS ACTIVOS
                // ====================================================

                int internadosActivos =
                    EjecutarEntero(
                        con,
                        @"
                        SELECT COUNT(*)

                        FROM tbInternamientos I

                        INNER JOIN tbEstadosInternamiento E
                            ON I.IdEstadoInternamiento =
                               E.IdEstadoInternamiento

                        WHERE
                            I.Activo = 1
                            AND E.Nombre = 'Activo';
                        ");


                // ====================================================
                // FACTURAS DE HOY
                // ====================================================

                int facturasHoy =
                    EjecutarEntero(
                        con,
                        @"
                        SELECT COUNT(*)
                        FROM tbFacturas
                        WHERE
                            Activo = 1

                            AND CAST(FechaFactura AS DATE) =
                                CAST(GETDATE() AS DATE);
                        ");


                // ====================================================
                // CUENTAS PENDIENTES DE FACTURAR
                //
                // Utilizamos exactamente la misma lógica del módulo
                // Facturación.
                // ====================================================

                int cuentasPendientes = 0;


                try
                {
                    cuentasPendientes +=
                        facturacionDAO
                            .ObtenerEmergenciasPendientes()
                            .Count;


                    cuentasPendientes +=
                        facturacionDAO
                            .ObtenerInternamientosPendientes()
                            .Count;
                }
                catch
                {
                    // Si existe algún cargo mal configurado,
                    // evitamos que todo el Dashboard deje de cargar.
                    cuentasPendientes = 0;
                }


                // ====================================================
                // MOSTRAR INDICADORES
                // ====================================================

                txtTotalPacientes.Text =
                    pacientes.ToString();


                txtEmergenciasHoy.Text =
                    emergenciasHoy.ToString();


                txtCitasHoy.Text =
                    citasHoy.ToString();


                txtInternadosActivos.Text =
                    internadosActivos.ToString();


                txtFacturasHoy.Text =
                    facturasHoy.ToString();


                txtCuentasPendientes.Text =
                    cuentasPendientes.ToString();


                // ====================================================
                // RESUMEN DERECHO
                // ====================================================

                txtResumenEmergencias.Text =
                    emergenciasHoy.ToString();


                txtResumenInternados.Text =
                    internadosActivos.ToString();


                txtResumenCitas.Text =
                    citasHoy.ToString();


                txtResumenFacturas.Text =
                    facturasHoy.ToString();


                // ====================================================
                // ACTIVIDAD RECIENTE
                // ====================================================

                CargarActividadReciente(
                    con);


                // ====================================================
                // ÚLTIMA ACTUALIZACIÓN
                // ====================================================

                txtUltimaActualizacion.Text =
                    DateTime.Now.ToString(
                        "dd/MM/yyyy hh:mm:ss tt");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"No fue posible cargar el Dashboard.\n\n{ex.Message}",
                    "MediSys",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }


        // ============================================================
        // EJECUTAR COUNT
        // ============================================================

        private int EjecutarEntero(
            SqlConnection con,
            string sql)
        {
            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            object? resultado =
                cmd.ExecuteScalar();


            if (resultado == null ||
                resultado == DBNull.Value)
            {
                return 0;
            }


            return Convert.ToInt32(
                resultado);
        }


        // ============================================================
        // ACTIVIDAD RECIENTE
        // ============================================================

        private void CargarActividadReciente(
            SqlConnection con)
        {
            List<ActividadDashboard> actividades =
                new List<ActividadDashboard>();


            string sql = @"
                SELECT TOP 15
                    X.Fecha,
                    X.Modulo,
                    X.Referencia,
                    X.Descripcion,
                    X.Estado

                FROM
                (

                    /* ===============================================
                       FACTURAS
                    =============================================== */

                    SELECT

                        F.FechaFactura
                            AS Fecha,

                        'Facturación'
                            AS Modulo,

                        F.CodigoFactura
                            AS Referencia,

                        CONCAT(
                            F.TipoFactura,
                            ' - ',
                            ISNULL(
                                F.CodigoOrigen,
                                ''
                            )
                        ) AS Descripcion,

                        EF.Nombre
                            AS Estado

                    FROM tbFacturas F

                    INNER JOIN tbEstadosFactura EF
                        ON F.IdEstadoFactura =
                           EF.IdEstadoFactura

                    WHERE
                        F.Activo = 1


                    UNION ALL


                    /* ===============================================
                       INTERNAMIENTOS
                    =============================================== */

                    SELECT

                        I.FechaIngreso
                            AS Fecha,

                        'Internamiento'
                            AS Modulo,

                        I.CodigoInternamiento
                            AS Referencia,

                        CONCAT(
                            'Ingreso de ',
                            P.Nombre,
                            ' ',
                            P.Apellido
                        ) AS Descripcion,

                        EI.Nombre
                            AS Estado

                    FROM tbInternamientos I

                    INNER JOIN tbPacientes P
                        ON I.IdPaciente =
                           P.IdPaciente

                    INNER JOIN tbEstadosInternamiento EI
                        ON I.IdEstadoInternamiento =
                           EI.IdEstadoInternamiento

                    WHERE
                        I.Activo = 1


                    UNION ALL


                    /* ===============================================
                       EMERGENCIAS
                    =============================================== */

                    SELECT

                        E.FechaIngreso
                            AS Fecha,

                        'Emergencia'
                            AS Modulo,

                        E.CodigoEmergencia
                            AS Referencia,

                        CONCAT(
                            'Atención de ',
                            P.Nombre,
                            ' ',
                            P.Apellido
                        ) AS Descripcion,

                        EE.Nombre
                            AS Estado

                    FROM tbEmergencias E

                    INNER JOIN tbPacientes P
                        ON E.IdPaciente =
                           P.IdPaciente

                    INNER JOIN tbEstadoEmergencia EE
                        ON E.IdEstadoEmergencia =
                           EE.IdEstadoEmergencia


                    UNION ALL


                    /* ===============================================
                       PACIENTES
                    =============================================== */

                    SELECT

                        P.FechaCreacion
                            AS Fecha,

                        'Pacientes'
                            AS Modulo,

                        P.CodigoPaciente
                            AS Referencia,

                        CONCAT(
                            'Registro de ',
                            P.Nombre,
                            ' ',
                            P.Apellido
                        ) AS Descripcion,

                        CASE
                            WHEN P.Activo = 1
                                THEN 'Activo'
                            ELSE
                                'Inactivo'
                        END AS Estado

                    FROM tbPacientes P

                ) X


                ORDER BY
                    X.Fecha DESC;
            ";


            using SqlCommand cmd =
                new SqlCommand(
                    sql,
                    con);


            using SqlDataReader reader =
                cmd.ExecuteReader();


            while (reader.Read())
            {
                actividades.Add(
                    new ActividadDashboard
                    {
                        Fecha =
                            Convert.ToDateTime(
                                reader["Fecha"]),

                        Modulo =
                            reader["Modulo"]
                                .ToString() ?? "",

                        Referencia =
                            reader["Referencia"]
                                .ToString() ?? "",

                        Descripcion =
                            reader["Descripcion"]
                                .ToString() ?? "",

                        Estado =
                            reader["Estado"]
                                .ToString() ?? ""
                    });
            }


            dgActividadReciente.ItemsSource =
                actividades;
        }


        // ============================================================
        // ACTUALIZAR
        // ============================================================

        private void btnActualizarDashboard_Click(
            object sender,
            RoutedEventArgs e)
        {
            CargarDashboard();
        }
    }


    // ================================================================
    // MODELO PARA ACTIVIDAD RECIENTE
    // ================================================================

    public class ActividadDashboard
    {
        public DateTime Fecha { get; set; }

        public string Modulo { get; set; } = "";

        public string Referencia { get; set; } = "";

        public string Descripcion { get; set; } = "";

        public string Estado { get; set; } = "";


        public string FechaTexto =>
            Fecha.ToString(
                "dd/MM/yyyy hh:mm tt");
    }
}