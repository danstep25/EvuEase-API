using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetAllUsers();
}

