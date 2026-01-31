using EvuEase.Application.Common;
using EvuEase.Application.DTOs.SystemLog;

namespace EvuEase.Application.Interfaces.Services;

public interface ISystemLogService
{
    Task<PagedResults<SystemLogResponse>> GetAllSystemLogs(SystemLogRequest systemLogRequest);
    Task<SystemLogResponse?> GetSystemLogByIdAsync(long id);
    Task<SystemLogStatisticsResponse> GetStatisticsAsync();
}

