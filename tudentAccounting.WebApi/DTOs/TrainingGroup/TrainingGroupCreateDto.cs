using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.WebApi.DTOs.TrainingGroup;

public class TrainingGroupCreateDto
{
    [Required]
    [MaxLength(255)]
    public string GroupName { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Note { get; set; }

    [Required]
    public Guid TrainingProgramId { get; set; }
}