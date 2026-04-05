using EvuEase.Application.Common;
using EvuEase.Application.DTOs;
using EvuEase.Application.DTOs.Course;
using EvuEase.Domain.Entities;

namespace EvuEase.Application.Interfaces.Repositories;

public interface ICourseRepository
{
    Task<PagedResults<Course>> GetAllCourses(CourseRequest courseRequest);
    Task<Course?> GetCourseByCodeAsync(string courseCode);
    Task<Course> CreateCourseAsync(Course course);
    Task<Course> UpdateCourseAsync(Course course);
    Task DeleteCourseAsync(Course course);
    Task<List<LookupItem>> GetLookupItemsAsync();

    
    Task SoftDeleteAllForProgramAsync(long programId, CancellationToken cancellationToken = default);
}




