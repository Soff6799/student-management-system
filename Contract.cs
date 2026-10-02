namespace StudentAccounting.Domain
{
    /// <summary>
    /// Договор на обучение
    /// </summary>
    public class Contract: BaseEntity
    {
        /// <summary>
        /// Номер договора (генерируется автоматически по шаблону Д-YYYY-NNNN)
        /// </summary>
        public string ContractNumber { get; set; } = string.Empty;

        /// <summary>
        /// Дата формирования договора
        /// </summary>
        public DateOnly GenerationDate { get; set; }

        /// <summary>
        /// Путь к файлу сформированного договора (.docx или .pdf) в хранилище.
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор группы обучения, для которой сформирован договор (внешний ключ).
        /// </summary>
        public Guid TrainingGroupId { get; set; }

        /// <summary>
        /// Группа обучения, к которой привязан договор.
        /// </summary>
        public TrainingGroup TrainingGroup { get; set; } = null!;

    }
}
