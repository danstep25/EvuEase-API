namespace EvuEase.Application.DTOs.Analytics;

public class AnalyticsDashboardResponse
{
    public AnalyticsKpiDto Kpis { get; set; } = new();
    public IReadOnlyList<AnalyticsMonthlyTrendPointDto> MonthlyTrend { get; set; } = Array.Empty<AnalyticsMonthlyTrendPointDto>();
    public AnalyticsSubjectStatusDto SubjectStatus { get; set; } = new();
    public IReadOnlyList<AnalyticsCountLabelDto> StudentsByProgram { get; set; } = Array.Empty<AnalyticsCountLabelDto>();
    public IReadOnlyList<AnalyticsCountLabelDto> StudentsByYearLevel { get; set; } = Array.Empty<AnalyticsCountLabelDto>();
    public IReadOnlyList<AnalyticsCountLabelDto> UnitLoadDistribution { get; set; } = Array.Empty<AnalyticsCountLabelDto>();
    public IReadOnlyList<AnalyticsCourseFailureRateDto> TopFailedCourses { get; set; } = Array.Empty<AnalyticsCourseFailureRateDto>();
    public AnalyticsQuickStatisticsDto QuickStatistics { get; set; } = new();
}

public class AnalyticsKpiDto
{
    public int TotalStudents { get; set; }
    public int TotalEvaluations { get; set; }
    public decimal CompletionRatePercent { get; set; }
    public int FailedSubjects { get; set; }
    public decimal AverageUnitsPerStudent { get; set; }
}

public class AnalyticsMonthlyTrendPointDto
{
    public string MonthLabel { get; set; } = string.Empty;
    public int Evaluations { get; set; }
    public int ChargeSlips { get; set; }
}

public class AnalyticsSubjectStatusDto
{
    public int Passed { get; set; }
    public int Failed { get; set; }
    public int InProgress { get; set; }
}

public class AnalyticsCountLabelDto
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class AnalyticsCourseFailureRateDto
{
    public string CourseCode { get; set; } = string.Empty;
    public int Failed { get; set; }
    public int Total { get; set; }
    public decimal RatePercent { get; set; }
}

public class AnalyticsQuickStatisticsDto
{
    public decimal PassRatePercent { get; set; }
    public decimal FailureRatePercent { get; set; }
    public decimal AverageSubjectsPerStudent { get; set; }
}
