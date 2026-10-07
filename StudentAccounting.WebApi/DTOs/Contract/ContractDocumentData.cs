namespace StudentAccounting.WebApi.DTOs.Contract;

/// <summary>
/// Данные для формирования тела договора (ТЗ 4.g.iii)
/// </summary>
public class ContractDocumentData
{
    public string ContractNumber { get; set; } = string.Empty;
    public DateOnly GenerationDate { get; set; }

    /// <summary>
    /// Заказчик: наименование организации или ФИО физического лица
    /// </summary>
    public string CustomerName { get; set; } = string.Empty;
    public bool IsIndividual { get; set; }
    public string? CustomerInn { get; set; }
    public string? CustomerKpp { get; set; }
    public string? CustomerOgrn { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerEmail { get; set; }

    public string GroupName { get; set; } = string.Empty;
    public string ProgramName { get; set; } = string.Empty;
    public string? DurationText { get; set; }
    public decimal CostPerStudent { get; set; }

    public int StudentsCount => Students.Count;
    public decimal TotalCost => CostPerStudent * Students.Count;

    public List<ContractStudentRow> Students { get; set; } = [];
}

/// <summary>
/// Строка перечня обучающихся в договоре
/// </summary>
public class ContractStudentRow
{
    public int Number { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Post { get; set; }
}