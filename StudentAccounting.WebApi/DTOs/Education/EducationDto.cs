using static StudentAccounting.Domain.Enums;

namespace StudentAccounting.WebApi.DTOs.Education;

public class EducationDto
{
    public Guid Id { get; set; }
    public EducationLevel Level { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public string Specialty { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public Guid EmployeeId { get; set; }
    public string? EmployeeFullName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}