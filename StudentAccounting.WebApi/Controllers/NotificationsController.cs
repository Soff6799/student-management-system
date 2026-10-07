using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Notifications;
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

    public NotificationsController(INotificationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить список уведомлений с сортировкой
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> Get(
        [FromQuery] string sortBy = "nextDate",
        [FromQuery] bool descending = false)
    {
        return Ok(await _service.GetAsync(sortBy, descending));
    }
}