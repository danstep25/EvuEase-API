using System.Globalization;
using EvuEase.Application.ClassRoster;
using EvuEase.Application.DTOs.Analytics;

namespace EvuEase.Application.Analytics;

internal static class AnalyticsDashboardComputer
{
    private static readonly string[] UnitLoadBuckets =
    [
        "0-15 units",
        "16-20 units",
        "21-26 units",
        ">26 units"
    ];

    private static readonly string[] YearLevelOrder =
    [
        "1st Year",
        "2nd Year",
        "3rd Year",
        "4th Year",
        "5th Year",
        "Unknown"
    ];

    public static AnalyticsDashboardResponse Compute(
        int totalStudents,
        IReadOnlyList<EnrollmentAnalyticsRow> enrollments,
        IReadOnlyList<AnalyticsCountLabelDto> studentsByProgram,
        IReadOnlyList<AnalyticsCountLabelDto> studentsByYearLevel,
        IReadOnlyList<(DateTime Timestamp, string Module, string Action)> chargeSlipLogs)
    {
        var rows = enrollments;
        var graded = rows.Where(HasOfficialGrade).ToList();
        var totalEvaluations = rows.Count;
        var completed = graded.Count;
        var completionRate = totalEvaluations > 0
            ? Math.Round((decimal)completed / totalEvaluations * 100m, 1, MidpointRounding.AwayFromZero)
            : 0m;

        var failedCount = rows.Count(r => IsFailed(r));
        var averageUnits = ComputeAverageUnitsPerStudent(rows);

        var monthlyTrend = BuildMonthlyTrend(rows, chargeSlipLogs);
        var subjectStatus = BuildSubjectStatus(rows);
        var unitLoad = BuildUnitLoadDistribution(rows);
        var topFailed = BuildTopFailedCourses(rows);
        var quick = BuildQuickStatistics(rows);

        return new AnalyticsDashboardResponse
        {
            Kpis = new AnalyticsKpiDto
            {
                TotalStudents = totalStudents,
                TotalEvaluations = totalEvaluations,
                CompletionRatePercent = completionRate,
                FailedSubjects = failedCount,
                AverageUnitsPerStudent = averageUnits
            },
            MonthlyTrend = monthlyTrend,
            SubjectStatus = subjectStatus,
            StudentsByProgram = studentsByProgram,
            StudentsByYearLevel = studentsByYearLevel,
            UnitLoadDistribution = unitLoad,
            TopFailedCourses = topFailed,
            QuickStatistics = quick
        };
    }

    private static decimal ComputeAverageUnitsPerStudent(IReadOnlyList<EnrollmentAnalyticsRow> rows)
    {
        if (rows.Count == 0)
        {
            return 0m;
        }

        var latestTerm = rows
            .Select(r => r.AcademicTerm)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .OrderByDescending(t => t, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        var scoped = string.IsNullOrWhiteSpace(latestTerm)
            ? rows
            : rows.Where(r => r.AcademicTerm == latestTerm).ToList();

        var perStudent = scoped
            .GroupBy(r => r.StudentId)
            .Select(g => g.Sum(x => Math.Max(0, x.Units)))
            .ToList();

        if (perStudent.Count == 0)
        {
            return 0m;
        }

        return Math.Round((decimal)perStudent.Average(), 1, MidpointRounding.AwayFromZero);
    }

    private static IReadOnlyList<AnalyticsMonthlyTrendPointDto> BuildMonthlyTrend(
        IReadOnlyList<EnrollmentAnalyticsRow> rows,
        IReadOnlyList<(DateTime Timestamp, string Module, string Action)> chargeSlipLogs)
    {
        var now = DateTime.Now;
        var points = new List<AnalyticsMonthlyTrendPointDto>();

        for (var i = 5; i >= 0; i--)
        {
            var monthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-i);
            var monthEnd = monthStart.AddMonths(1);
            var label = monthStart.ToString("MMM", CultureInfo.InvariantCulture);

            var evaluations = rows.Count(r =>
                HasOfficialGrade(r) &&
                r.UpdatedAt.HasValue &&
                r.UpdatedAt.Value >= monthStart &&
                r.UpdatedAt.Value < monthEnd);

            var chargeSlips = chargeSlipLogs.Count(l =>
                l.Timestamp >= monthStart &&
                l.Timestamp < monthEnd);

            if (chargeSlips == 0)
            {
                chargeSlips = rows.Count(r =>
                    r.CreatedAt.HasValue &&
                    r.CreatedAt.Value >= monthStart &&
                    r.CreatedAt.Value < monthEnd);
            }

            points.Add(new AnalyticsMonthlyTrendPointDto
            {
                MonthLabel = label,
                Evaluations = evaluations,
                ChargeSlips = chargeSlips
            });
        }

        return points;
    }

    private static AnalyticsSubjectStatusDto BuildSubjectStatus(IReadOnlyList<EnrollmentAnalyticsRow> rows)
    {
        var passed = 0;
        var failed = 0;
        var inProgress = 0;

        foreach (var row in rows)
        {
            var remark = GradeRosterRemarksHelper.EffectiveRemarks(row.StoredRemarks, row.OfficialGrade);
            if (string.Equals(remark, "Passed", StringComparison.OrdinalIgnoreCase))
            {
                passed++;
            }
            else if (string.Equals(remark, "Failed", StringComparison.OrdinalIgnoreCase))
            {
                failed++;
            }
            else
            {
                inProgress++;
            }
        }

        return new AnalyticsSubjectStatusDto
        {
            Passed = passed,
            Failed = failed,
            InProgress = inProgress
        };
    }

    private static IReadOnlyList<AnalyticsCountLabelDto> BuildUnitLoadDistribution(IReadOnlyList<EnrollmentAnalyticsRow> rows)
    {
        var latestTerm = rows
            .Select(r => r.AcademicTerm)
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .OrderByDescending(t => t, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault();

        var scoped = string.IsNullOrWhiteSpace(latestTerm)
            ? rows
            : rows.Where(r => r.AcademicTerm == latestTerm);

        var studentUnits = scoped
            .GroupBy(r => r.StudentId)
            .Select(g => g.Sum(x => Math.Max(0, x.Units)))
            .ToList();

        var counts = new int[UnitLoadBuckets.Length];
        foreach (var units in studentUnits)
        {
            var idx = units switch
            {
                <= 15 => 0,
                <= 20 => 1,
                <= 26 => 2,
                _ => 3
            };
            counts[idx]++;
        }

        return UnitLoadBuckets
            .Select((label, i) => new AnalyticsCountLabelDto { Label = label, Count = counts[i] })
            .ToList();
    }

    private static IReadOnlyList<AnalyticsCourseFailureRateDto> BuildTopFailedCourses(
        IReadOnlyList<EnrollmentAnalyticsRow> rows)
    {
        return rows
            .Where(r => !string.IsNullOrWhiteSpace(r.CourseCode))
            .GroupBy(r => r.CourseCode.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var total = g.Count();
                var failed = g.Count(IsFailed);
                var rate = total > 0
                    ? Math.Round((decimal)failed / total * 100m, 0, MidpointRounding.AwayFromZero)
                    : 0m;
                return new AnalyticsCourseFailureRateDto
                {
                    CourseCode = g.Key,
                    Failed = failed,
                    Total = total,
                    RatePercent = rate
                };
            })
            .Where(x => x.Total > 0)
            .OrderByDescending(x => x.RatePercent)
            .ThenByDescending(x => x.Failed)
            .Take(10)
            .ToList();
    }

    private static AnalyticsQuickStatisticsDto BuildQuickStatistics(IReadOnlyList<EnrollmentAnalyticsRow> rows)
    {
        var status = BuildSubjectStatus(rows);
        var decided = status.Passed + status.Failed;
        var passRate = decided > 0
            ? Math.Round((decimal)status.Passed / decided * 100m, 0, MidpointRounding.AwayFromZero)
            : 0m;
        var failureRate = decided > 0
            ? Math.Round((decimal)status.Failed / decided * 100m, 0, MidpointRounding.AwayFromZero)
            : 0m;

        var avgSubjects = rows.Count == 0
            ? 0m
            : Math.Round(
                (decimal)rows.GroupBy(r => r.StudentId).Average(g => (double)g.Count()),
                0,
                MidpointRounding.AwayFromZero);

        return new AnalyticsQuickStatisticsDto
        {
            PassRatePercent = passRate,
            FailureRatePercent = failureRate,
            AverageSubjectsPerStudent = avgSubjects
        };
    }

    private static bool HasOfficialGrade(EnrollmentAnalyticsRow row) =>
        !string.IsNullOrWhiteSpace(row.OfficialGrade);

    private static bool IsFailed(EnrollmentAnalyticsRow row)
    {
        var remark = GradeRosterRemarksHelper.EffectiveRemarks(row.StoredRemarks, row.OfficialGrade);
        return string.Equals(remark, "Failed", StringComparison.OrdinalIgnoreCase);
    }
}
