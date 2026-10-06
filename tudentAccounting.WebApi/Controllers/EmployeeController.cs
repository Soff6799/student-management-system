using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Employee;
using StudentAccounting.WebApi.Services.Employee;

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

    public EmployeeController(IEmployeeService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить всех сотрудников
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        var employees = await _service.GetAllAsync();
        return Ok(employees);
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
}