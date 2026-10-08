using Microsoft.EntityFrameworkCore;
using StudentAccounting.Domain;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.DTOs.Contract;
using StudentAccounting.WebApi.Services.Common;
using static StudentAccounting.Domain.Enums;
using DomainContract = global::StudentAccounting.Domain.Contract;
using DomainTrainingGroup = global::StudentAccounting.Domain.TrainingGroup;

namespace StudentAccounting.WebApi.Services.Contract;

public class ContractService : IContractService
{
    private readonly AppDbContext _context;
    private readonly IContractDocumentGenerator _generator;
    private readonly StorageOptions _storage;
    private readonly AuditService _auditService;

    public ContractService(AppDbContext context, 
        IContractDocumentGenerator generator, StorageOptions storage,
        AuditService auditService)
    {
        _context = context;
        _generator = generator;
        _storage = storage;
        _auditService = auditService;
    }

    public async Task<IEnumerable<ContractDto>> GetAllAsync()
    {
        var contracts = await WithIncludes().ToListAsync();
        return contracts.Select(MapToDto);
    }

    public async Task<ContractDto?> GetByIdAsync(Guid id)
    {
        var contract = await WithIncludes().FirstOrDefaultAsync(c => c.Id == id);
        return contract == null ? null : MapToDto(contract);
    }

    public async Task<IEnumerable<ContractDto>> GetByTrainingGroupIdAsync(Guid trainingGroupId)
    {
        var contracts = await WithIncludes()
            .Where(c => c.TrainingGroupId == trainingGroupId)
            .ToListAsync();
        return contracts.Select(MapToDto);
    }

    public async Task<ContractDto> CreateAsync(ContractCreateDto dto)
    {
        var group = await LoadGroupAsync(dto.TrainingGroupId);
        var number = await GenerateContractNumberAsync();
        var data = BuildDocumentData(group, number, DateOnly.FromDateTime(DateTime.UtcNow));

        var contract = new DomainContract
        {
            ContractNumber = number,
            GenerationDate = data.GenerationDate,
            FilePath = number,
            TrainingGroupId = group.Id
        };
        _auditService.SetAuditFields(contract);
        await SaveDocumentsAsync(data, contract.FilePath);

        await _context.Contracts.AddAsync(contract);
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(contract.Id))!;
    }

    /// <summary>
    /// Пересоздать файлы договора из актуальных данных группы (номер и дата сохраняются)
    /// </summary>
    public async Task<ContractDto> RegenerateAsync(Guid id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null)
            throw new KeyNotFoundException($"Договор с Id {id} не найден");

        var group = await LoadGroupAsync(contract.TrainingGroupId);
        var data = BuildDocumentData(group, contract.ContractNumber, contract.GenerationDate);
        await SaveDocumentsAsync(data, contract.FilePath);

        contract.UpdatedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
        return (await GetByIdAsync(contract.Id))!;
    }

    public async Task<(byte[] Content, string ContentType, string FileName)> GetDocumentAsync(Guid id, string format)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null)
            throw new KeyNotFoundException($"Договор с Id {id} не найден");

        var ext = format.ToLower();
        if (ext is not ("docx" or "pdf"))
            throw new ArgumentException("Формат должен быть docx или pdf");

        var path = Path.Combine(_storage.ContractsRoot, contract.FilePath + "." + ext);
        if (!File.Exists(path))
            throw new FileNotFoundException("Файл договора не найден в хранилище. Выполните POST /api/Contract/{id}/regenerate");

        var content = await File.ReadAllBytesAsync(path);
        var contentType = ext == "pdf"
            ? "application/pdf"
            : "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        return (content, contentType, $"{contract.ContractNumber}.{ext}");
    }

    public async Task DeleteAsync(Guid id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null)
            throw new KeyNotFoundException($"Договор с Id {id} не найден");

        contract.DeletedAt = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync();
    }

    private IQueryable<DomainContract> WithIncludes()
    {
        return _context.Contracts
            .Include(c => c.TrainingGroup).ThenInclude(g => g.TrainingProgram)
            .Include(c => c.TrainingGroup).ThenInclude(g => g.StudentTrainings).ThenInclude(t => t.Employee).ThenInclude(e => e.Organization);
    }

    private async Task<DomainTrainingGroup> LoadGroupAsync(Guid groupId)
    {
        var group = await _context.TrainingGroups
            .Include(g => g.TrainingProgram)
            .Include(g => g.StudentTrainings).ThenInclude(t => t.Employee).ThenInclude(e => e.Organization)
            .FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null)
            throw new KeyNotFoundException($"Группа с Id {groupId} не найдена");
        return group;
    }

    /// <summary>
    /// Сборка данных договора
    /// Заказчик — организация, если все сотрудники с организацией из одной;
    /// если сотрудники без организации — физлицо (ФИО).
    /// Смешанные организации в одной группе — ошибка (договор юридически некорректен).
    /// </summary>
    private static ContractDocumentData BuildDocumentData(DomainTrainingGroup group, string number, DateOnly date)
    {
        var students = group.StudentTrainings
            .Where(t => t.Status != TrainingStatus.DroppedOut)
            .Select(t => t.Employee)
            .Where(e => e != null)
            .ToList();

        if (students.Count == 0)
            throw new InvalidOperationException("Невозможно сформировать договор: в группе нет зачисленных обучающихся");

        var orgIds = students.Where(e => e.OrganizationId != null).Select(e => e.OrganizationId).Distinct().ToList();
        if (orgIds.Count > 1)
            throw new InvalidOperationException(
                "В группе обучающиеся из разных организаций. Для формирования договора сформируйте отдельные группы по заказчикам");

        var org = students.Select(e => e.Organization).FirstOrDefault(o => o != null);
        var program = group.TrainingProgram;

        var data = new ContractDocumentData
        {
            ContractNumber = number,
            GenerationDate = date,
            GroupName = group.GroupName,
            ProgramName = program.Name,
            DurationText = program.DurationValue.HasValue && program.DurationUnit.HasValue
                ? $"{program.DurationValue} {(program.DurationUnit == DurationUnit.Hours ? "часов" : "дней")}"
                : null,
            CostPerStudent = program.CostRubles,
            Students = students
                .Select((e, i) => new ContractStudentRow
                {
                    Number = i + 1,
                    FullName = $"{e.LastName} {e.FirstName} {e.Patronymic}".Trim(),
                    Post = e.Post
                })
                .ToList()
        };

        if (org != null)
        {
            data.CustomerName = org.FullName;
            data.CustomerInn = org.INN;
            data.CustomerKpp = org.KPP;
            data.CustomerOgrn = org.OGRN;
            data.CustomerAddress = org.LegalAddress;
            data.CustomerPhone = org.Phone;
            data.CustomerEmail = org.Email;
        }
        else
        {
            var individual = students[0];
            data.IsIndividual = true;
            data.CustomerName = $"{individual.LastName} {individual.FirstName} {individual.Patronymic}".Trim();
            data.CustomerPhone = individual.Phone;
            data.CustomerEmail = individual.Email;
        }

        return data;
    }

    private async Task SaveDocumentsAsync(ContractDocumentData data, string fileBase)
    {
        Directory.CreateDirectory(_storage.ContractsRoot);
        var baseFull = Path.Combine(_storage.ContractsRoot, fileBase);
        await File.WriteAllBytesAsync(baseFull + ".docx", _generator.GenerateDocx(data));
        await File.WriteAllBytesAsync(baseFull + ".pdf", _generator.GeneratePdf(data));
    }

    /// <summary>
    /// Генерация номера договора по шаблону Д-YYYY-NNNN (ТЗ 4.g.iii)
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
        var group = c.TrainingGroup;
        var studentsCount = group?.StudentTrainings?.Count(t => t.Status != TrainingStatus.DroppedOut) ?? 0;
        var cost = group?.TrainingProgram?.CostRubles ?? 0m;

        return new ContractDto
        {
            Id = c.Id,
            ContractNumber = c.ContractNumber,
            GenerationDate = c.GenerationDate,
            FilePath = c.FilePath,
            TrainingGroupId = c.TrainingGroupId,
            TrainingGroupName = group?.GroupName,
            CustomerName = ResolveCustomerName(group),
            StudentsCount = studentsCount,
            TotalCost = cost * studentsCount,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt == default ? null : c.UpdatedAt
        };
    }

    private static string? ResolveCustomerName(DomainTrainingGroup? group)
    {
        var employees = group?.StudentTrainings?
            .Where(t => t.Status != TrainingStatus.DroppedOut)
            .Select(t => t.Employee)
            .Where(e => e != null)
            .ToList();
        if (employees == null || employees.Count == 0)
            return null;

        var org = employees.Select(e => e.Organization).FirstOrDefault(o => o != null);
        if (org != null)
            return org.FullName;

        var first = employees[0];
        return $"{first.LastName} {first.FirstName} {first.Patronymic}".Trim();
    }
}