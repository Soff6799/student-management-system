using System.ComponentModel.DataAnnotations;
using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.TrainingProgram;

public class TrainingProgramCreateDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal CostRubles { get; set; }

    public RequirementsEducation? RequirementsEducation { get; set; }

    /// <summary>Периодичность повторного обучения (обязательно по ТЗ)</summary>
    public RetrainingPeriodicity RetrainingPeriodicity { get; set; }

    /// <summary>Заполняется только при RetrainingPeriodicity = CustomInMonths</summary>
    [Range(1, 1200)]
    public int? CustomRetrainingMonths { get; set; }

    /// <summary>
    /// Срок обучения: значение
    /// </summary>
    [Range(1, int.MaxValue)]
    public int? DurationValue { get; set; }

    /// <summary>
    /// Срок обучения: единица измерения (часов/дней)
    /// </summary>
    public DurationUnit? DurationUnit { get; set; }

    /// <summary>
    /// Заблаговременность уведомления, по умолчанию 60 дней (ТЗ 4.f.iii)
    /// </summary>
    [Range(0, 365)]
    public int NotificationLeadTimeDays { get; set; } = 60;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (RetrainingPeriodicity == RetrainingPeriodicity.CustomInMonths && !CustomRetrainingMonths.HasValue)
            yield return new ValidationResult(
                "Для периодичности «Произвольный срок (в месяцах)» укажите количество месяцев",
                new[] { nameof(CustomRetrainingMonths) });

        if (DurationValue.HasValue != DurationUnit.HasValue)
            yield return new ValidationResult(
                "Срок обучения указывается парой: значение + единица измерения (часов/дней)",
                new[] { nameof(DurationValue), nameof(DurationUnit) });
    }
}