namespace EvuEase.Application.DTOs.User;

public class UserStatisticsResponse
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int Registrars { get; set; }
    public int Evaluators { get; set; }
    public int Administrators { get; set; }
}

