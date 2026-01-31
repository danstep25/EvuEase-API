namespace EvuEase.Domain.Entities;

public class SystemLog
{
    public long log_id { get; private set; }
    public string user { get; private set; } = string.Empty;
    public string role { get; private set; } = string.Empty;
    public string action { get; private set; } = string.Empty;
    public DateTime timestamp { get; private set; }
    public string module { get; private set; } = string.Empty;
    public string details { get; private set; } = string.Empty;
    public string? ip_address { get; private set; }

    // Private constructor for EF Core
    private SystemLog() { }

    // Factory method for creating new system logs
    public static SystemLog Create(
        string user, 
        string role, 
        string action, 
        string module, 
        string details, 
        string? ip_address = null,
        DateTime? timestamp = null)
    {
        var systemLog = new SystemLog();
        var type = typeof(SystemLog);
        
        type.GetProperty(nameof(user))?.SetValue(systemLog, user);
        type.GetProperty(nameof(role))?.SetValue(systemLog, role);
        type.GetProperty(nameof(action))?.SetValue(systemLog, action);
        type.GetProperty(nameof(module))?.SetValue(systemLog, module);
        type.GetProperty(nameof(details))?.SetValue(systemLog, details);
        type.GetProperty(nameof(ip_address))?.SetValue(systemLog, ip_address);
        type.GetProperty(nameof(timestamp))?.SetValue(systemLog, timestamp ?? DateTime.Now);
        
        return systemLog;
    }
}

