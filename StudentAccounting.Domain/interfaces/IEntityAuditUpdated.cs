namespace StudentAccounting.Domain
{
    /// <summary>
    /// Сущность с полями аудита "обновлено"
    /// </summary>
    public interface IEntityAuditUpdated
    {
        DateTimeOffset UpdatedAt { get; set; }
        User? UpdatedBy { get; set; }
        Guid? UpdatedById { get; set; }
    }
}