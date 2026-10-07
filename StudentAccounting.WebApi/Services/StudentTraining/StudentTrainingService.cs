using Microsoft.EntityFrameworkCore;
using StudentAccounting.Dal.Contracts.interfaces;
using StudentAccounting.Domain;
using StudentAccounting.WebApi.DTOs.StudentTraining;
using StudentAccounting.Infrastructure.Data;
using static StudentAccounting.Domain.Enums;
using DomainStudentTraining = global::StudentAccounting.Domain.StudentTraining;

namespace StudentAccounting.WebApi.Services.StudentTraining;

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
        var trainings = await WithIncludes()
            .ToListAsync();
        return trainings.Select(MapToDto);
    }

    public async Task<IEnumerable<StudentTrainingDto>> GetByEmployeeIdAsync(Guid employeeId)
    {
        var trainings = await WithIncludes()
            .Where(t => t.EmployeeId == employeeId)
            .ToListAsync();
        return trainings.Select(MapToDto);
    }

    public async Task<IEnumerable<StudentTrainingDto>> GetByTrainingGroupIdAsync(Guid trainingGroupId)
    {
        var trainings = await WithIncludes()
        .Where(t => t.TrainingGroupId == trainingGroupId)
        .ToListAsync();
        return trainings.Select(MapToDto);
    }

    public async Task<StudentTrainingDto?> GetByIdAsync(Guid id)
    {
        var training = await WithIncludes()
        .FirstOrDefaultAsync(t => t.Id == id);
        return training == null ? null : MapToDto(training);
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

        // Автоматический расчёт NextTrainingDate если статус = Прошёл обучение
        if (dto.Status == TrainingStatus.Completed && dto.CompletionDate.HasValue)
        {
            training.NextTrainingDate = await CalculateNextTrainingDate(dto.TrainingGroupId, dto.CompletionDate.Value);
        }

        await _repository.AddAsync(training);
        await _repository.SaveChangesAsync();
        return MapToDto(training);
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
        if (dto.Status == TrainingStatus.Completed)
        {
            training.CompletionDate ??= DateOnly.FromDateTime(DateTime.UtcNow);
            var program = training.TrainingGroup?.TrainingProgram;
            var months = GetPeriodicityMonths(program?.RetrainingPeriodicity, program?.CustomRetrainingMonths);
            if (months.HasValue)
            {
                training.NextTrainingDate = training.CompletionDate.Value.AddMonths(months.Value);
            }
        }
        else
        {
            training.NextTrainingDate = null;
        }
        training.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return MapToDto(training);
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
            _ => (int)periodicity.Value > 0 ? (int)periodicity.Value : null
        };
    }

    private static StudentTrainingDto MapToDto(DomainStudentTraining training)
    {
        return new StudentTrainingDto
        {
            Id = training.Id,
            EmployeeId = training.EmployeeId,
            EmployeeFullName = training.Employee != null
                ? $"{training.Employee.LastName} {training.Employee.FirstName} {training.Employee.Patronymic}".Trim()
                : null,
            TrainingGroupId = training.TrainingGroupId,
            TrainingGroupName = training.TrainingGroup?.GroupName,
            Status = training.Status,
            CompletionDate = training.CompletionDate,
            NextTrainingDate = training.NextTrainingDate,
            CertificateNumber = training.CertificateNumber,
            Note = training.Note,
            CreatedAt = training.CreatedAt,
            UpdatedAt = training.UpdatedAt,
            TrainingProgramName = training.TrainingGroup?.TrainingProgram?.Name,
            StartDate = training.TrainingGroup?.StartDate,
            EndDate = training.TrainingGroup?.EndDate
        };
    }

    /// <summary>
    /// Предварительная проверка перед зачислением:
    /// дубликат — запрет; уволенный сотрудник и несоответствие образования — предупреждения
    /// </summary>
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

        // запрет дублирования зачисления в ту же группу
        var duplicate = await _context.StudentTrainings
            .AnyAsync(t => t.EmployeeId == dto.EmployeeId
                        && t.TrainingGroupId == dto.TrainingGroupId
                        && t.Status != TrainingStatus.DroppedOut);
        if (duplicate)
        {
            result.CanEnroll = false;
            result.Errors.Add("Сотрудник уже зачислен в эту группу");
        }

        // предупреждение об уволенном сотруднике
        if (employee.Status == EmployeeStatus.Dismissed)
            result.Warnings.Add("Сотрудник уволен — проверьте целесообразность зачисления");

        // предупреждение о несоответствии образования
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

    /// <summary>
    /// Ранг уровня образования для сравнения с требованиями
    /// </summary>
    private static int GetRank(EducationLevel level) => level switch
    {
        EducationLevel.SecondaryGeneral => 1,
        EducationLevel.SecondaryVocational => 2,
        EducationLevel.Higher => 3,
        _ => 0
    };

    private static int GetRank(RequirementsEducation req) => req switch
    {
        RequirementsEducation.SecondaryGeneral => 1,
        RequirementsEducation.SecondaryVocational => 2,
        RequirementsEducation.Higher => 3,
        _ => 0
    };

    private IQueryable<DomainStudentTraining> WithIncludes()
    {
        return _context.StudentTrainings
            .Include(t => t.Employee).ThenInclude(e => e.Organization)
            .Include(t => t.TrainingGroup).ThenInclude(g => g.TrainingProgram);
    }
}