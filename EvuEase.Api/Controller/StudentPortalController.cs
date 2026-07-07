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
    [HttpGet("grade-history")]
    public async Task<IActionResult> GetGradeHistory(CancellationToken cancellationToken)
    {
        var studentId = GetStudentIdFromClaims();
        if (studentId == null)
        {
            return Unauthorized("Invalid student token.");
        }

        var history = await _studentPortalService.GetGradeHistoryAsync(studentId.Value, cancellationToken);
        return Ok(history);
    }

    [Authorize(Roles = "Student")]
    [HttpPost("set-new-password")]
    public async Task<IActionResult> SetNewPassword(
        [FromBody] StudentPortalSetNewPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = GetStudentIdFromClaims();
        if (studentId == null)
        {
            return Unauthorized("Invalid student token.");
        }

        try
        {
            await _studentPortalService.SetNewPasswordAsync(studentId.Value, request, cancellationToken);
            return Success();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
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

    [Authorize(Roles = "Student")]
    [HttpPost("me/password-reset-request")]
    public async Task<IActionResult> RequestPasswordResetAuthenticated(
        [FromBody] StudentPortalAuthenticatedResetRequest request,
        CancellationToken cancellationToken)
    {
        var studentId = GetStudentIdFromClaims();
        if (studentId == null)
        {
            return Unauthorized("Invalid student token.");
        }

        try
        {
            await _studentPortalService.RequestPasswordResetForStudentAsync(
                studentId.Value,
                request.Reason,
                cancellationToken);
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

    private long? GetStudentIdFromClaims()
    {
        var claim = User.FindFirst("StudentId");
        return claim != null && long.TryParse(claim.Value, out var id) ? id : null;
    }
}
