using System.Globalization;

namespace EvuEase.Application.ClassRoster;


public static class GradeRosterRemarksHelper
{
    
    public static string? EffectiveRemarks(string? storedRemarks, string? officialGrade)
    {
        if (!string.IsNullOrWhiteSpace(storedRemarks))
        {
            return NormalizeRemarkLabel(storedRemarks.Trim());
        }

        return DeriveFromOfficialGrade(officialGrade);
    }

    private static string? NormalizeRemarkLabel(string s)
    {
        if (s.Equals("Passed", StringComparison.OrdinalIgnoreCase))
        {
            return "Passed";
        }

        if (s.Equals("Failed", StringComparison.OrdinalIgnoreCase))
        {
            return "Failed";
        }

        if (s.Equals("Incomplete", StringComparison.OrdinalIgnoreCase))
        {
            return "Incomplete";
        }

        return s;
    }

    
    private static string? DeriveFromOfficialGrade(string? g)
    {
        if (string.IsNullOrWhiteSpace(g))
        {
            return null;
        }

        var t = g.Trim().ToUpperInvariant();
        if (t is "INC" or "INCOMPLETE" or "I")
        {
            return "Incomplete";
        }

        if (decimal.TryParse(g, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
        {
            return d <= 3.0m ? "Passed" : "Failed";
        }

        return null;
    }
}
