using System.Reflection;

namespace EvuEase.Domain.Entities;

public class User : BaseEntity
{
    public long id { get; private set; }
    public string name { get; private set; } = string.Empty;
    public string email { get; private set; } = string.Empty;
    public DateTime? email_verified_at { get; private set; }
    public string password { get; private set; } = string.Empty;
    public string? remember_token { get; private set; }
    public string role { get; private set; } = string.Empty;

    // Private constructor for EF Core
    private User() { }

    // Factory method for creating new users
    public static User Create(string name, string email, string hashedPassword, string role = "evaluator")
    {
        var user = new User();
        var type = typeof(User);
        
        type.GetProperty(nameof(name))?.SetValue(user, name);
        type.GetProperty(nameof(email))?.SetValue(user, email);
        type.GetProperty(nameof(password))?.SetValue(user, hashedPassword);
        type.GetProperty(nameof(role))?.SetValue(user, role);
        type.GetProperty(nameof(created_at))?.SetValue(user, DateTime.UtcNow);
        type.GetProperty(nameof(updated_at))?.SetValue(user, DateTime.UtcNow);
        
        return user;
    }
}

