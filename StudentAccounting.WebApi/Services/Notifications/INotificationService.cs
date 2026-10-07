using StudentAccounting.WebApi.DTOs.Notifications;

namespace StudentAccounting.WebApi.Services.Notifications;

/// <summary>
/// Интерфейс сервиса для работы с уведомлениями системы
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// sortBy: nextDate | employee | organization | program
    /// </summary>
    Task<IEnumerable<NotificationDto>> GetAsync(string sortBy = "nextDate", bool descending = false);
}