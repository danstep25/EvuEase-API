using Microsoft.EntityFrameworkCore;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Persistence;

namespace EvuEase.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext){}

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await GetAll().ToListAsync();
        }
    }
}

