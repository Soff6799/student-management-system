using Microsoft.EntityFrameworkCore;
using StudentAccounting.Dal.Contracts.interfaces;
using StudentAccounting.Domain;
using StudentAccounting.WebApi.DTOs.Education;
using StudentAccounting.WebApi.Services.Common;
using DomainEducation = global::StudentAccounting.Domain.Education;

namespace StudentAccounting.WebApi.Services.Education;

public class EducationService : IEducationService
{
    private readonly IRepository<DomainEducation> _repository;
    private readonly AuditService _auditService;

    public EducationService(IRepository<DomainEducation> repository,
        AuditService auditService)
    {
        _repository = repository;
        _auditService = auditService;
    }

    public async Task<IEnumerable<EducationDto>> GetAllAsync()
    {
        var educations = await _repository.GetAllAsync();
        return educations.Select(MapToDto);
    }

    public async Task<IEnumerable<EducationDto>> GetByEmployeeIdAsync(Guid employeeId)
    {
        var educations = await _repository.GetAllAsync();
        return educations
            .Where(e => e.EmployeeId == employeeId)
            .Select(MapToDto);
    }

    public async Task<EducationDto?> GetByIdAsync(Guid id)
    {
        var education = await _repository.GetByIdAsync(id);
        return education == null ? null : MapToDto(education);
    }

    public async Task<EducationDto> CreateAsync(EducationCreateDto dto)
    {
        var education = new DomainEducation
        {
            Level = dto.Level,
            InstitutionName = dto.InstitutionName,
            GraduationYear = dto.GraduationYear,
            Specialty = dto.Specialty,
            FilePath = dto.FilePath,
            EmployeeId = dto.EmployeeId
        };
        _auditService.SetAuditFields(education);
        await _repository.AddAsync(education);
        await _repository.SaveChangesAsync();
        return MapToDto(education);
    }

    public async Task<EducationDto> UpdateAsync(EducationUpdateDto dto)
    {
        var education = await _repository.GetByIdAsync(dto.Id);
        if (education == null)
            throw new KeyNotFoundException($"Образование с Id {dto.Id} не найдено");

        education.Level = dto.Level;
        education.InstitutionName = dto.InstitutionName;
        education.GraduationYear = dto.GraduationYear;
        education.Specialty = dto.Specialty;
        education.FilePath = dto.FilePath;
        education.EmployeeId = dto.EmployeeId;
        education.UpdatedAt = DateTimeOffset.UtcNow;
        _auditService.SetAuditFields(education);
        await _repository.UpdateAsync(education);
        await _repository.SaveChangesAsync();
        return MapToDto(education);
    }

    public async Task DeleteAsync(Guid id)
    {
        var education = await _repository.GetByIdAsync(id);
        if (education == null)
            throw new KeyNotFoundException($"Образование с Id {id} не найдено");

        education.DeletedAt = DateTimeOffset.UtcNow;
        await _repository.UpdateAsync(education);
        await _repository.SaveChangesAsync();
    }

    private static EducationDto MapToDto(DomainEducation edu)
    {
        return new EducationDto
        {
            Id = edu.Id,
            Level = edu.Level,
            InstitutionName = edu.InstitutionName,
            GraduationYear = edu.GraduationYear,
            Specialty = edu.Specialty,
            FilePath = edu.FilePath,
            EmployeeId = edu.EmployeeId,
            EmployeeFullName = edu.Employee != null
                ? $"{edu.Employee.LastName} {edu.Employee.FirstName} {edu.Employee.Patronymic}".Trim()
                : null,
            CreatedAt = edu.CreatedAt,
            UpdatedAt = edu.UpdatedAt
        };
    }
}