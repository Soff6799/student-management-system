using StudentAccounting.WebApi.DTOs.StudentTraining;

namespace StudentAccounting.WebApi.Services.StudentTraining;

public interface IStudentTrainingService
{
    Task<IEnumerable<StudentTrainingDto>> GetAllAsync();
    Task<IEnumerable<StudentTrainingDto>> GetByEmployeeIdAsync(Guid employeeId);
    Task<IEnumerable<StudentTrainingDto>> GetByTrainingGroupIdAsync(Guid trainingGroupId);
    Task<StudentTrainingDto?> GetByIdAsync(Guid id);
    Task<StudentTrainingDto> CreateAsync(StudentTrainingCreateDto dto);
    Task<StudentTrainingDto> UpdateAsync(StudentTrainingUpdateDto dto);
    Task DeleteAsync(Guid id);

    Task<CheckEnrollmentResultDto> CheckEnrollmentAsync(StudentTrainingCreateDto dto);
}