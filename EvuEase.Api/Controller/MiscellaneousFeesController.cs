using EvuEase.Application.DTOs.MiscellaneousFee;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class MiscellaneousFeesController : BaseController
{
    private readonly IMiscellaneousFeeService _miscellaneousFeeService;

    public MiscellaneousFeesController(IMiscellaneousFeeService miscellaneousFeeService)
    {
        _miscellaneousFeeService = miscellaneousFeeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] MiscellaneousFeeRequest request)
    {
        try
        {
            var data = await _miscellaneousFeeService.GetAllMiscellaneousFees(request);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving miscellaneous fees.", ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _miscellaneousFeeService.GetMiscellaneousFeeByIdAsync(id);
            if (data == null)
            {
                return NotFound($"Miscellaneous fee with ID {id} not found.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while retrieving miscellaneous fee with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateMiscellaneousFeeRequest request)
    {
        try
        {
            var data = await _miscellaneousFeeService.CreateMiscellaneousFeeAsync(request);
            return Created(data);
        }
        catch (Exception ex)
        {
            var errorDetails = ex.InnerException != null
                ? $"{ex.Message}. Inner: {ex.InnerException.Message}"
                : ex.Message;
            return InternalServerError("An error occurred while creating the miscellaneous fee.", errorDetails);
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateMiscellaneousFeeRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("Miscellaneous fee ID in route does not match body.");
            }
            var data = await _miscellaneousFeeService.UpdateMiscellaneousFeeAsync(request);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while updating miscellaneous fee with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _miscellaneousFeeService.DeleteMiscellaneousFeeAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while deleting miscellaneous fee with ID {id}.", ex.Message);
        }
    }
}


