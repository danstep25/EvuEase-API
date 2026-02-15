using EvuEase.Application.DTOs;
using EvuEase.Application.DTOs.User;

namespace EvuEase.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<UserResponse?> GetUserByIdAsync(long id);
}

