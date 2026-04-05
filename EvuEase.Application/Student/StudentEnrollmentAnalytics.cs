using System.Globalization;
using EvuEase.Application.DTOs.Student;

namespace EvuEase.Application.EnrollmentAnalytics;


public static class StudentEnrollmentAnalytics
{
    public static StudentAcademicSummaryDto ComputeSummary(IReadOnlyList<StudentClassEnrollmentRowDto> rows)
    {
        var summary = new StudentAcademicSummaryDto();
        if (rows.Count == 0)
        {
            return summary;
        }

        decimal weightedSum = 0;
        int unitsForGpa = 0;
        var completedUnits = 0;

        foreach (var row in rows)
        {
            if (!TryParseNumericGrade(row.OfficialGrade, out var g))
            {
                continue;
            }

            var u = Math.Max(0, row.Units);
            if (u == 0)
            {
                continue;
            }

            if (g <= 3.0m)
            {
                completedUnits += u;
                weightedSum += g * u;
                unitsForGpa += u;
            }
            else
            {
                summary.FailedSubjects++;
            }
        }

        summary.TotalUnitsCompleted = completedUnits;
        if (unitsForGpa > 0)
        {
            summary.CumulativeGpa = Math.Round(weightedSum / unitsForGpa, 4, MidpointRounding.AwayFromZero);
        }

        var byCode = rows
            .Where(r => !string.IsNullOrWhiteSpace(r.CourseCode))
            .GroupBy(r => r.CourseCode.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToList();
        foreach (var g in byCode)
        {
            if (g.Count() > 1)
            {
                summary.RetakenSubjects += g.Count() - 1;
            }
        }

        return summary;
    }

    private static bool TryParseNumericGrade(string? officialGrade, out decimal value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(officialGrade))
        {
            return false;
        }

        var t = officialGrade.Trim();
        if (t.Length >= 3)
        {
            var u = t.ToUpperInvariant();
            if (u is "INC" or "INCOMPLETE" or "I" or "DRP" or "UD")
            {
                return false;
            }
        }

        return decimal.TryParse(t, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
    }
}
