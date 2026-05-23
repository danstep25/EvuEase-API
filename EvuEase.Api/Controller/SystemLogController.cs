using EvuEase.Application.DTOs.SystemLog;
using EvuEase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace EvuEase.API.Controller;

[Route("api/[controller]")]
public class SystemLogController : BaseController
{
    private readonly ISystemLogService _systemLogService;

    public SystemLogController(ISystemLogService systemLogService)
    {
        _systemLogService = systemLogService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] SystemLogRequest systemLogRequest)
    {
        try
        {
            var data = await _systemLogService.GetAllSystemLogs(systemLogRequest);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving system logs.", ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _systemLogService.GetSystemLogByIdAsync(id);
            if (data == null)
            {
                return NotFound("System log not found.");
            }
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving system log.", ex.Message);
        }
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        try
        {
            var data = await _systemLogService.GetStatisticsAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return InternalServerError("An error occurred while retrieving system log statistics.", ex.Message);
        }
    }
}

