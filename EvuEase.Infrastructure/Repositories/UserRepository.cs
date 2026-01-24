using Microsoft.EntityFrameworkCore;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;

namespace EvuEase.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext dbContext) : base(dbContext) { }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await GetAll().ToListAsync();
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

