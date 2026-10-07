using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Common;
using StudentAccounting.WebApi.DTOs.Employee;
using StudentAccounting.WebApi.Services.Employee;
using StudentAccounting.WebApi.Services.Excel;

namespace StudentAccounting.WebApi.Controllers;

/// <summary>
/// Контроллер для управления данными сотрудников
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _service;
    private readonly IExcelExportService _excel;

    public EmployeeController(IEmployeeService service, IExcelExportService excel)
    {
        _service = service;
        _excel = excel;
    }

    /// <summary>
    /// Получить сотрудников с поиском, фильтрами, сортировкой и пагинацией
    /// уволенные скрыты по умолчанию includeDismissed=true покажет их
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<EmployeeDto>>> GetAll([FromQuery] EmployeeListParams p)
    {
        return Ok(await _service.GetPagedAsync(p));
    }

    /// <summary>
    /// Получить сотрудника по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> GetById(Guid id)
    {
        var employee = await _service.GetByIdAsync(id);
        if (employee == null)
            return NotFound();
        return Ok(employee);
    }

    /// <summary>
    /// Создать нового сотрудника
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] EmployeeCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var employee = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    /// <summary>
    /// Обновить сотрудника
    /// </summary>
    [HttpPut]
    public async Task<ActionResult<EmployeeDto>> Update([FromBody] EmployeeUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var employee = await _service.UpdateAsync(dto);
            return Ok(employee);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Удалить сотрудника (мягкое удаление)
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

    /// <summary>
    /// Выгрузка сотрудников в XLSX с учётом фильтров и поиска
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] EmployeeListParams p)
    {
        var data = await _service.GetFilteredAsync(p);
        var file = _excel.ExportEmployees(data);
        return File(file,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"employees_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx");
    }
}