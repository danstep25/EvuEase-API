namespace EvuEase.Domain.Entities
{
    public class User
    {

        public int userid { get; private set; }
        public string username { get; private set; } = string.Empty;
        public string password_hash { get; private set; } = string.Empty;

        public User() { }
    }
}

