namespace StudentAccounting.WebApi.DTOs.Auth
{
    /// <summary>
    /// Объект передачи данных (DTO) для запроса на аутентификацию пользователя
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// Логин или имя пользователя для входа
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
