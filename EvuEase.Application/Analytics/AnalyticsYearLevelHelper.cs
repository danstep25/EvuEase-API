using System.Globalization;

namespace EvuEase.Application.Analytics;

public static class AnalyticsYearLevelHelper
{
    public static bool MatchesYearLevelFilter(string studentYearLevel, string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter) || filter.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return NormalizeYearLevelLabel(studentYearLevel)
            .Equals(NormalizeYearLevelLabel(filter), StringComparison.OrdinalIgnoreCase);
    }

    public static string NormalizeYearLevelLabel(string? yearLevel)
    {
        if (string.IsNullOrWhiteSpace(yearLevel))
        {
            return "Unknown";
        }

        var t = yearLevel.Trim().ToLowerInvariant();
        if (t is "1" or "1st year" or "first year" or "year 1" or "1st")
        {
            return "1st Year";
        }

        if (t is "2" or "2nd year" or "second year" or "year 2" or "2nd")
        {
            return "2nd Year";
        }

        if (t is "3" or "3rd year" or "third year" or "year 3" or "3rd")
        {
            return "3rd Year";
        }

        if (t is "4" or "4th year" or "fourth year" or "year 4" or "4th")
        {
            return "4th Year";
        }

        if (t is "5" or "5th year" or "fifth year" or "year 5" or "5th")
        {
            return "5th Year";
        }

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(yearLevel.Trim());
    }
}
