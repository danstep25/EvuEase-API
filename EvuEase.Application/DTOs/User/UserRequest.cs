namespace EvuEase.Application.DTOs.User
{
    public class UserRequest : FilterBaseDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool? Status { get; set; }
    }
}
