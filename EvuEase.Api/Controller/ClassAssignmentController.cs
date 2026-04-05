using EvuEase.Application.DTOs.FacultyCenter;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;




[Route("api/[controller]")]
public class ClassAssignmentController : BaseController
{
    private readonly IClassAssignmentService _classAssignmentService;

    public ClassAssignmentController(IClassAssignmentService classAssignmentService)
    {
        _classAssignmentService = classAssignmentService;
    }

    [Authorize]
    [HttpGet("grading-scheme-basis")]
    public async Task<IActionResult> GetGradingSchemeBasis(
        [FromQuery] string academicTermKey,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _classAssignmentService.GetGradingSchemeBasisAsync(academicTermKey, cancellationToken);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while loading grading scheme and basis.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("grading-scheme-basis")]
    public async Task<IActionResult> SaveGradingSchemeBasis(
        [FromBody] SaveGradingSchemeBasisRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _classAssignmentService.SaveGradingSchemeBasisAsync(request, cancellationToken);
            return Success(200);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while saving grading scheme and basis.", ex.Message);
        }
    }
}
