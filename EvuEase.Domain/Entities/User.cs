using EvuEase.Domain.Enums;

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
    public bool status { get; private set; }

    private User() { }

    public static User Create(string name, string email, string hashedPassword, Role role = Role.Evaluator, Status status = Status.ACTIVE)
    {
        var user = new User();
        var type = typeof(User);
        
        type.GetProperty(nameof(name))?.SetValue(user, name);
        type.GetProperty(nameof(email))?.SetValue(user, email);
        type.GetProperty(nameof(password))?.SetValue(user, hashedPassword);
        type.GetProperty(nameof(role))?.SetValue(user, role.GetDescription());
        type.GetProperty(nameof(status))?.SetValue(user, status == Status.ACTIVE ? true : false);
        type.GetProperty(nameof(created_at))?.SetValue(user, DateTime.Now);
        
        return user;
    }

    public void Update(string name, string email, Role role = Role.Evaluator, Status status = Status.ACTIVE)
    {
        var type = typeof(User);

        type.GetProperty(nameof(this.name))?.SetValue(this, name);
        type.GetProperty(nameof(this.email))?.SetValue(this, email);
        type.GetProperty(nameof(this.role))?.SetValue(this, role.GetDescription());
        type.GetProperty(nameof(this.status))?.SetValue(this, status == Status.ACTIVE ? true : false);
        type.GetProperty(nameof(updated_at))?.SetValue(this, DateTime.Now);
    }
}

