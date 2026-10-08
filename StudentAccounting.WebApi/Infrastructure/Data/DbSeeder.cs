using Microsoft.EntityFrameworkCore;
using StudentAccounting.Domain;
using StudentAccounting.Infrastructure.Data;
using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.Infrastructure.Data;

/// <summary>
/// Засев тестовых пользователей при старте приложения.
/// Существующим пользователям с неизвестным паролем пароль сбрасывается на тестовый.
/// </summary>
public static class DbSeeder
{
    /// <summary>
    /// Гарантирует наличие пользователей admin и methodist с известными паролями
    /// </summary>
    public static async Task SeedAsync(AppDbContext context)
    {
        await EnsureUserAsync(context, "adminka", "admin1234", "Админов", "Админ", "Админович", UserRole.Administrator);
        await EnsureUserAsync(context, "methodist", "methodist123", "Методистов", "Методист", "Методистович", UserRole.Methodologist);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Создаёт пользователя, если его нет. Если пользователь существует,
    /// но пароль не совпадает с тестовым — сбрасывает пароль (только для dev-окружения).
    /// </summary>
    private static async Task EnsureUserAsync(
        AppDbContext context,
        string login,
        string password,
        string lastName,
        string firstName,
        string middleName,
        UserRole role)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Login == login);

        if (user == null)
        {
            context.Users.Add(new User
            {
                Login = login,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                LastName = lastName,
                FirstName = firstName,
                MiddleName = middleName,
                Role = role,
                IsActive = true
            });
            return;
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        user.IsActive = true;
    }
}