using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetAllUsers();
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<User?> GetUserByIdAsync(long id);
    public Task<User> CreateUserAsync(User user);
}

