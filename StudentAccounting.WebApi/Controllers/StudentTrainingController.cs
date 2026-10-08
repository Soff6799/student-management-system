using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.StudentTraining;
using StudentAccounting.WebApi.Services.Excel;
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
    private readonly IExcelExportService _excel;

    public StudentTrainingController(IStudentTrainingService service, IExcelExportService excel)
    {
        _service = service;
        _excel = excel;
    }

    /// <summary>
    /// Получить все записи об обучении
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentTrainingDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    /// <summary>
    /// Получить историю обучения сотрудника
    /// </summary>
    [HttpGet("employee/{employeeId:guid}")]
    public async Task<ActionResult<IEnumerable<StudentTrainingDto>>> GetByEmployeeId(Guid employeeId)
    {
        return Ok(await _service.GetByEmployeeIdAsync(employeeId));
    }

    /// <summary>
    /// Получить обучающихся группы с поиском, фильтром, сортировкой и пагинацией
    /// </summary>
    [HttpGet("group/{trainingGroupId:guid}/trainings")]
    public async Task<ActionResult<PagedResultDto<StudentTrainingDto>>> GetByTrainingGroupId(
        Guid trainingGroupId, [FromQuery] GroupStudentListParams p)
    {
        return Ok(await _service.GetGroupStudentsPagedAsync(trainingGroupId, p));
    }

    /// <summary>
    /// Выгрузка обучающихся группы в XLSX с учётом фильтров и поиска
    /// </summary>
    [HttpGet("group/{trainingGroupId:guid}/export")]
    public async Task<IActionResult> ExportGroupStudents(Guid trainingGroupId, [FromQuery] GroupStudentListParams p)
    {
        var data = await _service.GetGroupStudentsFilteredAsync(trainingGroupId, p);
        var file = _excel.ExportGroupStudents(data);
        return File(file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"group_students_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx");
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
        try
        {
            var training = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = training.Id }, training);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Предварительная проверка перед зачислением
    /// </summary>
    [HttpPost("check")]
    public async Task<ActionResult<CheckEnrollmentResultDto>> Check([FromBody] StudentTrainingCreateDto dto)
    {
        return Ok(await _service.CheckEnrollmentAsync(dto));
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