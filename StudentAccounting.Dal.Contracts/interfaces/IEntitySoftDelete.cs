namespace StudentAccounting.Domain
{
    /// <summary>
    /// Сущность с мягким удалением
    /// </summary>
    public interface IEntitySoftDelete
    {
        DateTimeOffset? DeletedAt { get; set; }
    }
}