using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.Notifications;
using StudentAccounting.WebApi.Services.Excel;
using StudentAccounting.WebApi.Services.Notifications;

namespace StudentAccounting.WebApi.Controllers;

/// <summary>
/// Реестр уведомлений о необходимости повторного обучения
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _service;
    private readonly IExcelExportService _excel;

    public NotificationsController(INotificationService service, IExcelExportService excel)
    {
        _service = service;
        _excel = excel;
    }

    /// <summary>
    /// Получить уведомления с поиском, фильтрами, сортировкой и пагинацией
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<NotificationDto>>> Get([FromQuery] NotificationListParams p)
    {
        return Ok(await _service.GetPagedAsync(p));
    }

    /// <summary>
    /// Выгрузка реестра уведомлений в XLSX с учётом фильтров и поиска
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] NotificationListParams p)
    {
        var data = await _service.GetFilteredAsync(p);
        var file = _excel.ExportNotifications(data);
        return File(file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"notifications_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx");
    }
}