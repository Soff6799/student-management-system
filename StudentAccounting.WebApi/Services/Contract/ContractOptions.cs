namespace StudentAccounting.WebApi.Services.Contract;

/// <summary>
/// Настройки хранилища файлов договоров
/// </summary>
public class StorageOptions
{
    public const string SectionName = "StorageSettings";

    /// <summary>
    /// Абсолютный путь к папке с файлами договоров
    /// </summary>
    public string ContractsRoot { get; set; } = "storage/contracts";
}

/// <summary>
/// Реквизиты исполнителя (организации, оказывающей услуги обучения)
/// </summary>
public class ExecutorOptions
{
    public const string SectionName = "ExecutorSettings";

    public string FullName { get; set; } = "ООО «Учебный центр»";
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}