using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.Domain
{
    /// <summary>
    /// Сущность группы обучения
    /// </summary>
    public class TrainingGroup: BaseEntity
    {
        /// <summary>
        /// Название группы
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор программы обучения (внешний ключ)
        /// </summary>
        public Guid TrainingProgramId { get; set; }

        /// <summary>
        /// Программа обучения, по которой занимается группа
        /// </summary>
        public TrainingProgram TrainingProgram { get; set; } = null!;

        /// <summary>
        /// Дата начала обучения
        /// </summary>
        public DateOnly StartDate { get; set; }

        /// <summary>
        /// Дата окончания обучения
        /// </summary>
        public DateOnly? EndDate { get; set; }

        /// <summary>
        /// Статус группы
        /// </summary>
        public GroupStatus Status { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// Коллекция записей о прохождении сотрудником программы
        /// </summary>
        public List<StudentTraining> StudentTrainings { get; set; } = [];

        /// <summary>
        /// Коллекция договоров
        /// </summary>
        public List<Contract> Contracts { get; set; } = [];

    }
}
