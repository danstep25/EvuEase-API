using System.Text.RegularExpressions;

namespace EvuEase.Application.Common;

public static class CourseBatchImportHelper
{
    private static readonly Regex TotalUnitsRowPattern = new(
        @"^total\s*units?$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static bool IsSkippableRow(string? courseCode, string? courseTitle)
    {
        var code = courseCode?.Trim() ?? string.Empty;
        var title = courseTitle?.Trim() ?? string.Empty;

        if (code.Length == 0 && title.Length == 0)
        {
            return true;
        }

        if (TotalUnitsRowPattern.IsMatch(code) || TotalUnitsRowPattern.IsMatch(title))
        {
            return true;
        }

        return false;
    }

    public static string NormalizeYearLevel(string? raw)
    {
        var value = raw?.Trim() ?? string.Empty;
        if (value.Length == 0)
        {
            return string.Empty;
        }

        return value.ToLowerInvariant() switch
        {
            "first year" or "1st year" or "year 1" or "1" => "Year 1",
            "second year" or "2nd year" or "year 2" or "2" => "Year 2",
            "third year" or "3rd year" or "year 3" or "3" => "Year 3",
            "fourth year" or "4th year" or "year 4" or "4" => "Year 4",
            "fifth year" or "5th year" or "year 5" or "5" => "Year 5",
            _ when value.StartsWith("Year ", StringComparison.OrdinalIgnoreCase) => value,
            _ => value
        };
    }

    public static string NormalizeSemester(string? raw)
    {
        var value = raw?.Trim() ?? string.Empty;
        if (value.Length == 0)
        {
            return string.Empty;
        }

        return value.ToLowerInvariant() switch
        {
            "first term" or "1st term" or "1st semester" or "first semester" or "sem 1" or "semester 1" => "1st Semester",
            "second term" or "2nd term" or "2nd semester" or "second semester" or "sem 2" or "semester 2" => "2nd Semester",
            "summer" or "summer term" => "Summer",
            _ => value
        };
    }

    public static string? NormalizePrerequisites(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var trimmed = raw.Trim();
        if (trimmed.Equals("none", StringComparison.OrdinalIgnoreCase)
            || trimmed.Equals("n/a", StringComparison.OrdinalIgnoreCase)
            || trimmed.Equals("-", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var codes = trimmed
            .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(c => c.Length > 0)
            .Select(c => c.ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return codes.Count == 0 ? null : string.Join("; ", codes);
    }

    public static IReadOnlyList<string> ParsePrerequisiteCodes(string? normalized)
    {
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Array.Empty<string>();
        }

        return normalized
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(c => c.ToUpperInvariant())
            .ToList();
    }

    public static string? DeriveComponent(int lecUnits, int labUnits)
    {
        var hasLec = lecUnits > 0;
        var hasLab = labUnits > 0;

        if (hasLec && hasLab)
        {
            return "Lecture, Lab";
        }

        if (hasLab)
        {
            return "Lab";
        }

        if (hasLec)
        {
            return "Lecture";
        }

        return null;
    }
}
