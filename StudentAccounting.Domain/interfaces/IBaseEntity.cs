using StudentAccounting.Dal.Contracts.interfaces;

namespace StudentAccounting.Domain
{
    /// <summary>
    /// Базовая сущность со всеми стандартными полями:
    /// Id, аудит создания, аудит обновления, мягкое удаление
    /// </summary>
    public interface IBaseEntity :
        IEntityWithId,
        IEntityAuditCreated,
        IEntityAuditUpdated,
        IEntitySoftDelete
    {
    }
}