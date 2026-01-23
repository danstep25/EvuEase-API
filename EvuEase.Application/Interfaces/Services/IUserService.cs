namespace EvuEase.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<List<String>> AllUsers();
    }
}

