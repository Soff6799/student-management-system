using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.StudentTraining;
using StudentAccounting.WebApi.Services.StudentTraining;

namespace StudentAccounting.WebApi.Controllers;

/// <summary>
/// Контроллер для управления записями о прохождении обучения
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StudentTrainingController : ControllerBase
{
    private readonly IStudentTrainingService _service;

    public StudentTrainingController(IStudentTrainingService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить все записи об обучении
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentTrainingDto>>> GetAll()
    {
        var trainings = await _service.GetAllAsync();
        return Ok(trainings);
    }

    /// <summary>
    /// Получить записи об обучении по Id сотрудника
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<IEnumerable<StudentTrainingDto>>> GetByEmployeeId(Guid employeeId)
    {
        var trainings = await _service.GetByEmployeeIdAsync(employeeId);
        return Ok(trainings);
    }

    /// <summary>
    /// Получить записи об обучении по Id группы
    /// </summary>
    [HttpGet("group/{trainingGroupId:guid}")]
    public async Task<ActionResult<IEnumerable<StudentTrainingDto>>> GetByTrainingGroupId(Guid trainingGroupId)
    {
        var trainings = await _service.GetByTrainingGroupIdAsync(trainingGroupId);
        return Ok(trainings);
    }

    /// <summary>
    /// Получить запись об обучении по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StudentTrainingDto>> GetById(Guid id)
    {
        var training = await _service.GetByIdAsync(id);
        if (training == null)
            return NotFound();
        return Ok(training);
    }

    /// <summary>
    /// Создать новую запись об обучении
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<StudentTrainingDto>> Create([FromBody] StudentTrainingCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var training = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = training.Id }, training);
    }

    /// <summary>
    /// Обновить запись об обучении (автоматический расчёт даты следующего обучения при статусе "Прошёл обучение")
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<StudentTrainingDto>> Update([FromBody] StudentTrainingUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var training = await _service.UpdateAsync(dto);
            return Ok(training);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Удалить запись об обучении (мягкое удаление)
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