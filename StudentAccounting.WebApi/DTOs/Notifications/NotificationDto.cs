namespace StudentAccounting.WebApi.DTOs.Notifications;

/// <summary>
/// Цветовая индикация срочности
/// В ТЗ жёлтый указан как 60–90 дней, зелёный >120;
/// 90–120 отнесён к жёлтому
/// </summary>
public enum NotificationColor
{
    Green,  // более 120 дней
    Yellow, // от 60 до 120 дней
    Red     // менее 60 дней, включая просрочку
}

/// <summary>
/// Запись реестра уведомлений о повторном обучении
/// </summary>
public class NotificationDto
{
    public Guid StudentTrainingId { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeFullName { get; set; } = string.Empty;
    public string? OrganizationName { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public DateOnly? LastCompletionDate { get; set; }
    public DateOnly NextTrainingDate { get; set; }

    /// <summary>
    /// Дней до срока (отрицательное => просрочено)
    /// </summary>
    public int DaysRemaining { get; set; }
    public NotificationColor Color { get; set; }
}