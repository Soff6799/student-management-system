using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.WebApi.DTOs.Contract;

public class ContractUpdateDto
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid TrainingGroupId { get; set; }

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;
}