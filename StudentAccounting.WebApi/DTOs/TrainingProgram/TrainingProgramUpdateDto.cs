using System.ComponentModel.DataAnnotations;
using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.TrainingProgram;

public class TrainingProgramUpdateDto : IValidatableObject
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal CostRubles { get; set; }

    public RequirementsEducation? RequirementsEducation { get; set; }

    public RetrainingPeriodicity RetrainingPeriodicity { get; set; }

    [Range(1, 1200)]
    public int? CustomRetrainingMonths { get; set; }

    [Range(1, int.MaxValue)]
    public int? DurationValue { get; set; }

    public DurationUnit? DurationUnit { get; set; }

    public ProgramStatus Status { get; set; }

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