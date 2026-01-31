using EvuEase.Application.Common;
using EvuEase.Application.DTOs.SystemLog;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface ISystemLogRepository
{
    Task<PagedResults<SystemLog>> GetAllSystemLogs(SystemLogRequest systemLogRequest);
    Task<SystemLog?> GetSystemLogByIdAsync(long id);
    Task<SystemLogStatisticsResponse> GetStatisticsAsync();
}

