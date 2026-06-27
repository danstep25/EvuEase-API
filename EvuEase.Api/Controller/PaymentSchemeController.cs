using EvuEase.Application.DTOs.PaymentScheme;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class PaymentSchemeController : BaseController
{
    private readonly IPaymentSchemeService _paymentSchemeService;

    public PaymentSchemeController(IPaymentSchemeService paymentSchemeService)
    {
        _paymentSchemeService = paymentSchemeService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PaymentSchemeRequest request)
    {
        try
        {
            var data = await _paymentSchemeService.GetAllAsync(request);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving payment schemes.", ex.Message);
        }
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _paymentSchemeService.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound($"Payment scheme with ID {id} was not found.");
            }

            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while retrieving payment scheme with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentSchemeRequest request)
    {
        try
        {
            var data = await _paymentSchemeService.CreateAsync(request);
            return Created(data);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            var details = ex.InnerException != null ? $"{ex.Message}. Inner: {ex.InnerException.Message}" : ex.Message;
            return InternalServerError("An error occurred while creating the payment scheme.", details);
        }
    }

    [Authorize]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePaymentSchemeRequest request)
    {
        try
        {
            if (id != request.Id)
            {
                return BadRequest("Payment scheme ID in route does not match body.");
            }

            var data = await _paymentSchemeService.UpdateAsync(request);
            return Ok(data);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while updating payment scheme with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _paymentSchemeService.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while deleting payment scheme with ID {id}.", ex.Message);
        }
    }
}
