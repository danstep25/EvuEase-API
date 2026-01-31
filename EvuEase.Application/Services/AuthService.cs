using EvuEase.Application.DTOs;
using EvuEase.Application.DTOs.User;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;
using EvuEase.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EvuEase.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (!VerifyPassword(request.Password, user.password))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            Token = token,
            Email = user.email,
            Name = user.name,
            Role = user.role,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
        
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var hashedPassword = HashPassword(request.Password);
        var user = User.Create(request.Name, request.Email, hashedPassword, request.Role);
        
        var createdUser = await _userRepository.CreateUserAsync(user);
        var token = GenerateJwtToken(createdUser);

        return new AuthResponse
        {
            Token = token,
            Email = createdUser.email,
            Name = createdUser.name,
            Role = createdUser.role,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    public async Task<UserResponse?> GetUserByIdAsync(long id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        
        if (user == null)
        {
            return null;
        }

        return new UserResponse
        {
            Id = user.id,
            Name = user.name,
            Email = user.email,
            Role = user.role,
            EmailVerifiedAt = user.email_verified_at,
            CreatedAt = user.created_at
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
        var issuer = jwtSettings["Issuer"] ?? "EvuEase";
        var audience = jwtSettings["Audience"] ?? "EvuEase";
        var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "24");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId", user.id.ToString()),
            new Claim("Email", user.email),
            new Claim("UserName", user.name),
            new Claim("Role", user.role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            // Handle legacy password hashes if needed
            return false;
        }
    }
}

