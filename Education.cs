using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.Domain
{
    /// <summary>
    /// Раздел образование карточки сотрудника
    /// </summary>
    public class Education: BaseEntity
    {
        /// <summary>
        /// Уровень образования
        /// </summary>
        public EducationLevel Level { get; set; }

        /// <summary>
        /// Наименование учебного заведения
        /// </summary>
        public string InstitutionName { get; set; } = string.Empty;

        /// <summary>
        /// Год окончания
        /// </summary>
        public int GraduationYear { get; set; }

        /// <summary>
        /// Специальность / направление подготовки
        /// </summary>
        public string Specialty { get; set; } = string.Empty;

        /// <summary>
        /// Путь к прикрепленному файлу документа (скан/фото)
        /// </summary>
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Внешний ключ
        /// </summary>
        public Guid EmployeeId { get; set; }

        /// <summary>
        /// Сотрудник, к которому относится данное образование
        /// </summary>
        public Employee? Employee {  get; set; }
    }
}
