namespace EvuEase.Application.Common;

public static class CurriculumStatusHelper
{
    public const string Active = "Active";

    public static bool IsActive(string? status)
    {
        return string.Equals(status?.Trim(), Active, StringComparison.OrdinalIgnoreCase);
    }
}
