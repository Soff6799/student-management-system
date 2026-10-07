using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.TrainingGroup;
using StudentAccounting.WebApi.Services.Excel;
using StudentAccounting.WebApi.Services.TrainingGroup;

namespace StudentAccounting.WebApi.Controllers;

/// <summary>
/// Контроллер для управления группами обучения
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TrainingGroupController : ControllerBase
{
    private readonly ITrainingGroupService _service;
    private readonly IExcelExportService _excel;

    public TrainingGroupController(ITrainingGroupService service, IExcelExportService excel)
    {
        _service = service;
        _excel = excel;
    }

    /// <summary>
    /// Получить группы с поиском, фильтрами, сортировкой и пагинацией
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<TrainingGroupDto>>> GetAll([FromQuery] TrainingGroupListParams p)
    {
        return Ok(await _service.GetPagedAsync(p));
    }

    /// <summary>
    /// Выгрузка групп в XLSX с учётом фильтров и поиска
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] TrainingGroupListParams p)
    {
        var data = await _service.GetFilteredAsync(p);
        var file = _excel.ExportTrainingGroups(data);
        return File(file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"training_groups_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx");
    }

    /// <summary>
    /// Получить группу по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TrainingGroupDto>> GetById(Guid id)
    {
        var group = await _service.GetByIdAsync(id);
        return group == null ? NotFound() : Ok(group);
    }

    /// <summary>
    /// Получить группы программы
    /// </summary>
    [HttpGet("program/{trainingProgramId:guid}")]
    public async Task<ActionResult<IEnumerable<TrainingGroupDto>>> GetByProgramId(Guid trainingProgramId)
    {
        return Ok(await _service.GetByProgramIdAsync(trainingProgramId));
    }

    /// <summary>
    /// Создать группу
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TrainingGroupDto>> Create([FromBody] TrainingGroupCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var group = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Обновить группу
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<TrainingGroupDto>> Update([FromBody] TrainingGroupUpdateDto dto)
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
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Удалить группу (мягкое удаление)
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