using System.Globalization;
using EvuEase.Application.DTOs.Student;

namespace EvuEase.Application.Common;

/// <summary>
/// Determines whether a student has passed every required curriculum subject.
/// Mirrors the frontend <c>mergeCurriculumWithEnrollments</c> + <c>isCandidateForGraduation</c> rules:
/// elective options are excluded; the latest enrollment per course code wins.
/// </summary>
public static class StudentGraduationEligibilityHelper
{
    public static bool IsCandidateForGraduation(
        IEnumerable<GraduationRequiredCourse> requiredCourses,
        IReadOnlyList<StudentClassEnrollmentRowDto> enrollments)
    {
        var required = requiredCourses.Where(c => !c.IsElectiveOption).ToList();
        if (required.Count == 0)
        {
            return false;
        }

        var latestByCode = BuildLatestEnrollmentByCourseCode(enrollments);

        foreach (var course in required)
        {
            var key = NormalizeCode(course.CourseCode);
            if (string.IsNullOrEmpty(key)
                || !latestByCode.TryGetValue(key, out var row)
                || !IsPassedEnrollment(row))
            {
                return false;
            }
        }

        return true;
    }

    private static Dictionary<string, StudentClassEnrollmentRowDto> BuildLatestEnrollmentByCourseCode(
        IReadOnlyList<StudentClassEnrollmentRowDto> enrollments)
    {
        var sorted = enrollments
            .OrderBy(e => e.AcademicTerm ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var map = new Dictionary<string, StudentClassEnrollmentRowDto>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in sorted)
        {
            var key = NormalizeCode(row.CourseCode);
            if (!string.IsNullOrEmpty(key))
            {
                map[key] = row;
            }
        }

        return map;
    }

    private static bool IsPassedEnrollment(StudentClassEnrollmentRowDto row)
    {
        if (string.IsNullOrWhiteSpace(row.OfficialGrade))
        {
            return false;
        }

        var remarks = row.Remarks?.Trim() ?? string.Empty;
        if (remarks.Equals("Passed", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (remarks.Equals("Failed", StringComparison.OrdinalIgnoreCase)
            || remarks.Equals("Incomplete", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!decimal.TryParse(
                row.OfficialGrade.Trim(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var grade))
        {
            return false;
        }

        return grade > 0 && grade <= 3.0m;
    }

    private static string NormalizeCode(string? code) =>
        code?.Trim().ToLowerInvariant().Replace(" ", string.Empty, StringComparison.Ordinal) ?? string.Empty;

    public readonly record struct GraduationRequiredCourse(string CourseCode, bool IsElectiveOption);
}
