using EvuEase.Application.DTOs.Course;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class CourseController : BaseController
{
    private readonly ICourseService _courseService;

    public CourseController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] CourseRequest courseRequest)
    {
        try
        {
            var data = await _courseService.GetAllCourses(courseRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving courses.", ex.Message);
        }
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            var data = await _courseService.GetCourseByCodeAsync(code);
            if (data == null)
            {
                return NotFound($"Course with code '{code}' not found.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while retrieving course with code '{code}'.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("batch/detect-pdf")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> DetectBatchPdf(IFormFile? file, CancellationToken cancellationToken)
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
            var data = await _courseService.DetectBatchPdfAsync(stream, cancellationToken);
            return Ok(data);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while reading the curriculum PDF.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("batch/parse-pdf")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> ParseBatchPdf(
        IFormFile? file,
        [FromForm] long programId,
        [FromForm] string curriculumCode,
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

        if (programId <= 0)
        {
            return BadRequest("Program is required.");
        }

        if (string.IsNullOrWhiteSpace(curriculumCode))
        {
            return BadRequest("Curriculum code is required.");
        }

        try
        {
            await using var stream = file.OpenReadStream();
            var data = await _courseService.PreviewBatchPdfAsync(stream, programId, curriculumCode.Trim(), cancellationToken);
            return Ok(data);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while parsing the curriculum PDF.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("batch/preview")]
    public async Task<IActionResult> PreviewBatchImport(
        [FromBody] CourseBatchImportRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _courseService.PreviewBatchImportAsync(request, cancellationToken);
            return Ok(data);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while previewing the course batch import.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("batch/import")]
    public async Task<IActionResult> ImportBatch(
        [FromBody] CourseBatchImportRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _courseService.ImportBatchAsync(request, cancellationToken);
            return Ok(data);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while importing courses.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateCourseRequest courseRequest)
    {
        try
        {
            var data = await _courseService.CreateCourseAsync(courseRequest);
            return Created(data);
        }
        catch (Exception ex)
        {
            var errorDetails = ex.InnerException != null 
                ? $"{ex.Message}. Inner: {ex.InnerException.Message}" 
                : ex.Message;
            return InternalServerError("An error occurred while creating the course.", errorDetails);
        }
    }

    [Authorize]
    [HttpPut("{code}")]
    public async Task<IActionResult> Update(string code, [FromBody] UpdateCourseRequest courseRequest)
    {
        try
        {
            if (code != courseRequest.CourseCode)
            {
                return BadRequest("Course code in route does not match body.");
            }
            var data = await _courseService.UpdateCourseAsync(code, courseRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while updating course with code '{code}'.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        try
        {
            await _courseService.DeleteCourseAsync(code);
            return NoContent();
        }
        catch (Exception ex)
        {
            return InternalServerError($"An error occurred while deleting course with code '{code}'.", ex.Message);
        }
    }
}


