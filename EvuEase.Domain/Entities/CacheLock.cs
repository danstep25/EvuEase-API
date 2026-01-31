namespace EvuEase.Domain.Entities;

public class CacheLock
{
    public string key { get; private set; } = string.Empty;
    public string owner { get; private set; } = string.Empty;
    public int expiration { get; private set; }
}

