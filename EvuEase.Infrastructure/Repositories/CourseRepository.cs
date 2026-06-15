using EvuEase.Application.Common;
using EvuEase.Application.DTOs;
using EvuEase.Application.DTOs.Course;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Domain.Entities;
using EvuEase.Infrastructure.Common;
using EvuEase.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EvuEase.Infrastructure.Repositories;

public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        : base(dbContext, httpContextAccessor) { }

    public async Task<PagedResults<Course>> GetAllCourses(CourseRequest courseRequest)
    {
        var query = GetAll();

        if (!string.IsNullOrWhiteSpace(courseRequest.SearchTerm))
        {
            var searchTerm = courseRequest.SearchTerm.ToLower();
            query = query.Where(c =>
                c.course_code.ToLower().Contains(searchTerm) ||
                c.course_title.ToLower().Contains(searchTerm)
            );
        }

        if (!string.IsNullOrWhiteSpace(courseRequest.CourseCode))
        {
            query = query.Where(c => c.course_code.ToLower().Contains(courseRequest.CourseCode.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(courseRequest.CurriculumCode))
        {
            var curriculumCode = courseRequest.CurriculumCode.Trim().ToLower();
            var curriculaQuery = dbContext.Set<Curricula>()
                .Where(cu => cu.curriculum_code.ToLower() == curriculumCode)
                .Select(cu => cu.id);
            query = query.Where(c => curriculaQuery.Contains(c.curriculum_id));
        }

        if (courseRequest.ProgramId.HasValue)
        {
            query = query.Where(c => c.program_id == courseRequest.ProgramId.Value);
        }

        if (!string.IsNullOrWhiteSpace(courseRequest.CourseTitle))
        {
            query = query.Where(c => c.course_title.ToLower().Contains(courseRequest.CourseTitle.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(courseRequest.YearLevel))
        {
            query = query.Where(c => c.course_yearlevel.ToLower() == courseRequest.YearLevel.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(courseRequest.Semester))
        {
            query = query.Where(c => c.course_semester.ToLower() == courseRequest.Semester.ToLower());
        }

        if (!string.IsNullOrWhiteSpace(courseRequest.Status))
        {
            query = query.Where(c => c.status.ToString().ToLower() == courseRequest.Status.ToLower());
        }

        if (courseRequest.HasPrerequisites.HasValue)
        {
            if (courseRequest.HasPrerequisites.Value)
            {
                query = query.Where(c =>
                    c.course_has_prerequities == 1 &&
                    c.prerequisites != null &&
                    c.prerequisites != "" &&
                    c.prerequisites.ToLower() != "none" &&
                    c.prerequisites.ToLower() != "n/a");
            }
            else
            {
                query = query.Where(c =>
                    c.course_has_prerequities == 0 ||
                    c.prerequisites == null ||
                    c.prerequisites == "" ||
                    c.prerequisites.ToLower() == "none" ||
                    c.prerequisites.ToLower() == "n/a");
            }
        }

        return await query.PaginateAsync(
            courseRequest.PageIndex,
            courseRequest.PageSize,
            courseRequest.SortKey,
            courseRequest.SortDirection
        );
    }

    public async Task<Course?> GetCourseByCodeAsync(string courseCode)
    {
        return await GetAll()
            .FirstOrDefaultAsync(c => c.course_code == courseCode);
    }

    public async Task<Course> CreateCourseAsync(Course course)
    {
        await AddAsync(course);
        await SaveChangesAsync();
        return course;
    }

    public async Task<Course> UpdateCourseAsync(Course course)
    {
        await UpdateAsync(course);
        await SaveChangesAsync();
        return course;
    }

    public async Task DeleteCourseAsync(Course course)
    {
        await SoftDeleteAsync(course);
        await SaveChangesAsync();
    }

    public async Task<List<LookupItem>> GetLookupItemsAsync()
    {
        return await GetAll()
            .Select(c => new LookupItem
            {
                Id = 0,
                Value = c.course_code,
                DisplayText = c.course_code + " - " + c.course_title
            })
            .OrderBy(c => c.Value)
            .ToListAsync();
    }

    public async Task<HashSet<string>> GetExistingCourseCodesAsync(
        IEnumerable<string> courseCodes,
        CancellationToken cancellationToken = default)
    {
        var normalized = courseCodes
            .Select(c => c.Trim())
            .Where(c => c.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalized.Count == 0)
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        var lowerSet = normalized.Select(c => c.ToLower()).ToList();
        var matches = await GetAll()
            .Where(c => lowerSet.Contains(c.course_code.ToLower()))
            .Select(c => c.course_code)
            .ToListAsync(cancellationToken);

        return new HashSet<string>(matches, StringComparer.OrdinalIgnoreCase);
    }

    public async Task SoftDeleteAllForProgramAsync(long programId, CancellationToken cancellationToken = default)
    {
        var list = await GetAll()
            .Where(c => c.program_id == programId)
            .ToListAsync(cancellationToken);
        foreach (var course in list)
        {
            await SoftDeleteAsync(course, cancellationToken);
        }

        await SaveChangesAsync(cancellationToken);
    }
}

