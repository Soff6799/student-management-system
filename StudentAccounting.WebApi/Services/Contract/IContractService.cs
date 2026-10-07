using StudentAccounting.WebApi.DTOs.Contract;

namespace StudentAccounting.WebApi.Services.Contract;

public interface IContractService
{
    Task<IEnumerable<ContractDto>> GetAllAsync();
    Task<ContractDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ContractDto>> GetByTrainingGroupIdAsync(Guid trainingGroupId);
    Task<ContractDto> CreateAsync(ContractCreateDto dto);
    Task<ContractDto> RegenerateAsync(Guid id);
    Task<(byte[] Content, string ContentType, string FileName)> GetDocumentAsync(Guid id, string format);
    Task DeleteAsync(Guid id);
}