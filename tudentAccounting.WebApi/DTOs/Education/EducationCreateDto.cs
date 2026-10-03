using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.WebApi.DTOs.Education;

public class EducationCreateDto
{
    [Required]
    [MaxLength(255)]
    public string InstitutionName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Specialty { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    public Guid EmployeeId { get; set; }
}