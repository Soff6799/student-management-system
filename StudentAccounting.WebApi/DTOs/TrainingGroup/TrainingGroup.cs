using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.TrainingGroup;

/// <summary>
/// DTO группы обучения
/// </summary>
public class TrainingGroupDto
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public Guid TrainingProgramId { get; set; }
    public string? TrainingProgramName { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public GroupStatus Status { get; set; }
    public string? Note { get; set; }
    public int StudentsCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}