using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.Notifications;

namespace StudentAccounting.WebApi.Services.Notifications;

/// <summary>
/// Интерфейс сервиса реестра уведомлений о повторном обучении
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Получить уведомления с поиском, фильтрами, сортировкой и пагинацией
    /// </summary>
    Task<PagedResultDto<NotificationDto>> GetPagedAsync(NotificationListParams p);

    /// <summary>
    /// Получить все отфильтрованные уведомления без пагинации (для XLSX)
    /// </summary>
    Task<List<NotificationDto>> GetFilteredAsync(NotificationListParams p);
}