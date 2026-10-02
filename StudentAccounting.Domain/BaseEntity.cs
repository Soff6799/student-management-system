namespace StudentAccounting.Domain
<<<<<<< HEAD
=======

>>>>>>> 2-database-migrations
{
    /// <summary>
    /// Общие поля для сущностей
    /// </summary>
    public class BaseEntity : IBaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Идентификатор пользователя, который создал запись
        /// </summary>
        public Guid? CreatedById { get; set; }

        /// <summary>
        /// Кем создан
        /// </summary>
        public User? CreatedBy { get; set; }

        /// <summary>
        /// Дата и время обновления
        /// </summary>
<<<<<<< HEAD
        public DateTimeOffset? UpdatedAt { get; set; }
=======
        public DateTimeOffset UpdatedAt { get; set; }
>>>>>>> 2-database-migrations

        /// <summary>
        /// Идентификатор пользователя, который обновил запись
        /// </summary>
        public Guid? UpdatedById { get; set; }

        /// <summary>
        /// Кем обновлён
        /// </summary>
        public User? UpdatedBy { get; set; }

        /// <summary>
        /// Дата и время удаления
        /// </summary>
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
