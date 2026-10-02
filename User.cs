using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.Domain;

/// <summary>
/// Пользователь системы (Администратор/Методист)
/// </summary>
public class User: BaseEntity
{
    /// <summary>
    /// Фамилия
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Имя
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Отчество
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Логин для входа в систему
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Хеш пароля
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Роль пользователя
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Статус активности (активен/заблокирован)
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Дата и время последнего входа
    /// </summary>
    public DateTime? LastLoginDate { get; set; }
}