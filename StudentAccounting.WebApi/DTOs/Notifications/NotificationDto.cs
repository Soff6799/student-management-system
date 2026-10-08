using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.Notifications;

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

    /// <summary>
    /// Цветовая индикация срочности (ТЗ 4.f.iv)
    /// </summary>
    public NotificationColor Color { get; set; }
}