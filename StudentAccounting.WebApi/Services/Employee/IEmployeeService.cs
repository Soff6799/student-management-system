using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.Employee;
using StudentAccounting.WebApi.DTOs.Organization;

namespace StudentAccounting.WebApi.Services.Employee;

public interface IEmployeeService
{
    Task<PagedResultDto<EmployeeDto>> GetPagedAsync(EmployeeListParams p);
    Task<List<EmployeeDto>> GetFilteredAsync(EmployeeListParams p);
    Task<EmployeeDto?> GetByIdAsync(Guid id);
    Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);
    Task<EmployeeDto> UpdateAsync(EmployeeUpdateDto dto);
    Task DeleteAsync(Guid id);
}