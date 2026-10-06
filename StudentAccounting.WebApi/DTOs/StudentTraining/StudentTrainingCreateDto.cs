using System.ComponentModel.DataAnnotations;
using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.StudentTraining;

public class StudentTrainingCreateDto
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public Guid TrainingGroupId { get; set; }

    [Required]
    public TrainingStatus Status { get; set; } = TrainingStatus.Enrolled;

    public DateOnly? CompletionDate { get; set; }

    [MaxLength(100)]
    public string? CertificateNumber { get; set; }

    [MaxLength(2000)]
    public string? Note { get; set; }
}