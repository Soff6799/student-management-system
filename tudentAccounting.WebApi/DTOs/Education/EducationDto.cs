namespace StudentAccounting.WebApi.DTOs.Education;

public class EducationDto
{
    public Guid Id { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public Guid EmployeeId { get; set; }
    public string? EmployeeFullName { get; set; }
}