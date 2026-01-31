namespace EvuEase.Application.Common
{
    public static class PasswordHelper
    {
        public static string Hash(this string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
        }
    }
}
