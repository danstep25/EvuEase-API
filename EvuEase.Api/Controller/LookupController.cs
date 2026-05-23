using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class LookupController : BaseController
{
    private readonly ILookupService _lookupService;

    public LookupController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet("modules")]
    public async Task<IActionResult> GetModules()
    {
        try
        {
            var data = await _lookupService.GetModuleLookupAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving module lookup.", ex.Message);
        }
    }

    [HttpGet("programs")]
    public async Task<IActionResult> GetPrograms()
    {
        try
        {
            var data = await _lookupService.GetProgramsLookupAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving programs lookup.", ex.Message);
        }
    }

    [HttpGet("syterms")]
    public async Task<IActionResult> GetSyTerms()
    {
        try
        {
            var data = await _lookupService.GetSyTermsLookupAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving school year terms lookup.", ex.Message);
        }
    }

    [HttpGet("curricula")]
    public async Task<IActionResult> GetCurricula([FromQuery] long? programId = null)
    {
        try
        {
            var data = await _lookupService.GetCurriculaLookupAsync(programId);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving curricula lookup.", ex.Message);
        }
    }

    [HttpGet("curriculum-versions")]
    public async Task<IActionResult> GetCurriculumVersions([FromQuery] string programCode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(programCode))
            {
                return BadRequest("Program code is required.");
            }

            var data = await _lookupService.GetCurriculumVersionsLookupAsync(programCode);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving curriculum versions lookup.", ex.Message);
        }
    }

    [HttpGet("courses")]
    public async Task<IActionResult> GetCourses()
    {
        try
        {
            var data = await _lookupService.GetCoursesLookupAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving courses lookup.", ex.Message);
        }
    }

    [HttpGet("grade-roster-classes")]
    public async Task<IActionResult> GetGradeRosterClasses(
        [FromQuery] string? academicTerm,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(academicTerm))
        {
            return BadRequest("Academic term is required.");
        }

        try
        {
            var data = await _lookupService.GetGradeRosterClassLookupAsync(academicTerm, search, cancellationToken);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while loading grade roster class lookup.", ex.Message);
        }
    }
}

