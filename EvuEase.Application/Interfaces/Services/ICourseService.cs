using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Course;

namespace EvuEase.Application.Interfaces.Services;

public interface ICourseService
{
    Task<PagedResults<CourseResponse>> GetAllCourses(CourseRequest courseRequest);
    Task<CourseResponse?> GetCourseByCodeAsync(string courseCode);
    Task<CourseResponse> CreateCourseAsync(CreateCourseRequest courseRequest);
    Task<CourseResponse> UpdateCourseAsync(string courseCode, UpdateCourseRequest courseRequest);
    Task DeleteCourseAsync(string courseCode);
}




