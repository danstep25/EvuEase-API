namespace EvuEase.Application.DTOs.User;

public class UserResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int Status { get; set; }
}

