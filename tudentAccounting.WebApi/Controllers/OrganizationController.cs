using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Organization;
using StudentAccounting.WebApi.Services.Organization;

namespace StudentAccounting.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationController : ControllerBase
{
    private readonly IOrganizationService _service;

    public OrganizationController(IOrganizationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить все организации
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrganizationDto>>> GetAll()
    {
        var organizations = await _service.GetAllAsync();
        return Ok(organizations);
    }

    /// <summary>
    /// Получить организацию по Id
    /// </summary>
    [HttpGet("{id:guid}")]
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
}