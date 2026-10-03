using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.WebApi.DTOs.Contract;

public class ContractCreateDto
{
    [Required]
    [MaxLength(50)]
    public string ContractNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [Required]
    public Guid TrainingGroupId { get; set; }
}