using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Education;
using StudentAccounting.WebApi.Services.Education;

namespace StudentAccounting.WebApi.Controllers;

/// <summary>
/// Контроллер для управления данными об образовании сотрудников
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EducationController : ControllerBase
{
    private readonly IEducationService _service;

    public EducationController(IEducationService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить все записи об образовании
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EducationDto>>> GetAll()
    {
        var educations = await _service.GetAllAsync();
        return Ok(educations);
    }

    /// <summary>
    /// Получить записи об образовании по Id сотрудника
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<IEnumerable<EducationDto>>> GetByEmployeeId(Guid employeeId)
    {
        var educations = await _service.GetByEmployeeIdAsync(employeeId);
        return Ok(educations);
    }

    /// <summary>
    /// Получить запись об образовании по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EducationDto>> GetById(Guid id)
    {
        var education = await _service.GetByIdAsync(id);
        if (education == null)
            return NotFound();
        return Ok(education);
    }

    /// <summary>
    /// Создать новую запись об образовании
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EducationDto>> Create([FromBody] EducationCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var education = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = education.Id }, education);
    }

    /// <summary>
    /// Обновить запись об образовании
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<EducationDto>> Update([FromBody] EducationUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var education = await _service.UpdateAsync(dto);
            return Ok(education);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Удалить запись об образовании (мягкое удаление)
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