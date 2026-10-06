using StudentAccounting.WebApi.DTOs.TrainingProgram;

namespace StudentAccounting.WebApi.Services.TrainingProgram;

public interface ITrainingProgramService
{
    Task<IEnumerable<TrainingProgramDto>> GetAllAsync();
    Task<TrainingProgramDto?> GetByIdAsync(Guid id);
    Task<TrainingProgramDto> CreateAsync(TrainingProgramCreateDto dto);
    Task<TrainingProgramDto> UpdateAsync(TrainingProgramUpdateDto dto);
    Task<TrainingProgramDto> ArchiveAsync(Guid id);
    Task DeleteAsync(Guid id);
}