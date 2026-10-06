using System.ComponentModel.DataAnnotations;
using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.TrainingGroup;

public class TrainingGroupUpdateDto : IValidatableObject
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string GroupName { get; set; } = string.Empty;

    [Required]
    public Guid TrainingProgramId { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public GroupStatus Status { get; set; }

    [MaxLength(2000)]
    public string? Note { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate.HasValue && EndDate.Value < StartDate)
            yield return new ValidationResult(
                "Дата окончания не может быть раньше даты начала",
                new[] { nameof(EndDate) });
    }
}