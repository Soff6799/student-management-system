using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StudentAccounting.Infrastructure.Data
{
    /// <summary>
    /// Фабрика для создания AppDbContext во время разработки (для миграций EF Core)
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Строка подключения для миграций (должна совпадать с той, что в WebApi)
            optionsBuilder.UseNpgsql(
                "Host=localhost;Database=student_accounting;Username=postgres;Password=postgres123"
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}