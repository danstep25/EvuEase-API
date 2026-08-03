using EvuEase.Application.DTOs.OtherSchoolFee;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class OtherSchoolFeesController : BaseController
{
    private readonly IOtherSchoolFeeService _otherSchoolFeeService;

    public OtherSchoolFeesController(IOtherSchoolFeeService otherSchoolFeeService)
    {
        _otherSchoolFeeService = otherSchoolFeeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] OtherSchoolFeeRequest request)
    {
        try
        {
            var data = await _otherSchoolFeeService.GetAllOtherSchoolFees(request);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving other school fees.", ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _otherSchoolFeeService.GetOtherSchoolFeeByIdAsync(id);
            if (data == null)
            {
                return NotFound($"Other school fee with ID {id} not found.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while retrieving other school fee with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateOtherSchoolFeeRequest request)
    {
        try
        {
            var data = await _otherSchoolFeeService.CreateOtherSchoolFeeAsync(request);
            return Created(data);
        }
        catch (Exception ex)
        {
            var errorDetails = ex.InnerException != null
                ? $"{ex.Message}. Inner: {ex.InnerException.Message}"
                : ex.Message;
            return InternalServerError("An error occurred while creating the other school fee.", errorDetails);
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateOtherSchoolFeeRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("Other school fee ID in route does not match body.");
            }
            var data = await _otherSchoolFeeService.UpdateOtherSchoolFeeAsync(request);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while updating other school fee with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _otherSchoolFeeService.DeleteOtherSchoolFeeAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while deleting other school fee with ID {id}.", ex.Message);
        }
    }
}



