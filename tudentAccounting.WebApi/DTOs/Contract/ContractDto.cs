namespace StudentAccounting.WebApi.DTOs.Contract;

public class ContractDto
{
    public Guid Id { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public Guid TrainingGroupId { get; set; }
    public string? TrainingGroupName { get; set; }
}