using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudentAccounting.Domain;

namespace StudentAccounting.Infrastructure.Data;

/// <summary>
/// Методы расширения для EntityTypeBuilder
/// </summary>
public static class EntityTypeBuilderExtensions
{
    /// <summary>
    /// Настраивает первичный ключ Id
    /// </summary>
    public static EntityTypeBuilder<T> HasIdAsKey<T>(this EntityTypeBuilder<T> builder)
        where T : BaseEntity
    {
        builder.HasKey(e => e.Id);
        return builder;
    }

    /// <summary>
    /// Настраивает связи аудита (CreatedBy + UpdatedBy) с NoAction
    /// </summary>
    public static EntityTypeBuilder<T> ConfigureAuditRelations<T>(this EntityTypeBuilder<T> builder)
        where T : BaseEntity
    {
        builder.HasOne(e => e.CreatedBy)
               .WithMany()
               .HasForeignKey(e => e.CreatedById)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(e => e.UpdatedBy)
               .WithMany()
               .HasForeignKey(e => e.UpdatedById)
               .OnDelete(DeleteBehavior.NoAction);

        return builder;
    }

    /// <summary>
    /// soft-delete фильтр по DeletedAt
    /// </summary>
    public static EntityTypeBuilder<T> ConfigureSoftDelete<T>(this EntityTypeBuilder<T> builder)
        where T : BaseEntity
    {
        builder.HasQueryFilter(e => e.DeletedAt == null);
        return builder;
    }

    /// <summary>
    /// Настраивает все стандартные поля (Id + аудит + soft-delete)
    /// </summary>
    public static EntityTypeBuilder<T> ConfigureBaseEntity<T>(this EntityTypeBuilder<T> builder)
        where T : BaseEntity
    {
        builder.HasIdAsKey();
        builder.ConfigureAuditRelations();
        builder.ConfigureSoftDelete();
        return builder;
    }
}