using System;
using System.Collections.Generic;
using System.Text;

namespace StudentAccounting.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseEntity
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Дата и время создания
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Кем создан
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время обновления
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

        /// <summary>
        /// Кем обновлён
        /// </summary>
        public string UpdatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Дата и время удаления
        /// </summary>
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
