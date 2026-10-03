using System.Linq.Expressions;

namespace StudentAccounting.Dal.Contracts.interfaces
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Получить все сущности
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Получить сущность по Id
        /// </summary>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Найти сущности по условию
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Создать сущность
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Обновить сущность
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Удалить сущность
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// сохранить изменения в бд
        /// </summary>
        Task SaveChangesAsync();



    }
}
