using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentAccounting.WebApi.DTOs.Contract;
using StudentAccounting.WebApi.Services.Contract;

namespace StudentAccounting.WebApi.Controllers;

/// <summary>
/// Контроллер для управления договорами на обучение
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContractController : ControllerBase
{
    private readonly IContractService _service;

    public ContractController(IContractService service)
    {
        _service = service;
    }

    /// <summary>
    /// Реестр договоров
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContractDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    /// <summary>
    /// Получить договор по Id
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContractDto>> GetById(Guid id)
    {
        var contract = await _service.GetByIdAsync(id);
        return contract == null ? NotFound() : Ok(contract);
    }

    /// <summary>
    /// Получить договоры группы
    /// </summary>
    [HttpGet("group/{trainingGroupId:guid}")]
    public async Task<ActionResult<IEnumerable<ContractDto>>> GetByTrainingGroupId(Guid trainingGroupId)
    {
        return Ok(await _service.GetByTrainingGroupIdAsync(trainingGroupId));
    }

    /// <summary>
    /// Сформировать договор: номер, дата и файлы (.docx/.pdf) создаются автоматически
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ContractDto>> Create([FromBody] ContractCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var contract = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Пересоздать файлы договора из актуальных данных группы
    /// </summary>
    [HttpPost("{id:guid}/regenerate")]
    public async Task<ActionResult<ContractDto>> Regenerate(Guid id)
    {
        try
        {
            return Ok(await _service.RegenerateAsync(id));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Скачать договор в формате docx или pdf
    /// </summary>
    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id, [FromQuery] string format = "pdf")
    {
        try
        {
            var (content, contentType, fileName) = await _service.GetDocumentAsync(id, format);
            return File(content, contentType, fileName);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (FileNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Удалить договор (мягкое удаление)
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