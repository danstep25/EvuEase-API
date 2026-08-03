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

        // GWA = Total Credit Points (grade x units, every occurrence counted
        // individually including retakes and failures) / Total Units, rounded to
        // two decimal places.
        decimal totalCreditPoints = 0;
        int totalUnits = 0;
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

            totalCreditPoints += g * u;
            totalUnits += u;

            if (g <= 3.0m)
            {
                completedUnits += u;
            }
            else
            {
                summary.FailedSubjects++;
            }
        }

        summary.TotalUnitsCompleted = completedUnits;
        if (totalUnits > 0)
        {
            summary.CumulativeGpa = Math.Round(totalCreditPoints / totalUnits, 2, MidpointRounding.AwayFromZero);
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
