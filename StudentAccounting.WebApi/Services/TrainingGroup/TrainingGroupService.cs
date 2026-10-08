using Microsoft.EntityFrameworkCore;
using StudentAccounting.Domain;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.TrainingGroup;
using StudentAccounting.WebApi.Services.Common;
using static StudentAccounting.Domain.Enums;
using DomainTrainingGroup = global::StudentAccounting.Domain.TrainingGroup;

namespace StudentAccounting.WebApi.Services.TrainingGroup;

public class TrainingGroupService : ITrainingGroupService
{
    private readonly AppDbContext _context;
    private readonly AuditService _auditService;

    public TrainingGroupService(AppDbContext context, AuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<PagedResultDto<TrainingGroupDto>> GetPagedAsync(TrainingGroupListParams p)
    {
        var query = ApplySorting(ApplyFilters(WithIncludes(), p), p);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((p.Page - 1) * p.PageSize)
            .Take(p.PageSize)
            .ToListAsync();

        return new PagedResultDto<TrainingGroupDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = p.Page,
            PageSize = p.PageSize
        };
    }

    public async Task<List<TrainingGroupDto>> GetFilteredAsync(TrainingGroupListParams p)
    {
        var items = await ApplySorting(ApplyFilters(WithIncludes(), p), p)
            .ToListAsync();
        return items.Select(MapToDto).ToList();
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
        _auditService.SetAuditFields(group);
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
        _auditService.SetAuditFields(group, isUpdate: true);
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

    /// <summary>
    /// Базовый запрос с загрузкой связанных данных
    /// </summary>
    private IQueryable<DomainTrainingGroup> WithIncludes()
    {
        return _context.TrainingGroups
            .Include(g => g.TrainingProgram)
            .Include(g => g.StudentTrainings);
    }

    /// <summary>
    /// Применение поиска и фильтров списка групп
    /// </summary>
    private static IQueryable<DomainTrainingGroup> ApplyFilters(IQueryable<DomainTrainingGroup> query, TrainingGroupListParams p)
    {
        if (p.TrainingProgramId.HasValue)
            query = query.Where(g => g.TrainingProgramId == p.TrainingProgramId.Value);

        if (p.Status.HasValue)
            query = query.Where(g => g.Status == p.Status.Value);

        if (!string.IsNullOrWhiteSpace(p.Search))
        {
            var s = p.Search.Trim();
            query = query.Where(g => g.GroupName.Contains(s) || g.TrainingProgram.Name.Contains(s));
        }
        return query;
    }

    /// <summary>
    /// Применение сортировки списка групп по столбцам
    /// </summary>
    private static IQueryable<DomainTrainingGroup> ApplySorting(IQueryable<DomainTrainingGroup> query, TrainingGroupListParams p)
    {
        return p.SortBy.ToLower() switch
        {
            "groupname" => p.Descending ? query.OrderByDescending(g => g.GroupName) : query.OrderBy(g => g.GroupName),
            "startdate" => p.Descending ? query.OrderByDescending(g => g.StartDate) : query.OrderBy(g => g.StartDate),
            "status" => p.Descending ? query.OrderByDescending(g => g.Status) : query.OrderBy(g => g.Status),
            _ => p.Descending ? query.OrderByDescending(g => g.CreatedAt) : query.OrderBy(g => g.CreatedAt)
        };
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
            StudentsCount = g.StudentTrainings.Count(t => t.Status != TrainingStatus.DroppedOut),
            CreatedAt = g.CreatedAt,
            UpdatedAt = g.UpdatedAt == default ? null : g.UpdatedAt
        };
    }
}