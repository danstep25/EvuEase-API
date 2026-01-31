namespace EvuEase.Domain.Entities;

public class Job
{
    public long id { get; private set; }
    public string queue { get; private set; } = string.Empty;
    public string payload { get; private set; } = string.Empty;
    public byte attempts { get; private set; }
    public int? reserved_at { get; private set; }
    public int available_at { get; private set; }
    public int created_at { get; private set; }
}

