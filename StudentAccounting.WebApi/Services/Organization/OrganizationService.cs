using Microsoft.EntityFrameworkCore;
using StudentAccounting.Dal.Contracts.interfaces;
using StudentAccounting.Domain;
using StudentAccounting.Infrastructure.Data;
using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.Organization;
using StudentAccounting.WebApi.Services.Common;
using DomainOrganization = global::StudentAccounting.Domain.Organization;


namespace StudentAccounting.WebApi.Services.Organization;

public class OrganizationService : IOrganizationService
{
    private readonly IRepository<DomainOrganization> _repository;
    private readonly AppDbContext _context;
    private readonly AuditService _auditService;

    public OrganizationService(IRepository<DomainOrganization> repository, 
        AppDbContext context,
        AuditService auditService)
    {
        _repository = repository;
        _context = context;
        _auditService = auditService;
    }

    public async Task<IEnumerable<OrganizationDto>> GetAllAsync()
    {
        var organizations = await _repository.GetAllAsync();
        return organizations.Select(MapToDto);
    }

    public async Task<OrganizationDto?> GetByIdAsync(Guid id)
    {
        var organization = await _repository.GetByIdAsync(id);
        return organization == null ? null : MapToDto(organization);
    }

    public async Task<OrganizationDto> CreateAsync(OrganizationCreateDto dto)
    {
        var organization = new DomainOrganization
        {
            FullName = dto.FullName,
            ShortName = dto.ShortName,
            INN = dto.INN,
            KPP = dto.KPP,
            OGRN = dto.OGRN,
            LegalAddress = dto.LegalAddress,
            ActualAddress = dto.ActualAddress,
            Phone = dto.Phone,
            Email = dto.Email,
            ContactPersonFullName = dto.ContactPersonFullName,
            ContactPersonPosition = dto.ContactPersonPosition,
            Note = dto.Note
        };
        _auditService.SetAuditFields(organization);
        await _repository.AddAsync(organization);
        await _repository.SaveChangesAsync();

        return MapToDto(organization);
    }

    public async Task<OrganizationDto> UpdateAsync(OrganizationUpdateDto dto)
    {
        var organization = await _repository.GetByIdAsync(dto.Id);
        if (organization == null)
            throw new KeyNotFoundException($"Организация с Id {dto.Id} не найдена");

        organization.FullName = dto.FullName;
        organization.ShortName = dto.ShortName;
        organization.INN = dto.INN;
        organization.KPP = dto.KPP;
        organization.OGRN = dto.OGRN;
        organization.LegalAddress = dto.LegalAddress;
        organization.ActualAddress = dto.ActualAddress;
        organization.Phone = dto.Phone;
        organization.Email = dto.Email;
        organization.ContactPersonFullName = dto.ContactPersonFullName;
        organization.ContactPersonPosition = dto.ContactPersonPosition;
        organization.Note = dto.Note;
        organization.UpdatedAt = DateTimeOffset.UtcNow;
        _auditService.SetAuditFields(organization, isUpdate: true);
        await _repository.UpdateAsync(organization);
        await _repository.SaveChangesAsync();

        return MapToDto(organization);
    }

    public async Task DeleteAsync(Guid id)
    {
        var organization = await _repository.GetByIdAsync(id);
        if (organization == null)
            throw new KeyNotFoundException($"Организация с Id {id} не найдена");

        organization.DeletedAt = DateTimeOffset.UtcNow;
        await _repository.UpdateAsync(organization);
        await _repository.SaveChangesAsync();
    }

    private static OrganizationDto MapToDto(DomainOrganization org)
    {
        return new OrganizationDto
        {
            Id = org.Id,
            FullName = org.FullName,
            ShortName = org.ShortName,
            INN = org.INN,
            KPP = org.KPP,
            OGRN = org.OGRN,
            LegalAddress = org.LegalAddress,
            ActualAddress = org.ActualAddress,
            Phone = org.Phone,
            Email = org.Email,
            ContactPersonFullName = org.ContactPersonFullName,
            ContactPersonPosition = org.ContactPersonPosition,
            Note = org.Note,
            CreatedAt = org.CreatedAt,
            UpdatedAt = org.UpdatedAt
        };
    }

    public async Task<PagedResultDto<OrganizationDto>> GetPagedAsync(OrganizationListParams p)
    {
        var query = ApplySorting(ApplyFilters(_context.Organizations, p), p);
        var totalCount = await query.CountAsync();
        var items = await query.Skip((p.Page - 1) * p.PageSize).Take(p.PageSize).ToListAsync();

        return new PagedResultDto<OrganizationDto>
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = p.Page,
            PageSize = p.PageSize
        };
    }

    public async Task<List<OrganizationDto>> GetFilteredAsync(OrganizationListParams p)
    {
        var items = await ApplySorting(ApplyFilters(_context.Organizations, p), p).ToListAsync();
        return items.Select(MapToDto).ToList();
    }

    private static IQueryable<DomainOrganization> ApplyFilters(IQueryable<DomainOrganization> query, OrganizationListParams p)
    {
        if (!string.IsNullOrWhiteSpace(p.Search))
        {
            var s = p.Search.Trim();
            query = query.Where(o => o.FullName.Contains(s) || o.ShortName.Contains(s) || o.INN.Contains(s));
        }
        return query;
    }

    private static IQueryable<DomainOrganization> ApplySorting(IQueryable<DomainOrganization> query, OrganizationListParams p)
    {
        return p.SortBy.ToLower() switch
        {
            "fullname" => p.Descending ? query.OrderByDescending(o => o.FullName) : query.OrderBy(o => o.FullName),
            "shortname" => p.Descending ? query.OrderByDescending(o => o.ShortName) : query.OrderBy(o => o.ShortName),
            "inn" => p.Descending ? query.OrderByDescending(o => o.INN) : query.OrderBy(o => o.INN),
            _ => p.Descending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt)
        };
    }
}