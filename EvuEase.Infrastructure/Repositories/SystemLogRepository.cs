using EvuEase.Application.Common;
using EvuEase.Application.DTOs.SystemLog;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Domain.Enums;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class SystemLogRepository : BaseRepository<SystemLog>, ISystemLogRepository
{
    public SystemLogRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) 
        : base(dbContext, httpContextAccessor) { }

    public async Task<PagedResults<SystemLog>> GetAllSystemLogs(SystemLogRequest systemLogRequest)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(systemLogRequest.SearchTerm))
        {
            var searchTerm = systemLogRequest.SearchTerm.ToLower();
            query = query.Where(log => 
                log.user.ToLower().Contains(searchTerm) ||
                log.role.ToLower().Contains(searchTerm) ||
                log.action.ToLower().Contains(searchTerm) ||
                log.module.ToLower().Contains(searchTerm) ||
                log.details.ToLower().Contains(searchTerm)
            );
        }

        if (!string.IsNullOrWhiteSpace(systemLogRequest.User))
        {
            query = query.Where(log => log.user.ToLower().Contains(systemLogRequest.User.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(systemLogRequest.Role))
        {
            query = query.Where(log => log.role.ToLower() == systemLogRequest.Role.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(systemLogRequest.Action))
        {
            query = query.Where(log => log.action.ToLower() == systemLogRequest.Action.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(systemLogRequest.Module))
        {
            var matchedDescription = ModuleHelper.GetMatchingModuleDescription(systemLogRequest.Module);

            if (matchedDescription != null)
            {
                query = query.Where(log => log.module.ToLower() == matchedDescription.ToLower());
            }
            else
            {
                query = query.Where(log => log.module.ToLower().Contains(systemLogRequest.Module.ToLower()));
            }
        }

        if (systemLogRequest.StartDate.HasValue)
        {
            query = query.Where(log => log.timestamp >= systemLogRequest.StartDate.Value);
        }

        if (systemLogRequest.EndDate.HasValue)
        {
            query = query.Where(log => log.timestamp <= systemLogRequest.EndDate.Value);
        }

        return await query.PaginateAsync(
            systemLogRequest.PageIndex, 
            systemLogRequest.PageSize, 
            systemLogRequest.SortKey, 
            systemLogRequest.SortDirection
        );
    }

    public async Task<SystemLog?> GetSystemLogByIdAsync(long id)
    {
        return await GetAll().FirstOrDefaultAsync(log => log.log_id == id);
    }

    public async Task<SystemLogStatisticsResponse> GetStatisticsAsync()
    {
        var query = GetAll();

        var totalLogs = await query.CountAsync();
        var createActions = await query.CountAsync(log => log.action.ToLower() == "create");
        var updateActions = await query.CountAsync(log => log.action.ToLower() == "update");
        var deleteActions = await query.CountAsync(log => log.action.ToLower() == "delete");

        return new SystemLogStatisticsResponse
        {
            TotalLogs = totalLogs,
            CreateActions = createActions,
            UpdateActions = updateActions,
            DeleteActions = deleteActions
        };
    }
}

