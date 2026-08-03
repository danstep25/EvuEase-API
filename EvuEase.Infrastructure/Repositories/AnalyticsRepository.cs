using EvuEase.Application.Analytics;
using EvuEase.Application.DTOs.Analytics;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly AppDbContext _dbContext;

    public AnalyticsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> CountStudentsAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = BuildStudentQuery(request);
        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AnalyticsCountLabelDto>> GetStudentsByProgramAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var students = await BuildStudentQuery(request)
            .Select(s => new { s.program_code })
            .ToListAsync(cancellationToken);

        return students
            .GroupBy(s => string.IsNullOrWhiteSpace(s.program_code) ? "Unknown" : s.program_code.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new AnalyticsCountLabelDto { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();
    }

    public async Task<IReadOnlyList<AnalyticsCountLabelDto>> GetStudentsByYearLevelAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var students = await BuildStudentQuery(request)
            .Select(s => new { s.year_level })
            .ToListAsync(cancellationToken);

        return students
            .GroupBy(s => AnalyticsYearLevelHelper.NormalizeYearLevelLabel(s.year_level))
            .Select(g => new AnalyticsCountLabelDto { Label = g.Key, Count = g.Count() })
            .OrderBy(x => x.Label)
            .ToList();
    }

    public async Task<IReadOnlyList<EnrollmentAnalyticsRow>> GetEnrollmentRowsAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var raw = await (
                from e in _dbContext.FacultyClassEnrollments.AsNoTracking()
                join s in _dbContext.Students.AsNoTracking() on e.student_id equals s.id
                join fc in _dbContext.FacultyClasses.AsNoTracking() on e.faculty_class_id equals fc.id
                join prog in _dbContext.Programs.AsNoTracking() on fc.program_code equals prog.program_code
                where s.status && s.deleted_at == null
                      && e.status && e.deleted_at == null
                      && fc.status && fc.deleted_at == null
                      && prog.status && prog.deleted_at == null
                      && (string.IsNullOrWhiteSpace(request.ProgramCode)
                          || request.ProgramCode == "all"
                          || s.program_code == request.ProgramCode)
                      && (string.IsNullOrWhiteSpace(request.SchoolYear)
                          || request.SchoolYear == "all"
                          || fc.academic_term.Contains(request.SchoolYear))
                let units = _dbContext.Courses.AsNoTracking()
                    .Where(c => c.course_code == fc.course_code
                                && c.program_id == prog.program_id
                                && c.status
                                && c.deleted_at == null)
                    .Select(c => (int?)c.course_total_units)
                    .FirstOrDefault() ?? 0
                select new
                {
                    s.id,
                    s.program_code,
                    s.year_level,
                    fc.course_code,
                    fc.academic_term,
                    units,
                    e.official_grade,
                    e.remarks,
                    e.created_at,
                    e.updated_at
                })
            .ToListAsync(cancellationToken);

        return raw
            .Where(x => AnalyticsYearLevelHelper.MatchesYearLevelFilter(x.year_level, request.YearLevel))
            .Select(x => new EnrollmentAnalyticsRow
            {
                StudentId = x.id,
                ProgramCode = x.program_code,
                YearLevel = x.year_level,
                CourseCode = x.course_code,
                AcademicTerm = x.academic_term,
                Units = x.units,
                OfficialGrade = x.official_grade,
                StoredRemarks = x.remarks,
                CreatedAt = x.created_at,
                UpdatedAt = x.updated_at
            })
            .ToList();
    }

    public async Task<IReadOnlyList<(DateTime Timestamp, string Module, string Action)>> GetChargeSlipLogEventsAsync(
        AnalyticsDashboardRequest request,
        CancellationToken cancellationToken = default)
    {
        var logs = await _dbContext.SystemLogs.AsNoTracking()
            .Where(l =>
                (EF.Functions.Like(l.module, "%Subject%")
                 || EF.Functions.Like(l.module, "%Evaluation%")
                 || EF.Functions.Like(l.details, "%charge slip%")
                 || EF.Functions.Like(l.details, "%Charge Slip%"))
                && (EF.Functions.Like(l.action, "%Print%")
                    || EF.Functions.Like(l.action, "%Export%")
                    || EF.Functions.Like(l.details, "%charge slip%")
                    || EF.Functions.Like(l.details, "%Charge Slip%")))
            .Select(l => new { l.timestamp, l.module, l.action })
            .ToListAsync(cancellationToken);

        return logs
            .Select(l => (l.timestamp, l.module, l.action))
            .ToList();
    }

    private IQueryable<Domain.Entities.Student> BuildStudentQuery(AnalyticsDashboardRequest request)
    {
        var query = _dbContext.Students.AsNoTracking()
            .Where(s => s.status && s.deleted_at == null && s.enrollment_status.ToLower() == "active");

        if (!string.IsNullOrWhiteSpace(request.ProgramCode) && !request.ProgramCode.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(s => s.program_code == request.ProgramCode);
        }

        if (!string.IsNullOrWhiteSpace(request.SchoolYear) && !request.SchoolYear.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(s =>
                _dbContext.FacultyClassEnrollments.Any(e =>
                    e.student_id == s.id
                    && e.status
                    && e.deleted_at == null
                    && _dbContext.FacultyClasses.Any(fc =>
                        fc.id == e.faculty_class_id
                        && fc.status
                        && fc.deleted_at == null
                        && fc.academic_term.Contains(request.SchoolYear))));
        }

        if (!string.IsNullOrWhiteSpace(request.YearLevel) && !request.YearLevel.Equals("all", StringComparison.OrdinalIgnoreCase))
        {
            var normalized = AnalyticsYearLevelHelper.NormalizeYearLevelLabel(request.YearLevel);
            query = query.Where(s =>
                s.year_level == request.YearLevel
                || s.year_level == normalized
                || (request.YearLevel == "1" && (s.year_level == "1st Year" || s.year_level == "First Year"))
                || (request.YearLevel == "2" && (s.year_level == "2nd Year" || s.year_level == "Second Year"))
                || (request.YearLevel == "3" && (s.year_level == "3rd Year" || s.year_level == "Third Year"))
                || (request.YearLevel == "4" && (s.year_level == "4th Year" || s.year_level == "Fourth Year")));
        }

        return query;
    }
}
