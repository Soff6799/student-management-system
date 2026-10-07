using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.WebApi.DTOs.Contract;

/// <summary>
/// Запрос на формирование договора: номер, дата и файлы создаются сервером
/// </summary>
public class ContractCreateDto
{
    [Required]
    public Guid TrainingGroupId { get; set; }
}