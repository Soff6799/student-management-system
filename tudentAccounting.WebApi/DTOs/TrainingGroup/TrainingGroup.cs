namespace StudentAccounting.WebApi.DTOs.TrainingGroup;

public class TrainingGroupDto
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string? Note { get; set; }
    public Guid TrainingProgramId { get; set; }
    public string? TrainingProgramName { get; set; }
    public int StudentsCount { get; set; }
}