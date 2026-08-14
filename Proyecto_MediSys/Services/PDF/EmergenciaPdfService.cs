using Proyecto_MediSys.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Proyecto_MediSys.Services.PDF
{
    public class EmergenciaPdfService
    {
        public string Generar(
            Emergencia emergencia,
            ProcesoEmergencia proceso,
            string rutaArchivo)
        {
            if (emergencia == null)
                throw new ArgumentNullException(nameof(emergencia));

            if (proceso == null)
                throw new ArgumentNullException(nameof(proceso));

            if (string.IsNullOrWhiteSpace(rutaArchivo))
                throw new ArgumentException(
                    "Debe especificar una ruta para guardar el PDF.",
                    nameof(rutaArchivo));


            Document
                .Create(document =>
                {
                    document.Page(page =>
                    {
                        page.Size(PageSizes.Letter);

                        page.Margin(35);

                        page.PageColor(
                            Colors.White);

                        page.DefaultTextStyle(
                            x => x
                                .FontSize(9)
                                .FontColor("#243447"));


                        // =================================================
                        // HEADER
                        // =================================================

                        page.Header()
                            .Element(container =>
                            {
                                CrearEncabezado(
                                    container,
                                    emergencia);
                            });


                        // =================================================
                        // CONTENIDO
                        // =================================================

                        page.Content()
                            .PaddingVertical(18)
                            .Column(column =>
                            {
                                column.Spacing(12);


                                // PACIENTE

                                column.Item()
                                    .Element(container =>
                                    {
                                        CrearPaciente(
                                            container,
                                            proceso);
                                    });


                                // EVALUACIÓN

                                column.Item()
                                    .Element(container =>
                                    {
                                        CrearEvaluacion(
                                            container,
                                            proceso);
                                    });


                                // INFORMACIÓN CLÍNICA

                                column.Item()
                                    .Element(container =>
                                    {
                                        CrearInformacionClinica(
                                            container,
                                            proceso);
                                    });


                                // DIAGNÓSTICOS

                                column.Item()
                                    .Element(container =>
                                    {
                                        CrearDiagnosticos(
                                            container,
                                            proceso);
                                    });


                                // TRATAMIENTO Y SERVICIOS

                                column.Item()
                                    .Element(container =>
                                    {
                                        CrearItemsClinicos(
                                            container,
                                            proceso);
                                    });


                                // DESTINO

                                column.Item()
                                    .Element(container =>
                                    {
                                        CrearDestino(
                                            container,
                                            proceso);
                                    });


                                // MÉDICO

                                column.Item()
                                    .Element(container =>
                                    {
                                        CrearMedico(
                                            container,
                                            emergencia);
                                    });
                            });


                        // =================================================
                        // FOOTER
                        // =================================================

                        page.Footer()
                            .AlignCenter()
                            .Text(text =>
                            {
                                text.DefaultTextStyle(
                                    x => x
                                        .FontSize(8)
                                        .FontColor(
                                            Colors.Grey.Medium));


                                text.Span(
                                    "MediSys | Expediente de emergencia | Página ");


                                text.CurrentPageNumber();


                                text.Span(
                                    " de ");


                                text.TotalPages();
                            });
                    });
                })
                .GeneratePdf(
                    rutaArchivo);


            return rutaArchivo;
        }


        // =========================================================
        // ENCABEZADO
        // =========================================================

        private void CrearEncabezado(
            IContainer container,
            Emergencia emergencia)
        {
            container
                .BorderBottom(1)
                .BorderColor("#D9E2EC")
                .PaddingBottom(12)
                .Row(row =>
                {
                    row.RelativeItem()
                        .Column(column =>
                        {
                            column.Item()
                                .Text(
                                    "MEDISYS")
                                .FontSize(10)
                                .SemiBold()
                                .FontColor(
                                    "#2457A6");


                            column.Item()
                                .PaddingTop(3)
                                .Text(
                                    "EXPEDIENTE DE EMERGENCIA")
                                .FontSize(18)
                                .Bold()
                                .FontColor(
                                    "#1E3A5F");


                            column.Item()
                                .PaddingTop(3)
                                .Text(
                                    emergencia.NombrePaciente)
                                .FontSize(11)
                                .SemiBold();
                        });


                    row.ConstantItem(150)
                        .AlignRight()
                        .Column(column =>
                        {
                            column.Item()
                                .AlignRight()
                                .Text(
                                    "Código")
                                .FontSize(8)
                                .FontColor(
                                    Colors.Grey.Medium);


                            column.Item()
                                .AlignRight()
                                .Text(
                                    emergencia.CodigoEmergencia)
                                .FontSize(15)
                                .Bold()
                                .FontColor(
                                    "#B42318");


                            column.Item()
                                .PaddingTop(5)
                                .AlignRight()
                                .Text(
                                    emergencia.Estado)
                                .FontSize(9)
                                .SemiBold()
                                .FontColor(
                                    "#9A6700");
                        });
                });
        }


        // =========================================================
        // PACIENTE
        // =========================================================

        private void CrearPaciente(
            IContainer container,
            ProcesoEmergencia proceso)
        {
            Paciente paciente =
                proceso.Paciente;


            CrearTarjeta(
                container,
                "Datos del paciente",
                contenido =>
                {
                    contenido.Column(column =>
                    {
                        column.Spacing(10);


                        column.Item()
                            .Row(row =>
                            {
                                Campo(
                                    row.RelativeItem(2),
                                    "Nombre completo",
                                    paciente.NombreCompleto);


                                Campo(
                                    row.RelativeItem(),
                                    "Documento",
                                    paciente.DocumentoMostrar);


                                Campo(
                                    row.RelativeItem(),
                                    "Edad",
                                    paciente.Edad);


                                Campo(
                                    row.RelativeItem(),
                                    "Sexo",
                                    Texto(paciente.Sexo));
                            });


                        column.Item()
                            .Row(row =>
                            {
                                Campo(
                                    row.RelativeItem(),
                                    "Teléfono",
                                    Texto(paciente.Telefono));


                                Campo(
                                    row.RelativeItem(),
                                    "Seguro",
                                    string.IsNullOrWhiteSpace(
                                        paciente.NombreSeguro)
                                        ? "Sin seguro"
                                        : paciente.NombreSeguro);


                                Campo(
                                    row.RelativeItem(2),
                                    "Dirección",
                                    Texto(paciente.Direccion));
                            });
                    });
                });
        }


        // =========================================================
        // EVALUACIÓN
        // =========================================================

        private void CrearEvaluacion(
            IContainer container,
            ProcesoEmergencia proceso)
        {
            EvaluacionInicial e =
                proceso.Evaluacion;


            CrearTarjeta(
                container,
                "Evaluación inicial",
                contenido =>
                {
                    contenido.Column(column =>
                    {
                        column.Spacing(10);


                        column.Item()
                            .Row(row =>
                            {
                                Campo(
                                    row.RelativeItem(),
                                    "Triage",
                                    ObtenerTriage(
                                        e.NivelTriage));


                                Campo(
                                    row.RelativeItem(),
                                    "Temperatura",
                                    e.Temperatura.HasValue
                                        ? $"{e.Temperatura.Value:N1} °C"
                                        : "—");


                                Campo(
                                    row.RelativeItem(),
                                    "Presión arterial",
                                    Texto(
                                        e.PresionArterial));


                                Campo(
                                    row.RelativeItem(),
                                    "Frecuencia cardíaca",
                                    e.FrecuenciaCardiaca.HasValue
                                        ? $"{e.FrecuenciaCardiaca.Value} lpm"
                                        : "—");
                            });


                        column.Item()
                            .Row(row =>
                            {
                                Campo(
                                    row.RelativeItem(),
                                    "Frecuencia respiratoria",
                                    e.FrecuenciaRespiratoria.HasValue
                                        ? $"{e.FrecuenciaRespiratoria.Value} rpm"
                                        : "—");


                                Campo(
                                    row.RelativeItem(),
                                    "Saturación O₂",
                                    e.Saturacion.HasValue
                                        ? $"{e.Saturacion.Value}%"
                                        : "—");


                                Campo(
                                    row.RelativeItem(),
                                    "Glucemia",
                                    e.Glucemia.HasValue
                                        ? $"{e.Glucemia.Value:N1} mg/dL"
                                        : "—");


                                Campo(
                                    row.RelativeItem(),
                                    "Peso / Talla",
                                    $"{ValorDecimal(e.Peso, "kg")} / {ValorDecimal(e.Talla, "cm")}");
                            });
                    });
                });
        }


        // =========================================================
        // INFORMACIÓN CLÍNICA
        // =========================================================

        private void CrearInformacionClinica(
            IContainer container,
            ProcesoEmergencia proceso)
        {
            InformacionClinica info =
                proceso.InformacionClinica;


            CrearTarjeta(
                container,
                "Información clínica",
                contenido =>
                {
                    contenido.Column(column =>
                    {
                        column.Spacing(9);


                        column.Item()
                            .Element(c =>
                                CampoVertical(
                                    c,
                                    "Motivo de consulta",
                                    Texto(
                                        info.MotivoConsulta)));


                        column.Item()
                            .Row(row =>
                            {
                                Campo(
                                    row.RelativeItem(),
                                    "Antecedentes",
                                    ConstruirAntecedentes(
                                        info));


                                Campo(
                                    row.RelativeItem(),
                                    "Alergias conocidas",
                                    Texto(
                                        info.Alergias));
                            });


                        column.Item()
                            .Row(row =>
                            {
                                Campo(
                                    row.RelativeItem(),
                                    "Medicamentos habituales",
                                    Texto(
                                        info.MedicamentosActuales));


                                Campo(
                                    row.RelativeItem(),
                                    "Observaciones clínicas",
                                    Texto(
                                        info.Observaciones));
                            });
                    });
                });
        }


        // =========================================================
        // DIAGNÓSTICOS
        // =========================================================

        private void CrearDiagnosticos(
            IContainer container,
            ProcesoEmergencia proceso)
        {
            DiagnosticoEmergencia diagnostico =
                proceso.Diagnostico;


            CrearTarjeta(
                container,
                "Diagnóstico",
                contenido =>
                {
                    contenido.Column(column =>
                    {
                        column.Spacing(9);


                        string principal =
                            proceso.DiagnosticoPrincipalCIE10
                            != null
                                ? proceso
                                    .DiagnosticoPrincipalCIE10
                                    .Mostrar
                                : Texto(
                                    diagnostico
                                    .DiagnosticoPrincipal);


                        column.Item()
                            .Background(
                                "#F4F7FB")
                            .Border(1)
                            .BorderColor(
                                "#D9E2EC")
                            .Padding(10)
                            .Column(c =>
                            {
                                c.Item()
                                    .Text(
                                        "Diagnóstico principal CIE-10")
                                    .FontSize(8)
                                    .FontColor(
                                        Colors.Grey.Medium);


                                c.Item()
                                    .PaddingTop(3)
                                    .Text(
                                        principal)
                                    .FontSize(10)
                                    .SemiBold()
                                    .FontColor(
                                        "#2457A6");
                            });


                        List<string> secundarios =
                            proceso
                            .DiagnosticosSeleccionados?
                            .Where(x =>
                                proceso
                                    .DiagnosticoPrincipalCIE10
                                    == null
                                ||
                                x.IdCIE10 !=
                                proceso
                                    .DiagnosticoPrincipalCIE10
                                    .IdCIE10)
                            .Select(x =>
                                x.Mostrar)
                            .ToList()
                            ?? new List<string>();


                        column.Item()
                            .Row(row =>
                            {
                                Campo(
                                    row.RelativeItem(),
                                    "Diagnósticos secundarios",
                                    secundarios.Count > 0
                                        ? string.Join(
                                            "\n",
                                            secundarios.Select(
                                                x => "• " + x))
                                        : "Sin diagnósticos secundarios.");


                                Campo(
                                    row.RelativeItem(),
                                    "Diagnósticos manuales",
                                    proceso.DiagnosticosManuales != null
                                    &&
                                    proceso.DiagnosticosManuales.Count > 0
                                        ? string.Join(
                                            "\n",
                                            proceso
                                                .DiagnosticosManuales
                                                .Where(x =>
                                                    !string.IsNullOrWhiteSpace(x))
                                                .Select(x =>
                                                    "• " + x))
                                        : "Ninguno.");
                            });


                        column.Item()
                            .Row(row =>
                            {
                                Campo(
                                    row.RelativeItem(),
                                    "Impresión clínica",
                                    Texto(
                                        diagnostico.ImpresionClinica));


                                Campo(
                                    row.RelativeItem(),
                                    "Observaciones médicas",
                                    Texto(
                                        diagnostico.Observaciones));
                            });
                    });
                });
        }


        // =========================================================
        // ITEMS CLÍNICOS
        // =========================================================

        private void CrearItemsClinicos(
            IContainer container,
            ProcesoEmergencia proceso)
        {
            CrearTarjeta(
                container,
                "Tratamiento y servicios",
                contenido =>
                {
                    contenido.Column(column =>
                    {
                        List<EmergenciaItem> items =
                            proceso.ItemsClinicos
                            ?? new List<EmergenciaItem>();


                        string plan =
                            items
                            .Where(x =>
                                !string.IsNullOrWhiteSpace(
                                    x.NombrePlanTarifario))
                            .Select(x =>
                                x.NombrePlanTarifario)
                            .FirstOrDefault()
                            ?? "No determinado";


                        column.Item()
                            .AlignRight()
                            .Text(
                                $"Plan tarifario: {plan}")
                            .FontSize(8)
                            .FontColor(
                                Colors.Grey.Medium);


                        column.Item()
                            .PaddingTop(8)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(
                                    columns =>
                                    {
                                        columns.ConstantColumn(60);

                                        columns.ConstantColumn(85);

                                        columns.RelativeColumn();

                                        columns.ConstantColumn(55);

                                        columns.ConstantColumn(75);

                                        columns.ConstantColumn(75);
                                    });


                                table.Header(header =>
                                {
                                    HeaderCelda(
                                        header.Cell(),
                                        "Código");

                                    HeaderCelda(
                                        header.Cell(),
                                        "Tipo");

                                    HeaderCelda(
                                        header.Cell(),
                                        "Servicio / artículo");

                                    HeaderCelda(
                                        header.Cell(),
                                        "Cant.");

                                    HeaderCelda(
                                        header.Cell(),
                                        "Precio");

                                    HeaderCelda(
                                        header.Cell(),
                                        "Total");
                                });


                                foreach (
                                    EmergenciaItem item
                                    in items)
                                {
                                    Celda(
                                        table.Cell(),
                                        item.Codigo);

                                    Celda(
                                        table.Cell(),
                                        item.TipoItem);

                                    Celda(
                                        table.Cell(),
                                        item.Nombre);

                                    Celda(
                                        table.Cell(),
                                        item.Cantidad
                                            .ToString("N2"));

                                    Celda(
                                        table.Cell(),
                                        $"RD$ {item.PrecioUnitarioAplicado:N2}");

                                    Celda(
                                        table.Cell(),
                                        $"RD$ {(item.Cantidad * item.PrecioUnitarioAplicado):N2}");
                                }
                            });


                        decimal subtotal =
                            items.Sum(
                                x =>
                                    x.Cantidad
                                    *
                                    x.PrecioUnitarioAplicado);


                        column.Item()
                            .PaddingTop(10)
                            .AlignRight()
                            .Text(
                                $"Subtotal: RD$ {subtotal:N2}")
                            .FontSize(12)
                            .Bold()
                            .FontColor(
                                "#2457A6");
                    });
                });
        }


        // =========================================================
        // DESTINO
        // =========================================================

        private void CrearDestino(
            IContainer container,
            ProcesoEmergencia proceso)
        {
            DestinoEmergencia destino =
                proceso.Destino;


            CrearTarjeta(
                container,
                "Destino del paciente",
                contenido =>
                {
                    contenido.Row(row =>
                    {
                        Campo(
                            row.RelativeItem(),
                            "Destino",
                            Texto(
                                destino.Destino));


                        Campo(
                            row.RelativeItem(),
                            "Fecha de salida",
                            destino.FechaSalida.HasValue
                                ? destino.FechaSalida.Value
                                    .ToString(
                                        "dd/MM/yyyy HH:mm")
                                : "Pendiente");


                        Campo(
                            row.RelativeItem(2),
                            "Observaciones finales",
                            Texto(
                                destino
                                    .ObservacionesFinales));
                    });
                });
        }


        // =========================================================
        // MÉDICO
        // =========================================================

        private void CrearMedico(
            IContainer container,
            Emergencia emergencia)
        {
            CrearTarjeta(
                container,
                "Responsable de la atención",
                contenido =>
                {
                    contenido.Row(row =>
                    {
                        Campo(
                            row.RelativeItem(),
                            "Médico",
                            Texto(
                                emergencia.NombreMedico));


                        Campo(
                            row.RelativeItem(),
                            "Especialidad",
                            Texto(
                                emergencia.Especialidad));


                        Campo(
                            row.RelativeItem(),
                            "Fecha de ingreso",
                            emergencia.FechaIngreso
                                .ToString(
                                    "dd/MM/yyyy HH:mm"));
                    });
                });
        }


        // =========================================================
        // TARJETA
        // =========================================================

        private void CrearTarjeta(
            IContainer container,
            string titulo,
            Action<IContainer> contenido)
        {
            container
                .Border(1)
                .BorderColor(
                    "#D9E2EC")
                .Padding(12)
                .Column(column =>
                {
                    column.Item()
                        .Text(
                            titulo)
                        .FontSize(12)
                        .SemiBold()
                        .FontColor(
                            "#1E3A5F");


                    column.Item()
                        .PaddingTop(8)
                        .Element(
                            contenido);
                });
        }


        // =========================================================
        // CAMPO HORIZONTAL
        // =========================================================

        private void Campo(
            IContainer container,
            string etiqueta,
            string valor)
        {
            container
                .PaddingRight(10)
                .Column(column =>
                {
                    column.Item()
                        .Text(
                            etiqueta)
                        .FontSize(8)
                        .FontColor(
                            Colors.Grey.Medium);


                    column.Item()
                        .PaddingTop(2)
                        .Text(
                            valor)
                        .FontSize(9)
                        .SemiBold();
                });
        }


        private void CampoVertical(
            IContainer container,
            string etiqueta,
            string valor)
        {
            Campo(
                container,
                etiqueta,
                valor);
        }


        // =========================================================
        // TABLA
        // =========================================================

        private void HeaderCelda(
            IContainer container,
            string texto)
        {
            container
                .Background(
                    "#EEF2F6")
                .BorderBottom(1)
                .BorderColor(
                    "#D9E2EC")
                .Padding(5)
                .Text(
                    texto)
                .FontSize(8)
                .SemiBold();
        }


        private void Celda(
            IContainer container,
            string texto)
        {
            container
                .BorderBottom(1)
                .BorderColor(
                    "#EDF1F5")
                .Padding(5)
                .Text(
                    texto)
                .FontSize(8);
        }


        // =========================================================
        // UTILIDADES
        // =========================================================

        private string Texto(
            string? valor)
        {
            return string.IsNullOrWhiteSpace(
                valor)
                ? "No especificado"
                : valor;
        }


        private string ValorDecimal(
            decimal? valor,
            string unidad)
        {
            return valor.HasValue
                ? $"{valor.Value:N1} {unidad}"
                : "—";
        }


        private string ObtenerTriage(
            int nivel)
        {
            return nivel switch
            {
                1 => "Nivel I - Reanimación",
                2 => "Nivel II - Emergencia",
                3 => "Nivel III - Urgencia",
                4 => "Nivel IV - Menor urgencia",
                5 => "Nivel V - No urgente",
                _ => "No registrado"
            };
        }


        private string ConstruirAntecedentes(
            InformacionClinica informacion)
        {
            if (informacion.Ninguno)
            {
                return
                    "Sin antecedentes conocidos";
            }


            List<string> antecedentes =
                new List<string>();


            if (informacion.Diabetes)
                antecedentes.Add(
                    "Diabetes");


            if (informacion.Hipertension)
                antecedentes.Add(
                    "Hipertensión");


            if (informacion.Asma)
                antecedentes.Add(
                    "Asma");


            if (informacion.Cardiopatia)
                antecedentes.Add(
                    "Cardiopatía");


            if (informacion.Embarazo)
                antecedentes.Add(
                    "Embarazo");


            return antecedentes.Count > 0
                ? string.Join(
                    ", ",
                    antecedentes)
                : "No especificados";
        }
    }
}