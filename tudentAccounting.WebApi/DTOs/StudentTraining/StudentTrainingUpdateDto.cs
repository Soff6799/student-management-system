using System.ComponentModel.DataAnnotations;
using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.StudentTraining;

public class StudentTrainingUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public TrainingStatus Status { get; set; }

    public DateOnly? CompletionDate { get; set; }

    [MaxLength(100)]
    public string? CertificateNumber { get; set; }

    [MaxLength(2000)]
    public string? Note { get; set; }
}