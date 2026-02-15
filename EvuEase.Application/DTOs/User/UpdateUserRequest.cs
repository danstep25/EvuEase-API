using EvuEase.Domain.Enums;

namespace EvuEase.Application.DTOs.User
{
    public class UpdateUserRequest
    {
        public long Id { get; set; }
        public String FullName { get; set; } = string.Empty;
        public String Email { get; set; } = string.Empty;
        public Role Role { get; set; }
        public Status Status { get; set; }
    }
}
