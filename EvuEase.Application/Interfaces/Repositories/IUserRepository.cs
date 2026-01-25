using EvuEase.Application.Common;
using EvuEase.Application.DTOs.User;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<PagedResults<User>> GetAllUsers(UserRequest userRequest);
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<User?> GetUserByIdAsync(long id);
    public Task<User> CreateUserAsync(User user);
}

