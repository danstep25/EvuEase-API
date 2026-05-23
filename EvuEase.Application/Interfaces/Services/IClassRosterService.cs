using EvuEase.Application.DTOs.ClassRoster;
using EvuEase.Application.DTOs.GradeRoster;

namespace EvuEase.Application.Interfaces.Services;

public interface IClassRosterService
{
    Task<IReadOnlyList<ClassRosterResponse>> GetClassesAsync(string? search, CancellationToken cancellationToken = default);

    
    
    
    Task<IReadOnlyList<GradeRosterClassLookupResponse>> GetGradeRosterClassLookupAsync(
        string academicTerm,
        string? search,
        CancellationToken cancellationToken = default);

    Task<ClassRosterResponse> CreateClassAsync(CreateClassRosterRequest request, CancellationToken cancellationToken = default);

    Task DeleteClassAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ClassRosterStudentResponse>> GetStudentsForClassAsync(long facultyClassId, CancellationToken cancellationToken = default);

    Task<ClassRosterStudentResponse> AddStudentToClassAsync(long facultyClassId, long studentId, CancellationToken cancellationToken = default);

    Task RemoveStudentFromClassAsync(long facultyClassId, long enrollmentId, CancellationToken cancellationToken = default);

    Task<ClassRosterStudentResponse> UpdateEnrollmentGradeAsync(
        long enrollmentId,
        UpdateEnrollmentGradeRequest request,
        CancellationToken cancellationToken = default);

    Task<ClassRosterBatchUploadResponse> BatchUploadRosterPdfAsync(long facultyClassId, Stream pdfStream, CancellationToken cancellationToken = default);

    
    
    
    
    Task<ClassRosterPdfImportSummaryResponse> ImportClassRosterPdfAsync(
        Stream pdfStream,
        IReadOnlyCollection<string>? includedRowKeys = null,
        CancellationToken cancellationToken = default);

    
    
    
    Task<ClassListPdfPreviewResponse> PreviewClassListPdfAsync(Stream pdfStream, CancellationToken cancellationToken = default);
}
