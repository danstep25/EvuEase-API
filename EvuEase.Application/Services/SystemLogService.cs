using AutoMapper;
using EvuEase.Application.Common;
using EvuEase.Application.DTOs.SystemLog;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Services;

public class SystemLogService : ISystemLogService
{
    private readonly ISystemLogRepository _systemLogRepository;
    private readonly IMapper _mapper;

    public SystemLogService(ISystemLogRepository systemLogRepository, IMapper mapper)
    {
        _systemLogRepository = systemLogRepository;
        _mapper = mapper;
    }

    public async Task<PagedResults<SystemLogResponse>> GetAllSystemLogs(SystemLogRequest systemLogRequest)
    {
        var pagedEntities = await _systemLogRepository.GetAllSystemLogs(systemLogRequest);
        return pagedEntities.MapToDto<SystemLog, SystemLogResponse>(_mapper);
    }

    public async Task<SystemLogResponse?> GetSystemLogByIdAsync(long id)
    {
        var systemLog = await _systemLogRepository.GetSystemLogByIdAsync(id);
        return systemLog != null ? _mapper.Map<SystemLogResponse>(systemLog) : null;
    }

    public async Task<SystemLogStatisticsResponse> GetStatisticsAsync()
    {
        return await _systemLogRepository.GetStatisticsAsync();
    }
}

