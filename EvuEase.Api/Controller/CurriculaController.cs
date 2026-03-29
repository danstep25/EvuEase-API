using EvuEase.Application.DTOs.Curricula;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class CurriculaController : BaseController
{
    private readonly ICurriculaService _curriculaService;

    public CurriculaController(ICurriculaService curriculaService)
    {
        _curriculaService = curriculaService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] CurriculaRequest curriculaRequest)
    {
        try
        {
            var data = await _curriculaService.GetAllCurricula(curriculaRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving curricula.", ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _curriculaService.GetCurriculaByIdAsync(id);
            if (data == null)
            {
                return NotFound($"Curriculum with ID {id} not found.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while retrieving curriculum with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateCurriculaRequest curriculaRequest)
    {
        try
        {
            var data = await _curriculaService.CreateCurriculaAsync(curriculaRequest);
            return Created(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while creating the curriculum.", ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateCurriculaRequest curriculaRequest)
    {
        try
        {
            if (id != curriculaRequest.Id)
            {
                return BadRequest("Curriculum ID in route does not match body.");
            }
            var data = await _curriculaService.UpdateCurriculaAsync(curriculaRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while updating curriculum with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _curriculaService.DeleteCurriculaAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while deleting curriculum with ID {id}.", ex.Message);
        }
    }
}




