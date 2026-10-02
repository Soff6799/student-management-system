namespace StudentAccounting.Domain
{
    /// <summary>
    /// Сущность с полями аудита "создано"
    /// </summary>
    public interface IEntityAuditCreated
    {
        DateTimeOffset CreatedAt { get; set; }
        User? CreatedBy { get; set; }
        Guid? CreatedById { get; set; }
    }
}