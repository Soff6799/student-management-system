namespace StudentAccounting.WebApi.DTOs.Contract;

/// <summary>
/// DTO договора на обучение
/// </summary>
public class ContractDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public DateOnly GenerationDate { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public Guid TrainingGroupId { get; set; }
    public string? TrainingGroupName { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}