namespace tudentAccounting.WebApi.DTOs.TrainingProgram;

public class TrainingProgramDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? CostRubles { get; set; }
    public int GroupsCount { get; set; }
}