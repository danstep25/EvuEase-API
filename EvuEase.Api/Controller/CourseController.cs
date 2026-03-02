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


