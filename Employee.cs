using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.Domain
{
    /// <summary>
    /// сущность сотрудника
    /// </summary>
    public class Employee
    {
        /// <summary>
        /// фамилия
        /// </summary>
        public string LastName {  get; set; } = string.Empty;

        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Отчество
        /// </summary>
        public string? Patronymic { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public DateOnly? BirthDate { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        public string? Phone {  get; set; }

        /// <summary>
        /// Почта
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Организация Id
        /// </summary>
        public Guid? OrganizationId { get; set; }
    
        /// <summary>
        /// Привязка к организации
        /// </summary>
        public Organization? Organization { get; set; }

        /// <summary>
        /// Должность
        /// </summary>
        public string? Post { get; set; }

        /// <summary>
        /// Статус
        /// </summary>
        public EmployeeStatus Status { get; set; }

        /// <summary>
        /// Дата увольнения
        /// </summary>
        public DateOnly? DismissalDate { get; set; }

    }
}
