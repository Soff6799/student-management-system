using Microsoft.EntityFrameworkCore;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.DTOs.TrainingProgram;
using static StudentAccounting.Domain.Enums;
using DomainTrainingProgram = global::StudentAccounting.Domain.TrainingProgram;

namespace StudentAccounting.WebApi.Services.TrainingProgram;

public class TrainingProgramService : ITrainingProgramService
{
    private readonly AppDbContext _context;

    public TrainingProgramService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TrainingProgramDto>> GetAllAsync()
    {
        var programs = await _context.TrainingPrograms
            .Include(p => p.TrainingGroups)
            .ToListAsync();
        return programs.Select(MapToDto);
    }

    public async Task<TrainingProgramDto?> GetByIdAsync(Guid id)
    {
        var program = await _context.TrainingPrograms
            .Include(p => p.TrainingGroups)
            .FirstOrDefaultAsync(p => p.Id == id);
        return program == null ? null : MapToDto(program);
    }

    public async Task<TrainingProgramDto> CreateAsync(TrainingProgramCreateDto dto)
    {
        var program = new DomainTrainingProgram
        {
            Name = dto.Name,
            Description = dto.Description,
            CostRubles = dto.CostRubles,
            RequirementsEducation = dto.RequirementsEducation,
            RetrainingPeriodicity = dto.RetrainingPeriodicity,
            CustomRetrainingMonths = dto.CustomRetrainingMonths,
            DurationValue = dto.DurationValue,
            DurationUnit = dto.DurationUnit,
            NotificationLeadTimeDays = dto.NotificationLeadTimeDays,
            Status = ProgramStatus.Active
        };

        await _context.TrainingPrograms.AddAsync(program);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(program.Id))!;
    }

    public async Task<TrainingProgramDto> UpdateAsync(TrainingProgramUpdateDto dto)
    {
        var program = await _context.TrainingPrograms.FindAsync(dto.Id);
        if (program == null)
            throw new KeyNotFoundException($"Программа с Id {dto.Id} не найдена");

        program.Name = dto.Name;
        program.Description = dto.Description;
        program.CostRubles = dto.CostRubles;
        program.RequirementsEducation = dto.RequirementsEducation;
        program.RetrainingPeriodicity = dto.RetrainingPeriodicity;
        program.CustomRetrainingMonths = dto.CustomRetrainingMonths;
        program.DurationValue = dto.DurationValue;
        program.DurationUnit = dto.DurationUnit;
        program.Status = dto.Status;
        program.NotificationLeadTimeDays = dto.NotificationLeadTimeDays;
        program.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return (await GetByIdAsync(program.Id))!;
    }

    /// <summary>
    /// Архивирование программы (ТЗ 4.c.i)
    /// </summary>
    public async Task<TrainingProgramDto> ArchiveAsync(Guid id)
    {
        var program = await _context.TrainingPrograms.FindAsync(id);
        if (program == null)
            throw new KeyNotFoundException($"Программа с Id {id} не найдена");

        program.Status = ProgramStatus.Archive;
        program.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return (await GetByIdAsync(program.Id))!;
    }

    public async Task DeleteAsync(Guid id)
    {
        var program = await _context.TrainingPrograms.FindAsync(id);
        if (program == null)
            throw new KeyNotFoundException($"Программа с Id {id} не найдена");

        program.DeletedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
    }

    private static TrainingProgramDto MapToDto(DomainTrainingProgram p)
    {
        return new TrainingProgramDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            CostRubles = p.CostRubles,
            RequirementsEducation = p.RequirementsEducation,
            RetrainingPeriodicity = p.RetrainingPeriodicity,
            CustomRetrainingMonths = p.CustomRetrainingMonths,
            DurationValue = p.DurationValue,
            DurationUnit = p.DurationUnit,
            Status = p.Status,
            NotificationLeadTimeDays = p.NotificationLeadTimeDays,
            GroupsCount = p.TrainingGroups.Count,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}