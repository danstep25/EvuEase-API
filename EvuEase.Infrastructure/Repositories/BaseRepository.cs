using Microsoft.EntityFrameworkCore;
using EvuEase.Infrastructure.Persistence;
using EvuEase.Domain.Entities;
using EvuEase.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace EvuEase.Infrastructure.Repositories
{
    public abstract class BaseRepository<T> where T : class
    {
        protected readonly AppDbContext dbContext;
        protected readonly IHttpContextAccessor? httpContextAccessor;

        protected BaseRepository(AppDbContext dbContext, IHttpContextAccessor? httpContextAccessor = null)
        {
            this.dbContext = dbContext;
            this.httpContextAccessor = httpContextAccessor;
        }

        protected IQueryable<T> GetAll() => dbContext.Set<T>().AsQueryable();

        protected async Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        protected async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Set<T>().FindAsync(new[] { id }, cancellationToken);
        }

        protected async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await dbContext.Set<T>().AddAsync(entity, cancellationToken);
            var moduleDescription = GetModuleDescription(typeof(T).Name);
            await AuditAsync("create", moduleDescription, $"Created {typeof(T).Name} entity", cancellationToken);
            return entity;
        }

        protected async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            dbContext.Set<T>().Update(entity);
            var moduleDescription = GetModuleDescription(typeof(T).Name);
            await AuditAsync("update", moduleDescription, $"Updated {typeof(T).Name} entity", cancellationToken);
        }

        protected async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            dbContext.Set<T>().Remove(entity);
            var moduleDescription = GetModuleDescription(typeof(T).Name);
            await AuditAsync("delete", moduleDescription, $"Deleted {typeof(T).Name} entity", cancellationToken);
        }

        protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }

        protected async Task AuditAsync(string action, string module, string details, CancellationToken cancellationToken = default)
        {
            try
            {
                var userName = "System";
                var userRole = "System";
                string? ipAddress = null;

                if (httpContextAccessor?.HttpContext != null)
                {
                    var httpContext = httpContextAccessor.HttpContext;
                    var user = httpContext.User;

                    if (user.Identity != null && user.Identity.IsAuthenticated)
                    {
                        userName = user.FindFirst("UserName")?.Value
                            ?? user.FindFirst("Email")?.Value
                            ?? user.FindFirst("UserId")?.Value
                            ?? "System";

                        userRole = user.FindFirst("Role")?.Value
                            ?? "System";

                        ipAddress = GetClientIpAddress(httpContext);
                    }
                }

                var systemLog = SystemLog.Create(
                    user: userName,
                    role: userRole,
                    action: action,
                    module: module,
                    details: details,
                    ip_address: ipAddress
                );

                await dbContext.Set<SystemLog>().AddAsync(systemLog, cancellationToken);
            }
            catch (Exception)
            {
            }
        }

        private string GetModuleDescription(string entityTypeName)
        {
            // Map entity type names to Module enum values
            // If entity name matches a Module enum value, return its description
            // Otherwise, return the entity type name as fallback
            if (Enum.TryParse<Module>(entityTypeName, true, out var module))
            {
                return module.GetDescription();
            }

            // Handle common mappings (e.g., "User" -> "UserManagement")
            var entityToModuleMap = new Dictionary<string, Module>(StringComparer.OrdinalIgnoreCase)
            {
                { "User", Module.User }
            };

            if (entityToModuleMap.TryGetValue(entityTypeName, out var mappedModule))
            {
                return mappedModule.GetDescription();
            }

            // Fallback to entity type name if no mapping found
            return entityTypeName;
        }

        private string? GetClientIpAddress(HttpContext httpContext)
        {
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ip = forwardedFor.Split(',')[0].Trim();
                if (!string.IsNullOrEmpty(ip))
                {
                    return ip;
                }
            }

            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            return httpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}

