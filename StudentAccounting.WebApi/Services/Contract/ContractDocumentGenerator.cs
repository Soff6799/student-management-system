using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using Word = DocumentFormat.OpenXml.Wordprocessing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StudentAccounting.WebApi.DTOs.Contract;

namespace StudentAccounting.WebApi.Services.Contract;

/// <summary>
/// Генерация договора: DOCX через OpenXml, PDF через QuestPDF
/// </summary>
public class ContractDocumentGenerator : IContractDocumentGenerator
{
    private readonly ExecutorOptions _executor;

    public ContractDocumentGenerator(ExecutorOptions executor)
    {
        _executor = executor;
    }

    public byte[] GenerateDocx(ContractDocumentData data)
    {
        using var ms = new MemoryStream();
        using (var document = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document))
        {
            var mainPart = document.AddMainDocumentPart();
            mainPart.Document = new Word.Document();
            var body = new Word.Body();
            mainPart.Document.AppendChild(body);

            body.AppendChild(Text("ДОГОВОР НА ОБУЧЕНИЕ", bold: true, size: 32, centered: true));
            body.AppendChild(Text($"№ {data.ContractNumber}", bold: true, size: 28, centered: true));
            body.AppendChild(Text(FormatDate(data.GenerationDate), centered: true));
            body.AppendChild(Text(string.Empty));
            body.AppendChild(Text(Preamble(data)));

            body.AppendChild(Text("1. ПРЕДМЕТ ДОГОВОРА", bold: true));
            body.AppendChild(Text(
                $"1.1. Исполнитель обязуется организовать и провести обучение по программе «{data.ProgramName}» " +
                $"для группы «{data.GroupName}», а Заказчик — оплатить обучение."));
            if (!string.IsNullOrWhiteSpace(data.DurationText))
                body.AppendChild(Text($"1.2. Срок обучения: {data.DurationText}."));
            body.AppendChild(Text($"1.3. Перечень обучающихся ({data.StudentsCount} чел.):"));
            body.AppendChild(StudentsTable(data.Students));

            body.AppendChild(Text("2. СТОИМОСТЬ ОБУЧЕНИЯ И ПОРЯДОК РАСЧЁТОВ", bold: true));
            body.AppendChild(Text($"2.1. Стоимость обучения одного обучающегося: {data.CostPerStudent:F2} руб."));
            body.AppendChild(Text($"2.2. Общая стоимость по настоящему договору: {data.TotalCost:F2} руб."));

            body.AppendChild(Text("3. РЕКВИЗИТЫ ЗАКАЗЧИКА", bold: true));
            foreach (var line in CustomerRequisites(data))
                body.AppendChild(Text(line));

            body.AppendChild(Text("4. ПОДПИСИ СТОРОН", bold: true));
            body.AppendChild(Text($"Исполнитель: {_executor.FullName}, {_executor.Address}, тел. {_executor.Phone}"));
            body.AppendChild(Text("Исполнитель ______________          Заказчик ______________"));
        }
        return ms.ToArray();
    }

    public byte[] GeneratePdf(ContractDocumentData data)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(t => t.FontSize(11));
                page.Content().Column(col =>
                {
                    col.Spacing(6);
                    col.Item().Text($"ДОГОВОР НА ОБУЧЕНИЕ № {data.ContractNumber}").Bold().FontSize(16).AlignCenter();
                    col.Item().Text(FormatDate(data.GenerationDate)).AlignCenter();
                    col.Item().Text(Preamble(data));

                    col.Item().Text("1. ПРЕДМЕТ ДОГОВОРА").Bold();
                    col.Item().Text(
                        $"1.1. Исполнитель обязуется организовать и провести обучение по программе «{data.ProgramName}» " +
                        $"для группы «{data.GroupName}», а Заказчик — оплатить обучение.");
                    if (!string.IsNullOrWhiteSpace(data.DurationText))
                        col.Item().Text($"1.2. Срок обучения: {data.DurationText}.");
                    col.Item().Text($"1.3. Перечень обучающихся ({data.StudentsCount} чел.):");
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(25);
                            c.RelativeColumn();
                            c.RelativeColumn();
                        });
                        table.Header(h =>
                        {
                            h.Cell().Element(Cell).Text("№");
                            h.Cell().Element(Cell).Text("ФИО");
                            h.Cell().Element(Cell).Text("Должность");
                        });
                        foreach (var s in data.Students)
                        {
                            table.Cell().Element(Cell).Text(s.Number.ToString());
                            table.Cell().Element(Cell).Text(s.FullName);
                            table.Cell().Element(Cell).Text(string.IsNullOrWhiteSpace(s.Post) ? "—" : s.Post);
                        }
                    });

                    col.Item().Text("2. СТОИМОСТЬ ОБУЧЕНИЯ И ПОРЯДОК РАСЧЁТОВ").Bold();
                    col.Item().Text($"2.1. Стоимость обучения одного обучающегося: {data.CostPerStudent:F2} руб.");
                    col.Item().Text($"2.2. Общая стоимость по настоящему договору: {data.TotalCost:F2} руб.");

                    col.Item().Text("3. РЕКВИЗИТЫ ЗАКАЗЧИКА").Bold();
                    foreach (var line in CustomerRequisites(data))
                        col.Item().Text(line);

                    col.Item().Text("4. ПОДПИСИ СТОРОН").Bold();
                    col.Item().Text($"Исполнитель: {_executor.FullName}, {_executor.Address}, тел. {_executor.Phone}");
                    col.Item().Text("Исполнитель ______________          Заказчик ______________");
                });
            });
        });
        return document.GeneratePdf();
    }

    // ---------- общие блоки ----------

    private string Preamble(ContractDocumentData data)
    {
        return $"{_executor.FullName}, именуемый в дальнейшем «Исполнитель», " +
               $"и {data.CustomerName}, именуемый(ая) в дальнейшем «Заказчик», " +
               "заключили настоящий договор о нижеследующем:";
    }

    private static List<string> CustomerRequisites(ContractDocumentData data)
    {
        var lines = new List<string> { data.CustomerName };
        if (!data.IsIndividual)
        {
            lines.Add($"ИНН {data.CustomerInn}, КПП {data.CustomerKpp}, ОГРН {data.CustomerOgrn}");
            lines.Add($"Адрес: {data.CustomerAddress}");
        }
        if (!string.IsNullOrWhiteSpace(data.CustomerPhone)) lines.Add($"Тел.: {data.CustomerPhone}");
        if (!string.IsNullOrWhiteSpace(data.CustomerEmail)) lines.Add($"E-mail: {data.CustomerEmail}");
        return lines;
    }

    private static string FormatDate(DateOnly date)
    {
        return date.ToString("dd MMMM yyyy", System.Globalization.CultureInfo.GetCultureInfo("ru-RU")) + " г.";
    }

    private static IContainer Cell(IContainer container) => container.Border(1).Padding(3);

    private static Word.Paragraph Text(string value, bool bold = false, uint size = 24, bool centered = false)
    {
        var run = new Word.Run(new Word.Text(value) { Space = DocumentFormat.OpenXml.SpaceProcessingModeValues.Preserve });
        var props = new Word.RunProperties();
        if (bold) props.Append(new Word.Bold());
        props.Append(new Word.FontSize { Val = size.ToString() });
        run.RunProperties = props;

        var paragraph = new Word.Paragraph(run);
        if (centered)
            paragraph.ParagraphProperties = new Word.ParagraphProperties(
                new Word.Justification { Val = Word.JustificationValues.Center });
        return paragraph;
    }

    private static Word.Table StudentsTable(List<ContractStudentRow> students)
    {
        var table = new Word.Table();
        table.AppendChild(new TableProperties(new Word.TableBorders(
            new Word.TopBorder { Val = Word.BorderValues.Single, Size = 4 },
            new Word.BottomBorder { Val = Word.BorderValues.Single, Size = 4 },
            new Word.LeftBorder { Val = Word.BorderValues.Single, Size = 4 },
            new Word.RightBorder { Val = Word.BorderValues.Single, Size = 4 },
            new Word.InsideHorizontalBorder { Val = Word.BorderValues.Single, Size = 4 },
            new Word.InsideVerticalBorder { Val = Word.BorderValues.Single, Size = 4 })));

        table.AppendChild(Row(true, "№", "ФИО", "Должность"));
        foreach (var s in students)
            table.AppendChild(Row(false, s.Number.ToString(), s.FullName, string.IsNullOrWhiteSpace(s.Post) ? "—" : s.Post));
        return table;
    }

    private static TableRow Row(bool bold, params string[] cells)
    {
        var row = new TableRow();
        foreach (var cell in cells)
            row.AppendChild(new TableCell(Text(cell, bold)));
        return row;
    }
}