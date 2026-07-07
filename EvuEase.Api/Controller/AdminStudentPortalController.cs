using EvuEase.Application.DTOs.StudentPortal;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/Admin/student-portal")]
[Authorize(Policy = "AdminOnly")]
public class AdminStudentPortalController : BaseController
{
    private readonly IStudentPortalService _studentPortalService;

    public AdminStudentPortalController(IStudentPortalService studentPortalService)
    {
        _studentPortalService = studentPortalService;
    }

    [HttpGet("password-reset-requests")]
    public async Task<IActionResult> ListPasswordResetRequests(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var rows = await _studentPortalService.ListPasswordResetRequestsAsync(status, cancellationToken);
        return Ok(rows);
    }

    [HttpPost("password-reset-requests/{id:long}/issue-temporary-password")]
    public async Task<IActionResult> IssueTemporaryPassword(
        long id,
        [FromBody] StudentPortalIssueTemporaryPasswordRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _studentPortalService.IssueTemporaryPasswordAsync(
                id,
                request,
                User.FindFirst("UserName")?.Value ?? User.FindFirst("Email")?.Value,
                cancellationToken);
            return Ok(result);
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
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("password-reset-requests/{id:long}/reject")]
    public async Task<IActionResult> RejectPasswordResetRequest(
        long id,
        [FromBody] StudentPortalPasswordResetRejectRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _studentPortalService.RejectPasswordResetRequestAsync(
                id,
                request,
                User.FindFirst("UserName")?.Value ?? User.FindFirst("Email")?.Value,
                cancellationToken);
            return Success();
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
            return BadRequest(ex.Message);
        }
    }
}
