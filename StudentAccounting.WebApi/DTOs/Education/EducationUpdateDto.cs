using System.ComponentModel.DataAnnotations;
using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.Education;

public class EducationUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public EducationLevel Level { get; set; }

    [Required]
    [MaxLength(255)]
    public string InstitutionName { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2100)]
    public int GraduationYear { get; set; }

    [Required]
    [MaxLength(255)]
    public string Specialty { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    public Guid EmployeeId { get; set; }
}