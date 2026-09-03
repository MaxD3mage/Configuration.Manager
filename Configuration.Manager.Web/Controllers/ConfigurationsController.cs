using Configuration.Manager.BusinessLogic.App.DTOs;
using Configuration.Manager.BusinessLogic.App.Services;
using Microsoft.AspNetCore.Mvc;
namespace Configuration.Manager.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigurationsController(IConfigurationService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string userId,
        [FromQuery] string? name,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var result = await service.GetListAsync(userId, name, from, to);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] string userId)
    {
        var result = await service.GetByIdAsync(id, userId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateConfigurationDto dto, [FromQuery] string userId)
    {
        var result = await service.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id, userId }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateConfigurationDto dto, [FromQuery] string userId)
    {
        var result = await service.UpdateAsync(id, dto, userId);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/rollback")]
    public async Task<IActionResult> Rollback(Guid id, [FromQuery] Guid versionId, [FromQuery] string userId)
    {
        await service.RollbackAsync(id, versionId, userId);
        return NoContent();
    }
}