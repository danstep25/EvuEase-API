namespace EvuEase.Domain.Entities;

public class FailedJob
{
    public long id { get; private set; }
    public string uuid { get; private set; } = string.Empty;
    public string connection { get; private set; } = string.Empty;
    public string queue { get; private set; } = string.Empty;
    public string payload { get; private set; } = string.Empty;
    public string exception { get; private set; } = string.Empty;
    public DateTime failed_at { get; private set; }
}

