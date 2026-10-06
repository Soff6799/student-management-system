using Microsoft.EntityFrameworkCore;
using StudentAccounting.Dal.Contracts.interfaces;
using StudentAccounting.Domain;
using StudentAccounting.WebApi.DTOs.Employee;
using DomainEmployee = global::StudentAccounting.Domain.Employee;

namespace StudentAccounting.WebApi.Services.Employee;

public class EmployeeService : IEmployeeService
{
    private readonly IRepository<DomainEmployee> _repository;

    public EmployeeService(IRepository<DomainEmployee> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();
        return employees.Select(MapToDto);
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        var employee = await _repository.GetByIdAsync(id);
        return employee == null ? null : MapToDto(employee);
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto)
    {
        var employee = new DomainEmployee
        {
            LastName = dto.LastName,
            FirstName = dto.FirstName,
            Patronymic = dto.Patronymic,
            Phone = dto.Phone,
            Email = dto.Email,
            Post = dto.Post,
            OrganizationId = dto.OrganizationId
        };

        await _repository.AddAsync(employee);
        await _repository.SaveChangesAsync();
        return MapToDto(employee);
    }

    public async Task<EmployeeDto> UpdateAsync(EmployeeUpdateDto dto)
    {
        var employee = await _repository.GetByIdAsync(dto.Id);
        if (employee == null)
            throw new KeyNotFoundException($"Сотрудник с Id {dto.Id} не найден");

        employee.LastName = dto.LastName;
        employee.FirstName = dto.FirstName;
        employee.Patronymic = dto.Patronymic;
        employee.Phone = dto.Phone;
        employee.Email = dto.Email;
        employee.Post = dto.Post;
        employee.OrganizationId = dto.OrganizationId;
        employee.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.UpdateAsync(employee);
        await _repository.SaveChangesAsync();
        return MapToDto(employee);
    }

    public async Task DeleteAsync(Guid id)
    {
        var employee = await _repository.GetByIdAsync(id);
        if (employee == null)
            throw new KeyNotFoundException($"Сотрудник с Id {id} не найден");

        employee.DeletedAt = DateTimeOffset.UtcNow;
        await _repository.UpdateAsync(employee);
        await _repository.SaveChangesAsync();
    }

    private static EmployeeDto MapToDto(DomainEmployee emp)
    {
        return new EmployeeDto
        {
            Id = emp.Id,
            LastName = emp.LastName,
            FirstName = emp.FirstName,
            Patronymic = emp.Patronymic,
            Phone = emp.Phone,
            Email = emp.Email,
            Post = emp.Post,
            OrganizationId = emp.OrganizationId,
            OrganizationName = emp.Organization?.ShortName,
            CreatedAt = emp.CreatedAt,
            UpdatedAt = emp.UpdatedAt
        };
    }
}