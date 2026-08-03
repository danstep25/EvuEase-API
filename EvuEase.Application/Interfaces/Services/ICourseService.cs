using EvuEase.Application.Common;
using EvuEase.Application.DTOs.Course;

namespace EvuEase.Application.Interfaces.Services;

public interface ICourseService
{
    Task<PagedResults<CourseResponse>> GetAllCourses(CourseRequest courseRequest);
    Task<CourseResponse?> GetCourseByCodeAsync(string courseCode);
    Task<CourseResponse> CreateCourseAsync(CreateCourseRequest courseRequest);
    Task<CourseResponse> UpdateCourseAsync(string courseCode, UpdateCourseRequest courseRequest);
    Task DeleteCourseAsync(string courseCode, string curriculumCode);
    Task<CourseBatchImportPreviewResponse> PreviewBatchImportAsync(
        CourseBatchImportRequest request,
        CancellationToken cancellationToken = default);
    Task<CourseBatchImportPreviewResponse> PreviewBatchPdfAsync(
        Stream pdfStream,
        long programId,
        string curriculumCode,
        CancellationToken cancellationToken = default);
    Task<CourseBatchPdfDetectionResponse> DetectBatchPdfAsync(
        Stream pdfStream,
        CancellationToken cancellationToken = default);
    Task<CourseBatchImportResultResponse> ImportBatchAsync(
        CourseBatchImportRequest request,
        CancellationToken cancellationToken = default);
}




