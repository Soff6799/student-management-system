using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using StudentAccounting.Dal.Contracts.interfaces;
using StudentAccounting.Domain;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.Employee;
using static StudentAccounting.Domain.Enums;
using DomainEmployee = global::StudentAccounting.Domain.Employee;

namespace StudentAccounting.WebApi.Services.Employee;

public class EmployeeService : IEmployeeService
{
    private readonly IRepository<DomainEmployee> _repository;
    private readonly AppDbContext _context;

    public EmployeeService(IRepository<DomainEmployee> repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
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

    public async Task<PagedResultDto<EmployeeDto>> GetPagedAsync(EmployeeListParams p)
    {
        var query = ApplySorting(ApplyFilters(_context.Employees, p), p);
        var totalCount = await query.CountAsync();
        var items = await query.Skip((p.Page - 1) * p.PageSize).Take(p.PageSize).ToListAsync();

        return new PagedResultDto<EmployeeDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = p.Page,
            PageSize = p.PageSize
        };
    }

    public async Task<List<EmployeeDto>> GetFilteredAsync(EmployeeListParams p)
    {
        var items = await ApplySorting(ApplyFilters(_context.Employees, p), p).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    private static IQueryable<DomainEmployee> ApplyFilters(IQueryable<DomainEmployee> query, EmployeeListParams p)
    {
        // уволенные скрыты по умолчанию
        if (!p.IncludeDismissed)
            query = query.Where(e => e.Status == EmployeeStatus.Active);

        if (p.OrganizationId.HasValue)
            query = query.Where(e => e.OrganizationId == p.OrganizationId.Value);

        if (p.EducationLevel.HasValue)
            query = query.Where(e => e.Educations.Any(ed => ed.Level == p.EducationLevel.Value));

        if (!string.IsNullOrWhiteSpace(p.Search))
        {
            var s = p.Search.Trim();
            query = query.Where(e =>
                e.LastName.Contains(s) ||
                e.FirstName.Contains(s) ||
                (e.Patronymic != null && e.Patronymic.Contains(s)) ||
                (e.Post != null && e.Post.Contains(s)) ||
                (e.Phone != null && e.Phone.Contains(s)) ||
                (e.Email != null && e.Email.Contains(s)));
        }
        return query;
    }

    private static IQueryable<DomainEmployee> ApplySorting(IQueryable<DomainEmployee> query, EmployeeListParams p)
    {
        return p.SortBy.ToLower() switch
        {
            "lastname" => p.Descending ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName),
            "firstname" => p.Descending ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName),
            "post" => p.Descending ? query.OrderByDescending(e => e.Post) : query.OrderBy(e => e.Post),
            _ => p.Descending ? query.OrderByDescending(e => e.CreatedAt) : query.OrderBy(e => e.CreatedAt)
        };
    }
}