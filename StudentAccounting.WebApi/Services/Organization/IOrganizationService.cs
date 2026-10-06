using StudentAccounting.WebApi.DTOs.Organization;

namespace StudentAccounting.WebApi.Services.Organization;

public interface IOrganizationService
{
    Task<IEnumerable<OrganizationDto>> GetAllAsync();
    Task<OrganizationDto?> GetByIdAsync(Guid id);
    Task<OrganizationDto> CreateAsync(OrganizationCreateDto dto);
    Task<OrganizationDto> UpdateAsync(OrganizationUpdateDto dto);
    Task DeleteAsync(Guid id);
}