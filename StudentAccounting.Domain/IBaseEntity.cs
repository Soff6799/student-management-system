namespace StudentAccounting.Domain;

/// <summary>
/// Базовая сущность со всеми стандартными полями:
/// Id, аудит создания, аудит обновления, мягкое удаление
/// </summary>
public interface IBaseEntity
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    Guid Id { get; set; }

    /// <summary>
    /// Дата и время создания
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Идентификатор пользователя, который создал запись
    /// </summary>
    Guid? CreatedById { get; set; }

    /// <summary>
    /// Дата и время обновления
    /// </summary>
    DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Идентификатор пользователя, который обновил запись
    /// </summary>
    Guid? UpdatedById { get; set; }

    /// <summary>
    /// Дата и время удаления
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }
}