namespace StudentAccounting.Dal.Contracts.interfaces
{
    /// <summary>
    /// Сущность с первичным ключом
    /// </summary>
    public interface IEntityWithId
    {
        Guid Id { get; set; }
    }
}
