using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.StudentTraining;

namespace StudentAccounting.WebApi.Services.StudentTraining;

/// <summary>
/// Интерфейс сервиса управления записями об обучении
/// </summary>
public interface IStudentTrainingService
{
    /// <summary>
    /// Получить все записи об обучении
    /// </summary>
    Task<IEnumerable<StudentTrainingDto>> GetAllAsync();

    /// <summary>
    /// Получить историю обучения сотрудника
    /// </summary>
    Task<IEnumerable<StudentTrainingDto>> GetByEmployeeIdAsync(Guid employeeId);

    /// <summary>
    /// Получить обучающихся группы с поиском, фильтром, сортировкой и пагинацией
    /// </summary>
    Task<PagedResultDto<StudentTrainingDto>> GetGroupStudentsPagedAsync(Guid trainingGroupId, GroupStudentListParams p);

    /// <summary>
    /// Получить всех обучающихся группы без пагинации (для XLSX)
    /// </summary>
    Task<List<StudentTrainingDto>> GetGroupStudentsFilteredAsync(Guid trainingGroupId, GroupStudentListParams p);

    /// <summary>
    /// Получить запись об обучении по Id
    /// </summary>
    Task<StudentTrainingDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Предварительная проверка перед зачислением (ошибки и предупреждения)
    /// </summary>
    Task<CheckEnrollmentResultDto> CheckEnrollmentAsync(StudentTrainingCreateDto dto);

    /// <summary>
    /// Зачислить сотрудника в группу
    /// </summary>
    Task<StudentTrainingDto> CreateAsync(StudentTrainingCreateDto dto);

    /// <summary>
    /// Обновить запись об обучении (авто-расчёт даты следующего обучения)
    /// </summary>
    Task<StudentTrainingDto> UpdateAsync(StudentTrainingUpdateDto dto);

    /// <summary>
    /// Удалить запись об обучении (мягкое удаление)
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Получить записи об обучении по Id группы
    /// </summary>
    Task<IEnumerable<StudentTrainingDto>> GetByTrainingGroupIdAsync(Guid trainingGroupId);
}