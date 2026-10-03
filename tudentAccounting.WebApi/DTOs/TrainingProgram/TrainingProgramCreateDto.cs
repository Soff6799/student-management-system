using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.WebApi.DTOs.TrainingProgram;

public class TrainingProgramCreateDto
{
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? CostRubles { get; set; }
}