using StudentAccounting.WebApi.DTOs.Education;

namespace StudentAccounting.WebApi.Services.Education;

public interface IEducationService
{
    Task<IEnumerable<EducationDto>> GetAllAsync();
    Task<IEnumerable<EducationDto>> GetByEmployeeIdAsync(Guid employeeId);
    Task<EducationDto?> GetByIdAsync(Guid id);
    Task<EducationDto> CreateAsync(EducationCreateDto dto);
    Task<EducationDto> UpdateAsync(EducationUpdateDto dto);
    Task DeleteAsync(Guid id);
}