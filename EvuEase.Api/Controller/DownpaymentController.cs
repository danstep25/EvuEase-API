using EvuEase.Application.DTOs.Downpayment;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class DownpaymentController : BaseController
{
    private readonly IDownpaymentService _downpaymentService;

    public DownpaymentController(IDownpaymentService downpaymentService)
    {
        _downpaymentService = downpaymentService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] DownpaymentRequest request)
    {
        try
        {
            var data = await _downpaymentService.GetAllAsync(request);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving downpayment settings.", ex.Message);
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] string programCode)
    {
        if (string.IsNullOrWhiteSpace(programCode))
        {
            return BadRequest("programCode is required.");
        }

        try
        {
            var data = await _downpaymentService.GetHistoryByProgramCodeAsync(programCode);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving downpayment history.", ex.Message);
        }
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _downpaymentService.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound($"Downpayment with ID {id} was not found.");
            }

            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while retrieving downpayment with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateDownpaymentRequest request)
    {
        try
        {
            var data = await _downpaymentService.CreateAsync(request);
            return Created(data);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            var details = ex.InnerException != null ? $"{ex.Message}. Inner: {ex.InnerException.Message}" : ex.Message;
            return InternalServerError("An error occurred while creating the downpayment rule.", details);
        }
    }

    [Authorize]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDownpaymentRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("Downpayment ID in route does not match body.");
            }

            var data = await _downpaymentService.UpdateAsync(request);
            return Ok(data);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while updating downpayment with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _downpaymentService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while deleting downpayment with ID {id}.", ex.Message);
        }
    }
}
