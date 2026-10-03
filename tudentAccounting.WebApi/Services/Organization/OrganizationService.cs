using Microsoft.EntityFrameworkCore;
using StudentAccounting.Dal.Contracts.interfaces;
using StudentAccounting.Domain;
using StudentAccounting.WebApi.DTOs.Organization;
using DomainOrganization = global::StudentAccounting.Domain.Organization;


namespace StudentAccounting.WebApi.Services.Organization;

public class OrganizationService : IOrganizationService
{
    private readonly IRepository<DomainOrganization> _repository;

    public OrganizationService(IRepository<DomainOrganization> repository)
    {
        _repository = repository;
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
}