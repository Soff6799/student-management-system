using StudentAccounting.WebApi.DTOs.Employee;

namespace StudentAccounting.WebApi.Services.Employee;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(Guid id);
    Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);
    Task<EmployeeDto> UpdateAsync(EmployeeUpdateDto dto);
    Task DeleteAsync(Guid id);
}