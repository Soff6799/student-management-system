using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.StudentTraining;

public class StudentTrainingDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeFullName { get; set; }
    public Guid TrainingGroupId { get; set; }
    public string? TrainingGroupName { get; set; }
    public TrainingStatus Status { get; set; }
    public DateOnly? CompletionDate { get; set; }
    public DateOnly? NextTrainingDate { get; set; }
    public string? CertificateNumber { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? TrainingProgramName { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}