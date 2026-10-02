namespace StudentAccounting.Domain
{
    /// <summary>
    /// Карточка организации
    /// </summary>
    public class Organization : BaseEntity
    {
        /// <summary>
        /// Полное имя организации
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Короткое имя организации
        /// </summary>
        public string ShortName {  get; set; } = string.Empty;

        /// <summary>
        /// ИНН
        /// </summary>
        public string INN { get; set; } = string.Empty;

        /// <summary>
        /// КПП
        /// </summary>
        public string KPP { get; set; } = string.Empty;

        /// <summary>
        /// ОГРН
        /// </summary>
        public string OGRN { get; set; } = string.Empty;

        /// <summary>
        /// Юридический адрес
        /// </summary>
        public string LegalAddress { get; set; } = string.Empty;
        
        /// <summary>
        /// Фактический адрес
        /// </summary>
        public string? ActualAddress { get; set; }
        
        /// <summary>
        /// Телефон
        /// </summary>
        public string? Phone { get; set; }
        
        /// <summary>
        /// Почта
        /// </summary>
        public string? Email { get; set; }
        
        /// <summary>
        /// Имя контактного лица
        /// </summary>
        public string? ContactPersonFullName { get; set; }
        
        /// <summary>
        /// Должность контактного лица
        /// </summary>
        public string? ContactPersonPosition { get; set; }
        
        /// <summary>
        /// Примечание
        /// </summary>
        public string? Note { get; set; }

        /// <summary>
        /// коллекция сотрудников
        /// </summary>
        public List<Employee> Employees { get; set; } = [];
    }
}
