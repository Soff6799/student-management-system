using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.Organization;
using StudentAccounting.WebApi.Services.Excel;
using StudentAccounting.WebApi.Services.Organization;

namespace StudentAccounting.WebApi.Controllers;

/// <summary>
/// Контроллер для управления данными организаций
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _service;
    private readonly IExcelExportService _excel;

    public OrganizationController(IOrganizationService service, IExcelExportService excel)
    {
        _service = service;
        _excel = excel;
    }

    /// <summary>
    /// Получить все организации
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<PagedResultDto<OrganizationDto>>> GetAll([FromQuery] OrganizationListParams p)
    {
        return Ok(await _service.GetPagedAsync(p));
    }

    /// <summary>
    /// Получить организацию по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<OrganizationDto>> GetById(Guid id)
    {
        var organization = await _service.GetByIdAsync(id);
        if (organization == null)
            return NotFound();

        return Ok(organization);
    }

    /// <summary>
    /// Создать новую организацию
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "AdministratorOnly")]
    public async Task<ActionResult<OrganizationDto>> Create([FromBody] OrganizationCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var organization = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = organization.Id }, organization);
    }

    /// <summary>
    /// Обновить организацию
    /// </summary>
    [HttpPut]
    [Authorize(Policy = "AdministatorOnly")]
    public async Task<ActionResult<OrganizationDto>> Update([FromBody] OrganizationUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var organization = await _service.UpdateAsync(dto);
            return Ok(organization);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Удалить организацию (мягкое удаление)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdministatorOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Выгрузка организаций в XLSX с учётом фильтров и поиска 
    /// </summary>
    [HttpGet("export")]
    [Authorize]
    public async Task<IActionResult> Export([FromQuery] OrganizationListParams p)
    {
        var data = await _service.GetFilteredAsync(p);
        var file = _excel.ExportOrganizations(data);
        return File(file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"organizations_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx");
    }
}