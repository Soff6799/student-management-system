using Microsoft.EntityFrameworkCore;
using StudentAccounting.Dal.Contracts.interfaces;
using StudentAccounting.Domain;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.StudentTraining;
using static StudentAccounting.Domain.Enums;
using DomainStudentTraining = global::StudentAccounting.Domain.StudentTraining;

namespace StudentAccounting.WebApi.Services.StudentTraining;

/// <summary>
/// Сервис управления записями о прохождении обучения
/// </summary>
public class StudentTrainingService : IStudentTrainingService
{
    private readonly IRepository<DomainStudentTraining> _repository;
    private readonly AppDbContext _context;

    public StudentTrainingService(IRepository<DomainStudentTraining> repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<IEnumerable<StudentTrainingDto>> GetAllAsync()
    {
        var trainings = await WithIncludes().ToListAsync();
        return trainings.Select(MapToDto);
    }

    public async Task<IEnumerable<StudentTrainingDto>> GetByEmployeeIdAsync(Guid employeeId)
    {
        var trainings = await WithIncludes()
            .Where(t => t.EmployeeId == employeeId)
            .ToListAsync();
        return trainings.Select(MapToDto);
    }

    public async Task<PagedResultDto<StudentTrainingDto>> GetGroupStudentsPagedAsync(Guid trainingGroupId, GroupStudentListParams p)
    {
        var query = ApplyStudentSorting(
            ApplyStudentFilters(WithIncludes().Where(t => t.TrainingGroupId == trainingGroupId), p), p);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((p.Page - 1) * p.PageSize)
            .Take(p.PageSize)
            .ToListAsync();

        return new PagedResultDto<StudentTrainingDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = p.Page,
            PageSize = p.PageSize
        };
    }

    public async Task<List<StudentTrainingDto>> GetGroupStudentsFilteredAsync(Guid trainingGroupId, GroupStudentListParams p)
    {
        var items = await ApplyStudentSorting(
            ApplyStudentFilters(WithIncludes().Where(t => t.TrainingGroupId == trainingGroupId), p), p)
            .ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    public async Task<StudentTrainingDto?> GetByIdAsync(Guid id)
    {
        var training = await WithIncludes().FirstOrDefaultAsync(t => t.Id == id);
        return training == null ? null : MapToDto(training);
    }

    public async Task<CheckEnrollmentResultDto> CheckEnrollmentAsync(StudentTrainingCreateDto dto)
    {
        var result = new CheckEnrollmentResultDto { CanEnroll = true };

        var employee = await _context.Employees
            .Include(e => e.Educations)
            .FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);
        if (employee == null)
        {
            result.CanEnroll = false;
            result.Errors.Add("Сотрудник не найден");
            return result;
        }

        var group = await _context.TrainingGroups
            .Include(g => g.TrainingProgram)
            .FirstOrDefaultAsync(g => g.Id == dto.TrainingGroupId);
        if (group == null)
        {
            result.CanEnroll = false;
            result.Errors.Add("Группа обучения не найдена");
            return result;
        }

        // ТЗ 4.d.iv.3: запрет дублирования зачисления в ту же группу
        var duplicate = await _context.StudentTrainings
            .AnyAsync(t => t.EmployeeId == dto.EmployeeId
                        && t.TrainingGroupId == dto.TrainingGroupId
                        && t.Status != TrainingStatus.DroppedOut);
        if (duplicate)
        {
            result.CanEnroll = false;
            result.Errors.Add("Сотрудник уже зачислен в эту группу");
        }

        // ТЗ 4.d.iv.1: предупреждение об уволенном сотруднике
        if (employee.Status == EmployeeStatus.Dismissed)
            result.Warnings.Add("Сотрудник уволен — проверьте целесообразность зачисления");

        // ТЗ 4.d.iv.2 / 4.b.v: предупреждение о несоответствии образования
        var required = group.TrainingProgram?.RequirementsEducation;
        if (required is RequirementsEducation req
            && req != RequirementsEducation.NotRequired
            && req != RequirementsEducation.Other)
        {
            var requiredRank = GetRank(req);
            var confirmed = employee.Educations.Any(e => GetRank(e.Level) >= requiredRank);
            if (!confirmed)
                result.Warnings.Add($"Программа требует образование «{req}», у сотрудника нет подтверждающей записи в разделе «Образование»");
        }

        return result;
    }

    public async Task<StudentTrainingDto> CreateAsync(StudentTrainingCreateDto dto)
    {
        var check = await CheckEnrollmentAsync(dto);
        if (!check.CanEnroll)
            throw new InvalidOperationException(string.Join("; ", check.Errors));

        var training = new DomainStudentTraining
        {
            EmployeeId = dto.EmployeeId,
            TrainingGroupId = dto.TrainingGroupId,
            Status = dto.Status,
            CompletionDate = dto.CompletionDate,
            CertificateNumber = dto.CertificateNumber,
            Note = dto.Note
        };

        // Автоматический расчёт NextTrainingDate, если сразу создан со статусом «Прошёл обучение»
        if (dto.Status == TrainingStatus.Completed && dto.CompletionDate.HasValue)
        {
            training.NextTrainingDate = await CalculateNextTrainingDate(dto.TrainingGroupId, dto.CompletionDate.Value);
        }

        await _repository.AddAsync(training);
        await _repository.SaveChangesAsync();
        return (await GetByIdAsync(training.Id))!;
    }

    public async Task<StudentTrainingDto> UpdateAsync(StudentTrainingUpdateDto dto)
    {
        var training = await _context.StudentTrainings
            .Include(t => t.TrainingGroup)
                .ThenInclude(g => g.TrainingProgram)
            .FirstOrDefaultAsync(t => t.Id == dto.Id);

        if (training == null)
            throw new KeyNotFoundException($"Запись об обучении с Id {dto.Id} не найдена");

        training.Status = dto.Status;
        training.CertificateNumber = dto.CertificateNumber;
        training.Note = dto.Note;
        training.CompletionDate = dto.CompletionDate;

        // ТЗ 4.e.ii: авто-расчёт даты следующего обучения при статусе «Прошёл обучение»
        if (dto.Status == TrainingStatus.Completed)
        {
            training.CompletionDate ??= DateOnly.FromDateTime(DateTime.UtcNow);

            var program = training.TrainingGroup?.TrainingProgram;
            var months = GetPeriodicityMonths(program?.RetrainingPeriodicity, program?.CustomRetrainingMonths);
            if (months.HasValue)
                training.NextTrainingDate = training.CompletionDate.Value.AddMonths(months.Value);
        }
        else
        {
            training.NextTrainingDate = null;
        }

        training.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(training.Id))!;
    }

    public async Task DeleteAsync(Guid id)
    {
        var training = await _repository.GetByIdAsync(id);
        if (training == null)
            throw new KeyNotFoundException($"Запись об обучении с Id {id} не найдена");

        training.DeletedAt = DateTimeOffset.UtcNow;
        await _repository.UpdateAsync(training);
        await _repository.SaveChangesAsync();
    }

    /// <summary>
    /// Базовый запрос с загрузкой связанных данных
    /// </summary>
    private IQueryable<DomainStudentTraining> WithIncludes()
    {
        return _context.StudentTrainings
            .Include(t => t.Employee).ThenInclude(e => e.Organization)
            .Include(t => t.TrainingGroup).ThenInclude(g => g.TrainingProgram);
    }

    /// <summary>
    /// Применение поиска и фильтров списка обучающихся группы
    /// </summary>
    private static IQueryable<DomainStudentTraining> ApplyStudentFilters(IQueryable<DomainStudentTraining> query, GroupStudentListParams p)
    {
        if (p.Status.HasValue)
            query = query.Where(t => t.Status == p.Status.Value);

        if (!string.IsNullOrWhiteSpace(p.Search))
        {
            var s = p.Search.Trim();
            query = query.Where(t =>
                t.Employee.LastName.Contains(s) ||
                t.Employee.FirstName.Contains(s) ||
                (t.Employee.Patronymic != null && t.Employee.Patronymic.Contains(s)));
        }

        return query;
    }

    /// <summary>
    /// Применение сортировки списка обучающихся группы
    /// </summary>
    private static IQueryable<DomainStudentTraining> ApplyStudentSorting(IQueryable<DomainStudentTraining> query, GroupStudentListParams p)
    {
        return p.SortBy.ToLower() switch
        {
            "employee" => p.Descending ? query.OrderByDescending(t => t.Employee.LastName) : query.OrderBy(t => t.Employee.LastName),
            "status" => p.Descending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            _ => p.Descending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
        };
    }

    /// <summary>
    /// Рассчитывает дату следующего обучения на основе периодичности программы
    /// </summary>
    private async Task<DateOnly?> CalculateNextTrainingDate(Guid trainingGroupId, DateOnly completionDate)
    {
        var trainingGroup = await _context.TrainingGroups
            .Include(g => g.TrainingProgram)
            .FirstOrDefaultAsync(g => g.Id == trainingGroupId);

        var program = trainingGroup?.TrainingProgram;
        var months = GetPeriodicityMonths(program?.RetrainingPeriodicity, program?.CustomRetrainingMonths);

        return months.HasValue ? completionDate.AddMonths(months.Value) : null;
    }

    /// <summary>
    /// Преобразует периодичность повторного обучения в количество месяцев
    /// </summary>
    private static int? GetPeriodicityMonths(RetrainingPeriodicity? periodicity, int? customMonths)
    {
        if (periodicity is null)
            return null;

        return periodicity.Value switch
        {
            RetrainingPeriodicity.OneYear => 12,
            RetrainingPeriodicity.ThreeYears => 36,
            RetrainingPeriodicity.FiveYears => 60,
            RetrainingPeriodicity.CustomInMonths => customMonths,
            _ => null
        };
    }

    /// <summary>
    /// Ранг уровня образования для сравнения с требованиями программы
    /// </summary>
    private static int GetRank(EducationLevel level)
    {
        return level switch
        {
            EducationLevel.SecondaryGeneral => 1,
            EducationLevel.SecondaryVocational => 2,
            EducationLevel.Higher => 3,
            _ => 0
        };
    }

    /// <summary>
    /// Ранг требования к образованию для сравнения с уровнем сотрудника
    /// </summary>
    private static int GetRank(RequirementsEducation req)
    {
        return req switch
        {
            RequirementsEducation.SecondaryGeneral => 1,
            RequirementsEducation.SecondaryVocational => 2,
            RequirementsEducation.Higher => 3,
            _ => 0
        };
    }

    /// <summary>
    /// Маппинг сущности записи об обучении в DTO
    /// </summary>
    private static StudentTrainingDto MapToDto(DomainStudentTraining training)
    {
        return new StudentTrainingDto
        {
            Id = training.Id,
            EmployeeId = training.EmployeeId,
            EmployeeFullName = training.Employee != null
                ? $"{training.Employee.LastName} {training.Employee.FirstName} {training.Employee.Patronymic}".Trim()
                : null,
            OrganizationName = training.Employee?.Organization?.ShortName,
            Post = training.Employee?.Post,
            TrainingGroupId = training.TrainingGroupId,
            TrainingGroupName = training.TrainingGroup?.GroupName,
            TrainingProgramName = training.TrainingGroup?.TrainingProgram?.Name,
            StartDate = training.TrainingGroup?.StartDate,
            EndDate = training.TrainingGroup?.EndDate,
            Status = training.Status,
            CompletionDate = training.CompletionDate,
            NextTrainingDate = training.NextTrainingDate,
            CertificateNumber = training.CertificateNumber,
            Note = training.Note,
            CreatedAt = training.CreatedAt,
            UpdatedAt = training.UpdatedAt == default ? null : training.UpdatedAt
        };
    }

    public async Task<IEnumerable<StudentTrainingDto>> GetByTrainingGroupIdAsync(Guid trainingGroupId)
    {
        var trainings = await _context.StudentTrainings
            .Include(t => t.Employee)
            .Include(t => t.TrainingGroup)
                .ThenInclude(g => g.TrainingProgram)
            .Where(t => t.TrainingGroupId == trainingGroupId && t.DeletedAt == null)
            .ToListAsync();
        return trainings.Select(MapToDto);
    }
}