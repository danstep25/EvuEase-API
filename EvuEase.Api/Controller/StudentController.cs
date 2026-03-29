using EvuEase.Application.DTOs.Student;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class StudentController : BaseController
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] StudentRequest studentRequest)
    {
        try
        {
            var data = await _studentService.GetAllStudents(studentRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving students.", ex.Message);
        }
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _studentService.GetStudentByIdAsync(id);
            if (data == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving student.", ex.Message);
        }
    }

    [Authorize]
    [HttpPost("new")]
    public async Task<IActionResult> Create([FromBody] CreateStudentRequest request)
    {
        try
        {
            var data = await _studentService.CreateStudentAsync(request);
            return Created(data);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while creating student.", ex.Message);
        }
    }

    [Authorize]
    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateStudentRequest request)
    {
        try
        {
            request.Id = id;
            var data = await _studentService.UpdateStudentAsync(request);
            return Ok(data);
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
            return InternalServerError("An error occurred while updating student.", ex.Message);
        }
    }

    [Authorize]
    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _studentService.DeleteStudentAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while deleting student.", ex.Message);
        }
    }
}
