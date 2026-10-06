using Microsoft.EntityFrameworkCore;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.DTOs.Contract;
using DomainContract = global::StudentAccounting.Domain.Contract;

namespace StudentAccounting.WebApi.Services.Contract;

public class ContractService : IContractService
{
    private readonly AppDbContext _context;

    public ContractService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ContractDto>> GetAllAsync()
    {
        var contracts = await _context.Contracts
            .Include(c => c.TrainingGroup)
            .ToListAsync();
        return contracts.Select(MapToDto);
    }

    public async Task<ContractDto?> GetByIdAsync(Guid id)
    {
        var contract = await _context.Contracts
            .Include(c => c.TrainingGroup)
            .FirstOrDefaultAsync(c => c.Id == id);
        return contract == null ? null : MapToDto(contract);
    }

    public async Task<IEnumerable<ContractDto>> GetByTrainingGroupIdAsync(Guid trainingGroupId)
    {
        var contracts = await _context.Contracts
            .Include(c => c.TrainingGroup)
            .Where(c => c.TrainingGroupId == trainingGroupId)
            .ToListAsync();
        return contracts.Select(MapToDto);
    }

    public async Task<ContractDto> CreateAsync(ContractCreateDto dto)
    {
        var group = await _context.TrainingGroups.FindAsync(dto.TrainingGroupId);
        if (group == null)
            throw new KeyNotFoundException($"Группа с Id {dto.TrainingGroupId} не найдена");

        var contract = new DomainContract
        {
            ContractNumber = await GenerateContractNumberAsync(),
            GenerationDate = DateOnly.FromDateTime(DateTime.UtcNow),
            FilePath = dto.FilePath,
            TrainingGroupId = dto.TrainingGroupId
        };

        await _context.Contracts.AddAsync(contract);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(contract.Id))!;
    }

    public async Task<ContractDto> UpdateAsync(ContractUpdateDto dto)
    {
        var contract = await _context.Contracts.FindAsync(dto.Id);
        if (contract == null)
            throw new KeyNotFoundException($"Договор с Id {dto.Id} не найден");

        var group = await _context.TrainingGroups.FindAsync(dto.TrainingGroupId);
        if (group == null)
            throw new KeyNotFoundException($"Группа с Id {dto.TrainingGroupId} не найдена");

        contract.FilePath = dto.FilePath;
        contract.TrainingGroupId = dto.TrainingGroupId;
        contract.UpdatedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync();
        return (await GetByIdAsync(contract.Id))!;
    }

    public async Task DeleteAsync(Guid id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null)
            throw new KeyNotFoundException($"Договор с Id {id} не найден");

        contract.DeletedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// генерация номера договора по шаблону Д-YYYY-NNNN
    /// </summary>
    private async Task<string> GenerateContractNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"Д-{year}-";

        var existing = await _context.Contracts
            .IgnoreQueryFilters()
            .Where(c => c.ContractNumber.StartsWith(prefix))
            .Select(c => c.ContractNumber)
            .ToListAsync();

        var maxNumber = existing
            .Select(n => n.Split('-').Last())
            .Select(s => int.TryParse(s, out var v) ? v : 0)
            .DefaultIfEmpty(0)
            .Max();

        return $"{prefix}{maxNumber + 1:D4}";
    }

    private static ContractDto MapToDto(DomainContract c)
    {
        return new ContractDto
        {
            Id = c.Id,
            ContractNumber = c.ContractNumber,
            GenerationDate = c.GenerationDate,
            FilePath = c.FilePath,
            TrainingGroupId = c.TrainingGroupId,
            TrainingGroupName = c.TrainingGroup?.GroupName,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }
}