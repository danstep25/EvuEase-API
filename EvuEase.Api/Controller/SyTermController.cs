using EvuEase.Application.DTOs.SyTerm;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class SyTermController : BaseController
{
    private readonly ISyTermService _syTermService;

    public SyTermController(ISyTermService syTermService)
    {
        _syTermService = syTermService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] SyTermRequest syTermRequest)
    {
        try
        {
            var data = await _syTermService.GetAllSyTerms(syTermRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving school year terms.", ex.Message);
        }
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        try
        {
            var data = await _syTermService.GetCurrentSyTermAsync(cancellationToken);
            if (data == null)
            {
                return NotFound("No active school year term is configured.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving the current school year term.", ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id}/set-current")]
    public async Task<IActionResult> SetCurrent(long id, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _syTermService.SetCurrentSyTermAsync(id, cancellationToken);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while setting school year term {id} as current.", ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _syTermService.GetSyTermByIdAsync(id);
            if (data == null)
            {
                return NotFound($"School Year Term with ID {id} not found.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while retrieving school year term with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateSyTermRequest syTermRequest)
    {
        try
        {
            var data = await _syTermService.CreateSyTermAsync(syTermRequest);
            return Created(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while creating the school year term.", ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateSyTermRequest syTermRequest)
    {
        try
        {
            if (id != syTermRequest.SyId)
            {
                return BadRequest("School Year Term ID in route does not match body.");
            }
            var data = await _syTermService.UpdateSyTermAsync(syTermRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while updating school year term with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _syTermService.DeleteSyTermAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while deleting school year term with ID {id}.", ex.Message);
        }
    }
}

