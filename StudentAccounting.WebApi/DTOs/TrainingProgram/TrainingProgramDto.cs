using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.TrainingProgram;

/// <summary>
/// DTO программы обучения
/// </summary>
public class TrainingProgramDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal CostRubles { get; set; }
    public RequirementsEducation? RequirementsEducation { get; set; }
    public RetrainingPeriodicity? RetrainingPeriodicity { get; set; }
    public int? CustomRetrainingMonths { get; set; }
    public int? DurationValue { get; set; }
    public DurationUnit? DurationUnit { get; set; }
    public ProgramStatus Status { get; set; }
    public int NotificationLeadTimeDays { get; set; }
    public int GroupsCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}