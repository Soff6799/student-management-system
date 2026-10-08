using StudentAccounting.Domain;
using StudentAccounting.WebApi.Services.Auth;

namespace StudentAccounting.WebApi.Services.Common;

/// <summary>
/// Сервис для автоматического заполнения полей аудита
/// </summary>
public class AuditService
{
    private readonly ICurrentUserService _currentUserService;

    public AuditService(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public void SetAuditFields(IBaseEntity entity, bool isUpdate = false)
    {
        var userId = _currentUserService.UserId;
        var now = DateTimeOffset.UtcNow;

        if (!isUpdate)
        {
            entity.CreatedAt = now;
            entity.CreatedById = userId;
        }

        entity.UpdatedAt = now;
        entity.UpdatedById = userId;
    }
}