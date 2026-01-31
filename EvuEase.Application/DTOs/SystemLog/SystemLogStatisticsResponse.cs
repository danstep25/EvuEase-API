namespace EvuEase.Application.DTOs.SystemLog;

public class SystemLogStatisticsResponse
{
    public int TotalLogs { get; set; }
    public int CreateActions { get; set; }
    public int UpdateActions { get; set; }
    public int DeleteActions { get; set; }
}

