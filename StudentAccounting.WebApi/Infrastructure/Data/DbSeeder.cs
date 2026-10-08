using Microsoft.EntityFrameworkCore;
using StudentAccounting.Domain;
using StudentAccounting.Infrastructure.Data;
using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync())
                return;

            context.Users.AddRange(
                new User
                {
                    Login = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    FirstName = "Админ",
                    LastName = "Админов",
                    MiddleName = "Админович",
                    Role = UserRole.Administrator,
                    IsActive = true
                },
                new User
                {
                    Login = "methodist",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("methodist123"),
                    FirstName = "Методист",
                    LastName = "Методистов",
                    MiddleName = "Методистович",
                    Role = UserRole.Methodologist,
                    IsActive = true
                }
        );
            await context.SaveChangesAsync();
        }
    }
}
