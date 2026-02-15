using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.SystemLog;

public class SystemLogRequest : FilterBaseDto
{
    public string? User { get; set; }
    public string? Role { get; set; }
    public string? Action { get; set; }
    public string? Module { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

