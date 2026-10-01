using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.Domain
{
    /// <summary>
    /// Запись о прохождении обучения конкретным сотрудником
    /// </summary>
    public class StudentTraining: BaseEntity
    {
        /// <summary>
        /// Идентификатор обучающегося студента
        /// </summary>
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// К какому сотруднику относится
        /// </summary>
        public Employee Employee { get; set; } = null!;

        /// <summary>
        /// Идентификатор Группы обучения
        /// </summary>
        public Guid TrainingGroupId { get; set; }

        /// <summary>
        /// К какой группе обучения относится
        /// </summary>
        public TrainingGroup TrainingGroup { get; set; } = null!;

        /// <summary>
        /// Программа обучения => Наследуется от группы
        /// </summary>
        public TrainingProgram? TrainingProgram => TrainingGroup?.TrainingProgram;

        /// <summary>
        /// Дата начала обучения => Наследуется от группы
        /// </summary>
        public DateOnly? StartDate => TrainingGroup?.StartDate;

        /// <summary>
        /// Дата окончания обучения => Наследуется от группы
        /// </summary>
        public DateOnly? EndDate => TrainingGroup?.EndDate;

        /// <summary>
        /// Статус сотрудника
        /// </summary>
        public TrainingStatus Status { get; set; }

        /// <summary>
        /// Дата прохождения. Заполняется при статусе - Прошел обучение
        /// </summary>
        public DateOnly? CompletionDate { get; set; }

        /// <summary>
        /// Дата следующего обучения. Рассчитывается автоматически
        /// </summary>
        public DateOnly? NextTrainingDate { get; set; }

        /// <summary>
        /// Номер удостоверения / сертификата. Необязательно
        /// </summary>
        public string? CertificateNumber { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string? Note { get; set; }




    }
}
