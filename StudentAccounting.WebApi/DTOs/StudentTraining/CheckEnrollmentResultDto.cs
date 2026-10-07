namespace StudentAccounting.WebApi.DTOs.StudentTraining;

/// <summary>
/// Результат предварительной проверки перед зачислением
/// </summary>
public class CheckEnrollmentResultDto
{
    public bool CanEnroll { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<string> Warnings { get; set; } = [];
}