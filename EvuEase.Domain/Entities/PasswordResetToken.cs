namespace EvuEase.Domain.Entities;

public class PasswordResetToken
{
    public string email { get; private set; } = string.Empty;
    public string token { get; private set; } = string.Empty;
    public DateTime? created_at { get; private set; }
}

