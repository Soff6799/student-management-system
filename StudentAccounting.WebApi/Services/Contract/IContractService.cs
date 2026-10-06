using StudentAccounting.WebApi.DTOs.Contract;

namespace StudentAccounting.WebApi.Services.Contract;

public interface IContractService
{
    Task<IEnumerable<ContractDto>> GetAllAsync();
    Task<ContractDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ContractDto>> GetByTrainingGroupIdAsync(Guid trainingGroupId);
    Task<ContractDto> CreateAsync(ContractCreateDto dto);
    Task<ContractDto> UpdateAsync(ContractUpdateDto dto);
    Task DeleteAsync(Guid id);
}