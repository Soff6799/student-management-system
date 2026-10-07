using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.Organization;

namespace StudentAccounting.WebApi.Services.Organization;

public interface IOrganizationService
{
    Task<PagedResultDto<OrganizationDto>> GetPagedAsync(OrganizationListParams p);
    Task<List<OrganizationDto>> GetFilteredAsync(OrganizationListParams p);
    Task<OrganizationDto?> GetByIdAsync(Guid id);
    Task<OrganizationDto> CreateAsync(OrganizationCreateDto dto);
    Task<OrganizationDto> UpdateAsync(OrganizationUpdateDto dto);
    Task DeleteAsync(Guid id);
}