using EvuEase.Application.DTOs;

namespace EvuEase.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<UserResponse?> GetUserByIdAsync(long id);
}

