using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.Employee;
using StudentAccounting.WebApi.DTOs.TrainingProgram;

namespace StudentAccounting.WebApi.Services.TrainingProgram;

public interface ITrainingProgramService
{
    Task<PagedResultDto<TrainingProgramDto>> GetPagedAsync(TrainingProgramListParams p);
    Task<TrainingProgramDto?> GetByIdAsync(Guid id);
    Task<TrainingProgramDto> CreateAsync(TrainingProgramCreateDto dto);
    Task<TrainingProgramDto> UpdateAsync(TrainingProgramUpdateDto dto);
    Task<TrainingProgramDto> ArchiveAsync(Guid id);
    Task DeleteAsync(Guid id);
}