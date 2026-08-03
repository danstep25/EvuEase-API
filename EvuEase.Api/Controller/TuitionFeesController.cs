using EvuEase.Application.DTOs.TuitionFee;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class TuitionFeesController : BaseController
{
    private readonly ITuitionFeeService _tuitionFeeService;

    public TuitionFeesController(ITuitionFeeService tuitionFeeService)
    {
        _tuitionFeeService = tuitionFeeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] TuitionFeeRequest tuitionFeeRequest)
    {
        try
        {
            var data = await _tuitionFeeService.GetAllTuitionFees(tuitionFeeRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving tuition fees.", ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _tuitionFeeService.GetTuitionFeeByIdAsync(id);
            if (data == null)
            {
                return NotFound($"Tuition fee with ID {id} not found.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while retrieving tuition fee with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateTuitionFeeRequest tuitionFeeRequest)
    {
        try
        {
            var data = await _tuitionFeeService.CreateTuitionFeeAsync(tuitionFeeRequest);
            return Created(data);
        }
        catch (Exception ex)
        {
            var errorDetails = ex.InnerException != null 
                ? $"{ex.Message}. Inner: {ex.InnerException.Message}" 
                : ex.Message;
            return InternalServerError("An error occurred while creating the tuition fee.", errorDetails);
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTuitionFeeRequest tuitionFeeRequest)
    {
        try
        {
            if (id != tuitionFeeRequest.Id)
            {
                return BadRequest("Tuition fee ID in route does not match body.");
            }
            var data = await _tuitionFeeService.UpdateTuitionFeeAsync(tuitionFeeRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while updating tuition fee with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _tuitionFeeService.DeleteTuitionFeeAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while deleting tuition fee with ID {id}.", ex.Message);
        }
    }
}

