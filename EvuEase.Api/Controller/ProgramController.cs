using EvuEase.Application.DTOs.Program;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class ProgramController : BaseController
{
    private readonly IProgramService _programService;

    public ProgramController(IProgramService programService)
    {
        _programService = programService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ProgramRequest programRequest)
    {
        try
        {
            var data = await _programService.GetAllPrograms(programRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving programs.", ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _programService.GetProgramByIdAsync(id);
            if (data == null)
            {
                return NotFound($"Program with ID {id} not found.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving program.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateProgramRequest programRequest)
    {
        try
        {
            var data = await _programService.CreateProgramAsync(programRequest);
            return Created(data);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while creating program.", ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateProgramRequest programRequest)
    {
        try
        {
            programRequest.ProgramId = id;
            var data = await _programService.UpdateProgramAsync(programRequest);
            return Ok(data);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while updating program.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _programService.DeleteProgramAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while deleting program.", ex.Message);
        }
    }
}


