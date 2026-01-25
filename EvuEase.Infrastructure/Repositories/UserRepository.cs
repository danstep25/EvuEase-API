using EvuEase.Application.Common;
using EvuEase.Application.DTOs.User;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<PagedResults<User>> GetAllUsers(UserRequest userRequest)
    {
        return await GetAll().PaginateAsync(
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
        return await GetByIdAsync(id);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        await AddAsync(user);
        await SaveChangesAsync();
        return user;
    }
}

