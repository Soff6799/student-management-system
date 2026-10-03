using Microsoft.EntityFrameworkCore;
using StudentAccounting.Domain;

namespace StudentAccounting.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<TrainingProgram> TrainingPrograms => Set<TrainingProgram>();
    public DbSet<TrainingGroup> TrainingGroups => Set<TrainingGroup>();
    public DbSet<StudentTraining> StudentTrainings => Set<StudentTraining>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Login).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.HasIndex(e => e.Login).IsUnique();
            entity.ConfigureBaseEntity();
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.Property(e => e.FullName).HasMaxLength(500).IsRequired();
            entity.Property(e => e.ShortName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.INN).HasMaxLength(12).IsRequired();
            entity.Property(e => e.KPP).HasMaxLength(9).IsRequired();
            entity.Property(e => e.OGRN).HasMaxLength(15).IsRequired();
            entity.Property(e => e.LegalAddress).HasMaxLength(500).IsRequired();
            entity.Property(e => e.ActualAddress).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.ContactPersonFullName).HasMaxLength(255);
            entity.Property(e => e.ContactPersonPosition).HasMaxLength(255);
            entity.Property(e => e.Note).HasMaxLength(2000);
            entity.HasIndex(e => e.INN).IsUnique();
            entity.HasMany(e => e.Employees).WithOne(emp => emp.Organization).HasForeignKey(emp => emp.OrganizationId).OnDelete(DeleteBehavior.SetNull);
            entity.ConfigureBaseEntity();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Patronymic).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Post).HasMaxLength(255);
            entity.HasOne(e => e.Organization).WithMany(org => org.Employees).HasForeignKey(e => e.OrganizationId).OnDelete(DeleteBehavior.SetNull);
            entity.HasMany(e => e.Educations).WithOne(ed => ed.Employee).HasForeignKey(ed => ed.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.StudentTrainings).WithOne(st => st.Employee).HasForeignKey(st => st.EmployeeId).OnDelete(DeleteBehavior.Restrict);
            entity.ConfigureBaseEntity();
        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.Property(e => e.InstitutionName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Specialty).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
            entity.HasOne(e => e.Employee).WithMany(emp => emp.Educations).HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            entity.ConfigureBaseEntity();
        });

        modelBuilder.Entity<TrainingProgram>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.CostRubles).HasPrecision(18, 2);
            entity.HasMany(e => e.TrainingGroups).WithOne(tg => tg.TrainingProgram).HasForeignKey(tg => tg.TrainingProgramId).OnDelete(DeleteBehavior.Restrict);
            entity.ConfigureBaseEntity();
        });

        modelBuilder.Entity<TrainingGroup>(entity =>
        {
            entity.Property(e => e.GroupName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Note).HasMaxLength(2000);
            entity.HasOne(e => e.TrainingProgram).WithMany(tp => tp.TrainingGroups).HasForeignKey(e => e.TrainingProgramId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.StudentTrainings).WithOne(st => st.TrainingGroup).HasForeignKey(st => st.TrainingGroupId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.Contracts).WithOne(c => c.TrainingGroup).HasForeignKey(c => c.TrainingGroupId).OnDelete(DeleteBehavior.Cascade);
            entity.ConfigureBaseEntity();
        });

        modelBuilder.Entity<StudentTraining>(entity =>
        {
            entity.Property(e => e.CertificateNumber).HasMaxLength(100);
            entity.Property(e => e.Note).HasMaxLength(2000);
            entity.HasOne(e => e.Employee).WithMany(emp => emp.StudentTrainings).HasForeignKey(e => e.EmployeeId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.TrainingGroup).WithMany(tg => tg.StudentTrainings).HasForeignKey(e => e.TrainingGroupId).OnDelete(DeleteBehavior.Cascade);
            entity.ConfigureBaseEntity();
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.Property(e => e.ContractNumber).HasMaxLength(50).IsRequired();
            entity.Property(e => e.FilePath).HasMaxLength(500).IsRequired();
            entity.HasIndex(e => e.ContractNumber).IsUnique();
            entity.HasOne(e => e.TrainingGroup).WithMany(tg => tg.Contracts).HasForeignKey(e => e.TrainingGroupId).OnDelete(DeleteBehavior.Cascade);
            entity.ConfigureBaseEntity();
        });
    }
}