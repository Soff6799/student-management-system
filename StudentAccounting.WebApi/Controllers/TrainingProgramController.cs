using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.TrainingProgram;
using StudentAccounting.WebApi.Services.TrainingProgram;

namespace StudentAccounting.WebApi.Controllers;

/// <summary>
/// Контроллер для управления программами обучения
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TrainingProgramController : ControllerBase
{
    private readonly ITrainingProgramService _service;

    public TrainingProgramController(ITrainingProgramService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить программы обучения с пагинацией
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResultDto<TrainingProgramDto>>> GetPaged([FromQuery] TrainingProgramListParams p)
    {
        return Ok(await _service.GetPagedAsync(p));
    }

    /// <summary>
    /// Получить программу по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TrainingProgramDto>> GetById(Guid id)
    {
        var program = await _service.GetByIdAsync(id);
        return program == null ? NotFound() : Ok(program);
    }

    /// <summary>
    /// Создать программу
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TrainingProgramDto>> Create([FromBody] TrainingProgramCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var program = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = program.Id }, program);
    }

    /// <summary>
    /// Обновить программу
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<TrainingProgramDto>> Update([FromBody] TrainingProgramUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            return Ok(await _service.UpdateAsync(dto));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Архивировать программу
    /// </summary>
    [HttpPost("{id:guid}/archive")]
    public async Task<ActionResult<TrainingProgramDto>> Archive(Guid id)
    {
        try
        {
            return Ok(await _service.ArchiveAsync(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Удалить программу (мягкое удаление)
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