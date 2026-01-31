namespace EvuEase.Domain.Entities;

public class Cache
{
    public string key { get; private set; } = string.Empty;
    public string value { get; private set; } = string.Empty;
    public int expiration { get; private set; }
}

