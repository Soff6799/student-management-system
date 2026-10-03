using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.WebApi.DTOs.StudentTraining;

public class StudentTrainingCreateDto
{
    [Required]
    public Guid EmployeeId { get; set; }

    [Required]
    public Guid TrainingGroupId { get; set; }

    [MaxLength(100)]
    public string? CertificateNumber { get; set; }

    [MaxLength(2000)]
    public string? Note { get; set; }
}