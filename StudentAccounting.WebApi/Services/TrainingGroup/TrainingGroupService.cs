using Microsoft.EntityFrameworkCore;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.DTOs.TrainingGroup;
using static StudentAccounting.Domain.Enums;
using DomainTrainingGroup = global::StudentAccounting.Domain.TrainingGroup;

namespace StudentAccounting.WebApi.Services.TrainingGroup;

public class TrainingGroupService : ITrainingGroupService
{
    private readonly AppDbContext _context;

    public TrainingGroupService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TrainingGroupDto>> GetAllAsync()
    {
        var groups = await _context.TrainingGroups
            .Include(g => g.TrainingProgram)
            .Include(g => g.StudentTrainings)
            .ToListAsync();
        return groups.Select(MapToDto);
    }
    public async Task<TrainingGroupDto?> GetByIdAsync(Guid id)
    {
        var group = await _context.TrainingGroups
            .Include(g => g.TrainingProgram)
            .Include(g => g.StudentTrainings)
            .FirstOrDefaultAsync(g => g.Id == id);
        return group == null ? null : MapToDto(group);
    }

    public async Task<IEnumerable<TrainingGroupDto>> GetByProgramIdAsync(Guid trainingProgramId)
    {
        var groups = await _context.TrainingGroups
            .Include(g => g.TrainingProgram)
            .Include(g => g.StudentTrainings)
            .Where(g => g.TrainingProgramId == trainingProgramId)
            .ToListAsync();
        return groups.Select(MapToDto);
    }

    public async Task<TrainingGroupDto> CreateAsync(TrainingGroupCreateDto dto)
    {
        var program = await _context.TrainingPrograms.FindAsync(dto.TrainingProgramId);
        if (program == null)
            throw new KeyNotFoundException($"Программа с Id {dto.TrainingProgramId} не найдена");
        if (program.Status == ProgramStatus.Archive)
            throw new InvalidOperationException("Нельзя создать группу для программы, находящейся в архиве");

        var group = new DomainTrainingGroup
        {
            GroupName = dto.GroupName,
            TrainingProgramId = dto.TrainingProgramId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            Note = dto.Note
        };

        await _context.TrainingGroups.AddAsync(group);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(group.Id))!;
    }

    public async Task<TrainingGroupDto> UpdateAsync(TrainingGroupUpdateDto dto)
    {
        var group = await _context.TrainingGroups.FindAsync(dto.Id);
        if (group == null)
            throw new KeyNotFoundException($"Группа с Id {dto.Id} не найдена");

        if (group.TrainingProgramId != dto.TrainingProgramId)
        {
            var program = await _context.TrainingPrograms.FindAsync(dto.TrainingProgramId);
            if (program == null)
                throw new KeyNotFoundException($"Программа с Id {dto.TrainingProgramId} не найдена");
            if (program.Status == ProgramStatus.Archive)
                throw new InvalidOperationException("Нельзя привязать группу к программе, находящейся в архиве");
        }

        group.GroupName = dto.GroupName;
        group.TrainingProgramId = dto.TrainingProgramId;
        group.StartDate = dto.StartDate;
        group.EndDate = dto.EndDate;
        group.Status = dto.Status;
        group.Note = dto.Note;
        group.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return (await GetByIdAsync(group.Id))!;
    }

    public async Task DeleteAsync(Guid id)
    {
        var group = await _context.TrainingGroups.FindAsync(id);
        if (group == null)
            throw new KeyNotFoundException($"Группа с Id {id} не найдена");

        group.DeletedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
    }

    private static TrainingGroupDto MapToDto(DomainTrainingGroup g)
    {
        return new TrainingGroupDto
        {
            Id = g.Id,
            GroupName = g.GroupName,
            TrainingProgramId = g.TrainingProgramId,
            TrainingProgramName = g.TrainingProgram?.Name,
            StartDate = g.StartDate,
            EndDate = g.EndDate,
            Status = g.Status,
            Note = g.Note,
            StudentsCount = g.StudentTrainings.Count,
            CreatedAt = g.CreatedAt,
            UpdatedAt = g.UpdatedAt
        };
    }
}