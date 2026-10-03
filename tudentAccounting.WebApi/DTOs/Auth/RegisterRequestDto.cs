using StudentAccounting.Domain;

namespace StudentAccounting.WebApi.DTOs.Auth
{
    /// <summary>
    /// Объект передачи данных (DTO) для запроса на регистрацию нового пользователя
    /// </summary>
    public class RegisterRequestDto
    {
        /// <summary>
        /// Логин или имя пользователя для создания учетной записи
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Пароль для новой учетной записи
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Отчество пользователя 
        /// </summary>
        public string? MiddleName { get; set; }

        /// <summary>
        /// Роль пользователя в системе (по умолчанию: Методист)
        /// </summary>
        public Enums.UserRole Role { get; set; } = Enums.UserRole.Methodologist;
    }
}
