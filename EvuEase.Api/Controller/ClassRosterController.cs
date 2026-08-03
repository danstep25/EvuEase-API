using EvuEase.Application.DTOs.ClassRoster;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EvuEase.API.Controller;




[Route("api/[controller]")]
public class ClassRosterController : BaseController
{
    private readonly IClassRosterService _classRosterService;

    public ClassRosterController(IClassRosterService classRosterService)
    {
        _classRosterService = classRosterService;
    }

    
    
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetClasses([FromQuery] string? search, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _classRosterService.GetClassesAsync(search, cancellationToken);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while loading classes.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateClass([FromBody] CreateClassRosterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _classRosterService.CreateClassAsync(request, cancellationToken);
            return Created(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while creating the class.", ex.Message);
        }
    }

    
    
    
    [Authorize]
    [HttpPost("preview-class-list-pdf")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> PreviewClassListPdf(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("A PDF file is required.");
        }

        var name = file.FileName ?? string.Empty;
        if (!name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only PDF files are accepted.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var data = await _classRosterService.PreviewClassListPdfAsync(stream, cancellationToken);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while previewing the class roster PDF.", ex.Message);
        }
    }

    
    
    
    
    [Authorize]
    [HttpPost("import-pdf")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> ImportClassRosterPdf(
        [FromForm] IFormFile? file,
        [FromForm] List<string>? includedRowKeys,
        [FromForm] string? programCurriculaJson,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("A PDF file is required.");
        }

        var name = file.FileName ?? string.Empty;
        if (!name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only PDF files are accepted.");
        }

        IReadOnlyList<ProgramCurriculumImportSelection>? programCurricula = null;
        if (!string.IsNullOrWhiteSpace(programCurriculaJson))
        {
            try
            {
                programCurricula = JsonSerializer.Deserialize<List<ProgramCurriculumImportSelection>>(
                    programCurriculaJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                return BadRequest("Invalid programCurriculaJson payload.");
            }
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var data = await _classRosterService.ImportClassRosterPdfAsync(
                stream,
                includedRowKeys,
                programCurricula,
                cancellationToken);
            return Ok(data);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while importing the class roster PDF.", ex.Message);
        }
    }

    
    
    
    [Authorize]
    [HttpPost("{id:long}/students/upload")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> UploadClassRosterPdf(long id, IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("A PDF file is required.");
        }

        var name = file.FileName ?? string.Empty;
        if (!name.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only PDF files are accepted.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var data = await _classRosterService.BatchUploadRosterPdfAsync(id, stream, cancellationToken);
            return Ok(data);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while importing the class roster PDF.", ex.Message);
        }
    }

    
    
    
    [Authorize]
    [HttpPost("{id:long}/students")]
    public async Task<IActionResult> AddStudentToClass(
        long id,
        [FromBody] AddStudentToClassRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null || request.StudentId <= 0)
        {
            return BadRequest("A valid student ID is required.");
        }

        try
        {
            var data = await _classRosterService.AddStudentToClassAsync(id, request.StudentId, cancellationToken);
            return Created(data);
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
            return InternalServerError("An error occurred while adding the student to the class.", ex.Message);
        }
    }

    
    
    
    [Authorize]
    [HttpDelete("{id:long}/students/{enrollmentId:long}")]
    public async Task<IActionResult> RemoveStudentFromClass(long id, long enrollmentId, CancellationToken cancellationToken)
    {
        try
        {
            await _classRosterService.RemoveStudentFromClassAsync(id, enrollmentId, cancellationToken);
            return Success(200);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while removing the student from the class.", ex.Message);
        }
    }

    
    
    
    [Authorize]
    [HttpGet("{id:long}/students")]
    public async Task<IActionResult> GetClassStudents(long id, CancellationToken cancellationToken)
    {
        try
        {
            var data = await _classRosterService.GetStudentsForClassAsync(id, cancellationToken);
            return Ok(data);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while loading class students.", ex.Message);
        }
    }

    
    
    
    [Authorize]
    [HttpPatch("enrollments/{enrollmentId:long}/grade")]
    public async Task<IActionResult> UpdateEnrollmentGrade(
        long enrollmentId,
        [FromBody] UpdateEnrollmentGradeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _classRosterService.UpdateEnrollmentGradeAsync(enrollmentId, request, cancellationToken);
            return Ok(data);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while updating the grade.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteClass(long id, CancellationToken cancellationToken)
    {
        try
        {
            await _classRosterService.DeleteClassAsync(id, cancellationToken);
            return Success(200);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while deleting the class.", ex.Message);
        }
    }
}
