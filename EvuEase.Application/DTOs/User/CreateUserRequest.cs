using EvuEase.Domain.Enums;

namespace EvuEase.Application.DTOs.User
{
    public class CreateUserRequest
    {
        public String FullName { get; set; } = string.Empty;
        public String Email { get; set; } = string.Empty;
        public String Password { get; set; } = string.Empty;
        public Role Role { get; set; }
        public Status Status { get; set; }
    }
}
