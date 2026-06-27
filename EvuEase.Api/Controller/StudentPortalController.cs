using EvuEase.Application.Interfaces.Services;
using EvuEase.Application.DTOs.StudentPortal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class StudentPortalController : BaseController
{
    private readonly IStudentPortalService _studentPortalService;

    public StudentPortalController(IStudentPortalService studentPortalService)
    {
        _studentPortalService = studentPortalService;
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] StudentPortalLoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _studentPortalService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpPost("password-reset-request")]
    public async Task<IActionResult> RequestPasswordReset(
        [FromBody] StudentPortalPasswordResetCreateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _studentPortalService.RequestPasswordResetAsync(request, cancellationToken);
            return Success(201);
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

    [Authorize(Roles = "Student")]
    [HttpGet("Me")]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var studentId = GetStudentIdFromClaims();
        if (studentId == null)
        {
            return Unauthorized("Invalid student token.");
        }

        var profile = await _studentPortalService.GetProfileAsync(studentId.Value, cancellationToken);
        return profile == null ? NotFound("Student not found.") : Ok(profile);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken)
    {
        var studentId = GetStudentIdFromClaims();
        if (studentId == null)
        {
            return Unauthorized("Invalid student token.");
        }

        var dashboard = await _studentPortalService.GetDashboardAsync(studentId.Value, cancellationToken);
        return dashboard == null ? NotFound("Student not found.") : Ok(dashboard);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("enrollments")]
    public async Task<IActionResult> GetEnrollments(CancellationToken cancellationToken)
    {
        var studentId = GetStudentIdFromClaims();
        if (studentId == null)
        {
            return Unauthorized("Invalid student token.");
        }

        var overview = await _studentPortalService.GetEnrollmentsAsync(studentId.Value, cancellationToken);
        return overview == null ? NotFound("Student not found.") : Ok(overview);
    }

    [Authorize(Roles = "Student")]
    [HttpGet("pending-subjects")]
    public async Task<IActionResult> GetPendingSubjects(CancellationToken cancellationToken)
    {
        var studentId = GetStudentIdFromClaims();
        if (studentId == null)
        {
            return Unauthorized("Invalid student token.");
        }

        var pending = await _studentPortalService.GetPendingSubjectsAsync(studentId.Value, cancellationToken);
        return Ok(pending);
    }

    [Authorize(Roles = "Student")]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody] StudentPortalChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = GetStudentIdFromClaims();
        if (studentId == null)
        {
            return Unauthorized("Invalid student token.");
        }

        try
        {
            await _studentPortalService.ChangePasswordAsync(studentId.Value, request, cancellationToken);
            return Success();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Registrar,Admin")]
    [HttpGet("password-reset-requests")]
    public async Task<IActionResult> ListPasswordResetRequests(
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var rows = await _studentPortalService.ListPasswordResetRequestsAsync(status, cancellationToken);
        return Ok(rows);
    }

    [Authorize(Roles = "Registrar,Admin")]
    [HttpPost("password-reset-requests/{id:long}/resolve")]
    public async Task<IActionResult> ResolvePasswordResetRequest(
        long id,
        [FromBody] StudentPortalPasswordResetResolveRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _studentPortalService.ResolvePasswordResetRequestAsync(
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

    [Authorize(Roles = "Registrar,Admin")]
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

    private long? GetStudentIdFromClaims()
    {
        var claim = User.FindFirst("StudentId");
        return claim != null && long.TryParse(claim.Value, out var id) ? id : null;
    }
}
