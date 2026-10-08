using ClosedXML.Excel;
using StudentAccounting.WebApi.DTOs.Employee;
using StudentAccounting.WebApi.DTOs.Notifications;
using StudentAccounting.WebApi.DTOs.Organization;
using StudentAccounting.WebApi.DTOs.StudentTraining;
using StudentAccounting.WebApi.DTOs.TrainingGroup;

namespace StudentAccounting.WebApi.Services.Excel;

/// <summary>
/// Выгрузка данных в XLSX. Данные передаются уже отфильтрованными
/// </summary>
public interface IExcelExportService
{
    /// <summary>
    /// Выгрузка списка организаций
    /// </summary>
    byte[] ExportOrganizations(IEnumerable<OrganizationDto> data);

    /// <summary>
    /// Выгрузка списка сотрудников
    /// </summary>
    byte[] ExportEmployees(IEnumerable<EmployeeDto> data);

    /// <summary>
    /// Выгрузка списка групп обучения
    /// </summary>
    byte[] ExportTrainingGroups(IEnumerable<TrainingGroupDto> data);

    /// <summary>
    /// Выгрузка списка обучающихся в группе
    /// </summary>
    byte[] ExportGroupStudents(IEnumerable<StudentTrainingDto> data);

    /// <summary>
    /// Выгрузка реестра уведомлений о повторном обучении
    /// </summary>
    byte[] ExportNotifications(IEnumerable<NotificationDto> data);
}

/// <summary>
/// Реализация выгрузки в XLSX на базе ClosedXML
/// </summary>
public class ExcelExportService : IExcelExportService
{
    private const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    public byte[] ExportOrganizations(IEnumerable<OrganizationDto> data)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Организации");
        WriteHeaders(ws, ["Полное наименование", "Краткое наименование", "ИНН", "КПП", "ОГРН", "Телефон", "Email"]);

        var row = 2;
        foreach (var o in data)
        {
            ws.Cell(row, 1).Value = o.FullName;
            ws.Cell(row, 2).Value = o.ShortName;
            ws.Cell(row, 3).Value = o.INN;
            ws.Cell(row, 4).Value = o.KPP;
            ws.Cell(row, 5).Value = o.OGRN;
            ws.Cell(row, 6).Value = o.Phone ?? string.Empty;
            ws.Cell(row, 7).Value = o.Email ?? string.Empty;
            row++;
        }
        ws.Columns().AdjustToContents();
        return Save(workbook);
    }

    public byte[] ExportEmployees(IEnumerable<EmployeeDto> data)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Сотрудники");
        WriteHeaders(ws, ["Фамилия", "Имя", "Отчество", "Организация", "Должность", "Телефон", "Email"]);
        var row = 2;
        foreach (var e in data)
        {
            ws.Cell(row, 1).Value = e.LastName;
            ws.Cell(row, 2).Value = e.FirstName;
            ws.Cell(row, 3).Value = e.Patronymic ?? string.Empty;
            ws.Cell(row, 4).Value = e.OrganizationName ?? "Физическое лицо";
            ws.Cell(row, 5).Value = e.Post ?? string.Empty;
            ws.Cell(row, 6).Value = e.Phone ?? string.Empty;
            ws.Cell(row, 7).Value = e.Email ?? string.Empty;
            row++;
        }
        ws.Columns().AdjustToContents();
        return Save(workbook);
    }

    public byte[] ExportNotifications(IEnumerable<NotificationDto> data)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Уведомления");
        WriteHeaders(ws, ["ФИО", "Организация", "Программа", "Дата последнего обучения",
                       "Дата следующего обучения", "Дней осталось", "Цвет"]);
        var row = 2;
        foreach (var n in data)
        {
            ws.Cell(row, 1).Value = n.EmployeeFullName ?? string.Empty;
            ws.Cell(row, 2).Value = n.OrganizationName ?? string.Empty;
            ws.Cell(row, 3).Value = n.ProgramName ?? string.Empty;
            ws.Cell(row, 4).Value = n.LastCompletionDate?.ToString("dd.MM.yyyy") ?? string.Empty;
            ws.Cell(row, 5).Value = n.NextTrainingDate.ToString("dd.MM.yyyy");
            ws.Cell(row, 6).Value = n.DaysRemaining;
            ws.Cell(row, 7).Value = GetDescription(n.Color);
            row++;
        }
        ws.Columns().AdjustToContents();
        return Save(workbook);
    }

    public byte[] ExportTrainingGroups(IEnumerable<TrainingGroupDto> data)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Группы обучения");
        WriteHeaders(ws, ["Название группы", "Программа обучения", "Дата начала", "Дата окончания", "Статус", "Кол-во обучающихся"]);
        var row = 2;
        foreach (var g in data)
        {
            ws.Cell(row, 1).Value = g.GroupName;
            ws.Cell(row, 2).Value = g.TrainingProgramName ?? string.Empty;
            ws.Cell(row, 3).Value = g.StartDate.ToString("dd.MM.yyyy");
            ws.Cell(row, 4).Value = g.EndDate?.ToString("dd.MM.yyyy") ?? string.Empty;
            ws.Cell(row, 5).Value = GetDescription(g.Status);
            ws.Cell(row, 6).Value = g.StudentsCount;
            row++;
        }
        ws.Columns().AdjustToContents();
        return Save(workbook);
    }

    public byte[] ExportGroupStudents(IEnumerable<StudentTrainingDto> data)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Обучающиеся группы");
        WriteHeaders(ws, ["ФИО", "Группа", "Статус", "Дата завершения", "Следующее обучение"]);
        var row = 2;
        foreach (var s in data)
        {
            ws.Cell(row, 1).Value = s.EmployeeFullName ?? string.Empty;
            ws.Cell(row, 2).Value = s.TrainingGroupName ?? string.Empty;
            ws.Cell(row, 3).Value = GetDescription(s.Status);
            ws.Cell(row, 4).Value = s.CompletionDate?.ToString("dd.MM.yyyy") ?? string.Empty;
            ws.Cell(row, 5).Value = s.NextTrainingDate?.ToString("dd.MM.yyyy") ?? string.Empty;
            row++;
        }
        ws.Columns().AdjustToContents();
        return Save(workbook);
    }

    private static void WriteHeaders(IXLWorksheet ws, string[] headers)
    {
        for (var i = 0; i < headers.Length; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
        }
    }

    private static byte[] Save(XLWorkbook workbook)
    {
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Возвращает русский текст из атрибута [Description] перечисления
    /// </summary>
    private static string GetDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field?
            .GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
            .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;
        return attr?.Description ?? value.ToString();
    }
}