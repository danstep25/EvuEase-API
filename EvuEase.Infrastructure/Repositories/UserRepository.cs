using EvuEase.Application.Common;
using EvuEase.Application.DTOs.User;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Domain.Enums;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) 
        : base(dbContext, httpContextAccessor) { }

    public async Task<PagedResults<User>> GetAllUsers(UserRequest userRequest)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(userRequest.SearchTerm))
        {
            var searchTerm = userRequest.SearchTerm.ToLower();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u => 
                    u.name.ToLower().Contains(searchTerm) ||
                    u.email.ToLower().Contains(searchTerm) ||
                    u.role.ToLower().Contains(searchTerm)
                );
            }
            else
            {
                query = searchTerm switch
                {
                    "name" => query.Where(u => u.name.ToLower().Contains(searchTerm)),
                    "email" => query.Where(u => u.email.ToLower().Contains(searchTerm)),
                    "role" => query.Where(u => u.role.ToLower().Contains(searchTerm)),
                    _ => query.Where(u => 
                        u.name.ToLower().Contains(searchTerm) ||
                        u.email.ToLower().Contains(searchTerm) ||
                        u.role.ToLower().Contains(searchTerm)
                    )
                };
            }
        }

        if (!string.IsNullOrWhiteSpace(userRequest.Name))
        {
            query = query.Where(u => u.name.ToLower().Contains(userRequest.Name.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(userRequest.Email))
        {
            query = query.Where(u => u.email.ToLower().Contains(userRequest.Email.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(userRequest.Role))
        {
            query = query.Where(u => u.role.ToLower() == userRequest.Role.ToLower());
        }

        if (userRequest.Status.HasValue)
        {
            query = query.Where(u => u.status == userRequest.Status.Value);
        }

        return await query.PaginateAsync(
            userRequest.PageIndex, 
            userRequest.PageSize, 
            userRequest.SortKey, 
            userRequest.SortDirection
        );
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await GetAll().FirstOrDefaultAsync(u => u.email == email);
    }

    public async Task<User?> GetUserByIdAsync(long id)
    {
        return await GetAll().FirstOrDefaultAsync(user => user.id == id);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await AddAsync(user);
        await SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        await UpdateAsync(user);
        await SaveChangesAsync();
        return user;
    }

    public async Task<UserStatisticsResponse> GetStatisticsAsync()
    {
        var query = GetAll();

        var totalUsers = await query.CountAsync();
        var activeUsers = await query.CountAsync(u => u.status == true);
        
        var adminRole = Role.Admin.GetDescription();
        var evaluatorRole = Role.Evaluator.GetDescription();
        var registrarRole = Role.Registrar.GetDescription();

        var administrators = await query.CountAsync(u => u.role.ToLower() == adminRole.ToLower());
        var evaluators = await query.CountAsync(u => u.role.ToLower() == evaluatorRole.ToLower());
        var registrars = await query.CountAsync(u => u.role.ToLower() == registrarRole.ToLower());

        return new UserStatisticsResponse
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            Administrators = administrators,
            Evaluators = evaluators,
            Registrars = registrars
        };
    }
}

