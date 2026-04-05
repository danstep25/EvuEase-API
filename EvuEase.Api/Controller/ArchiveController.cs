using EvuEase.Application.DTOs.Archive;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class ArchiveController : BaseController
{
    private readonly IArchiveService _archiveService;

    public ArchiveController(IArchiveService archiveService)
    {
        _archiveService = archiveService;
    }

    [Authorize]
    [HttpGet("programs")]
    public async Task<IActionResult> ListPrograms([FromQuery] ArchiveProgramListQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _archiveService.ListProgramsAsync(query, cancellationToken);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while loading archived programs.", ex.Message);
        }
    }

    [Authorize]
    [HttpGet("students")]
    public async Task<IActionResult> ListStudents([FromQuery] ArchiveStudentListQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _archiveService.ListStudentsAsync(query, cancellationToken);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while loading archived students.", ex.Message);
        }
    }

    [Authorize]
    [HttpGet("school-years")]
    public async Task<IActionResult> ListSchoolYears([FromQuery] ArchiveSchoolYearListQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _archiveService.ListSchoolYearsAsync(query, cancellationToken);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while loading archived school years.", ex.Message);
        }
    }

    [Authorize]
    [HttpGet("curricula")]
    public async Task<IActionResult> ListCurricula([FromQuery] ArchiveListQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _archiveService.ListCurriculaAsync(query, cancellationToken);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while loading archived curricula.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("programs/{id:long}/restore")]
    public async Task<IActionResult> RestoreProgram(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _archiveService.RestoreProgramAsync(id, cancellationToken);
            return Success();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("Could not restore program.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("programs/{id:long}/permanent")]
    public async Task<IActionResult> PermanentDeleteProgram(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _archiveService.PermanentDeleteProgramAsync(id, cancellationToken);
            return Success();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            return Conflict("Cannot permanently delete this program while other records still reference it.", ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("Could not permanently delete program.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("students/{id:long}/restore")]
    public async Task<IActionResult> RestoreStudent(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _archiveService.RestoreStudentAsync(id, cancellationToken);
            return Success();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("Could not restore student.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("students/{id:long}/permanent")]
    public async Task<IActionResult> PermanentDeleteStudent(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _archiveService.PermanentDeleteStudentAsync(id, cancellationToken);
            return Success();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("Could not permanently delete student.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("school-years/{id:long}/restore")]
    public async Task<IActionResult> RestoreSchoolYear(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _archiveService.RestoreSchoolYearAsync(id, cancellationToken);
            return Success();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("Could not restore school year.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("school-years/{id:long}/permanent")]
    public async Task<IActionResult> PermanentDeleteSchoolYear(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _archiveService.PermanentDeleteSchoolYearAsync(id, cancellationToken);
            return Success();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (DbUpdateException ex)
        {
            return Conflict("Cannot permanently delete this school year while other records still reference it.", ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("Could not permanently delete school year.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("curricula/{id:long}/restore")]
    public async Task<IActionResult> RestoreCurriculum(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _archiveService.RestoreCurriculumAsync(id, cancellationToken);
            return Success();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("Could not restore curriculum.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("curricula/{id:long}/permanent")]
    public async Task<IActionResult> PermanentDeleteCurriculum(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _archiveService.PermanentDeleteCurriculumAsync(id, cancellationToken);
            return Success();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("Could not permanently delete curriculum.", ex.Message);
        }
    }
}
