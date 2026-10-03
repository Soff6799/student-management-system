namespace StudentAccounting.WebApi.DTOs.StudentTraining;

public class StudentTrainingDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string? EmployeeFullName { get; set; }
    public Guid TrainingGroupId { get; set; }
    public string? TrainingGroupName { get; set; }
    public string? CertificateNumber { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}