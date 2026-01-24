namespace EvuEase.Domain.Entities;

public class Migration
{
    public int id { get; private set; }
    public string migration { get; private set; } = string.Empty;
    public int batch { get; private set; }
}

