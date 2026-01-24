using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;

namespace EvuEase.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<string>> AllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return users.Select(u => u.name).ToList();
        }
    }
}

