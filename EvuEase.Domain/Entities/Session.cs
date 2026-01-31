namespace EvuEase.Domain.Entities;

public class Session
{
    public string id { get; private set; } = string.Empty;
    public long? user_id { get; private set; }
    public string? ip_address { get; private set; }
    public string? user_agent { get; private set; }
    public string payload { get; private set; } = string.Empty;
    public int last_activity { get; private set; }
}

