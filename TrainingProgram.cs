using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.Domain
{
    /// <summary>
    /// Сущность программы обучения
    /// </summary>
    public class TrainingProgram:BaseEntity
    {
        /// <summary>
        /// Наименование программы
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Описание
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Стоимость
        /// </summary>
        public decimal CostRubles {  get; set; }

        /// <summary>
        /// Требования к образованию
        /// </summary>
        public RequirementsEducation? RequirementsEducation { get; set; }

        /// <summary>
        /// Переодичность повторного обучения
        /// </summary>
        public RetrainingPeriodicity? RetrainingPeriodicity { get; set; }

        /// <summary>
        /// Срок обучения (значение)
        /// </summary>
        public int? DurationValue { get; set; }

        /// <summary>
        /// Единица измерения срока обучения (часы или дни)
        /// </summary>
        public DurationUnit? DurationUnit { get; set; }

        /// <summary>
        /// Статус программы. Обязательно.
        /// </summary>
        public ProgramStatus Status { get; set; }

    }
}
