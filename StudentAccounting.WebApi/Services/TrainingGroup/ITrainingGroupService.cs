using StudentAccounting.WebApi.DTOs.TrainingGroup;

namespace StudentAccounting.WebApi.Services.TrainingGroup;

public interface ITrainingGroupService
{
    Task<IEnumerable<TrainingGroupDto>> GetAllAsync();
    Task<TrainingGroupDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<TrainingGroupDto>> GetByProgramIdAsync(Guid trainingProgramId);
    Task<TrainingGroupDto> CreateAsync(TrainingGroupCreateDto dto);
    Task<TrainingGroupDto> UpdateAsync(TrainingGroupUpdateDto dto);
    Task DeleteAsync(Guid id);
}