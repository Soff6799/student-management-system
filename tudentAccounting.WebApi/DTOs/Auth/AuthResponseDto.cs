namespace tudentAccounting.WebApi.DTOs.Auth
{
    /// <summary>
    /// Объект передачи данных (DTO) с результатами успешной аутентификации или регистрации
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Токен доступа (например, JWT) для авторизации последующих запросов
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Логин аутентифицированного пользователя
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// Полное имя (ФИО) пользователя
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Роль пользователя в системе, определяющая его права доступа
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
