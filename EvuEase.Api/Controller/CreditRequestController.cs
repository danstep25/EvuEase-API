using EvuEase.Application.DTOs.CreditRequest;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class CreditRequestController : BaseController
{
    private readonly ICreditRequestService _creditRequestService;

    public CreditRequestController(ICreditRequestService creditRequestService)
    {
        _creditRequestService = creditRequestService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] CreditRequestRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _creditRequestService.GetAllCreditRequestsAsync(request, cancellationToken);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving credit requests.", ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _creditRequestService.GetCreditRequestByIdAsync(id, cancellationToken);
            if (data == null)
            {
                return NotFound($"Credit request with ID {id} was not found.");
            }

            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while retrieving credit request with ID {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateCreditRequestRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _creditRequestService.CreateCreditRequestAsync(request, cancellationToken);
            return Created(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while creating the credit request.", ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        long id,
        [FromBody] UpdateCreditRequestStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _creditRequestService.UpdateCreditRequestStatusAsync(id, request, cancellationToken);
            if (data == null)
            {
                return NotFound($"Credit request with ID {id} was not found.");
            }

            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while updating credit request {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("{id}/signed-pdf")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> UploadSignedPdf(long id, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("A PDF file is required.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var data = await _creditRequestService.UploadSignedPdfAsync(id, stream, file.FileName, cancellationToken);
            if (data == null)
            {
                return NotFound($"Credit request with ID {id} was not found.");
            }

            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while uploading the signed PDF for credit request {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpGet("{id}/signed-pdf")]
    public async Task<IActionResult> DownloadSignedPdf(long id, CancellationToken cancellationToken)
    {
        try
        {
            var file = await _creditRequestService.GetSignedPdfAsync(id, cancellationToken);
            if (file == null)
            {
                return NotFound("Signed PDF was not found for this credit request.");
            }

            return File(file.Value.Stream, "application/pdf", file.Value.FileName);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while downloading the signed PDF for credit request {id}.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id}/signed-pdf")]
    public async Task<IActionResult> RemoveSignedPdf(long id, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _creditRequestService.RemoveSignedPdfAsync(id, cancellationToken);
            if (data == null)
            {
                return NotFound($"Credit request with ID {id} was not found.");
            }

            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while removing the signed PDF for credit request {id}.", ex.Message);
        }
    }
}
