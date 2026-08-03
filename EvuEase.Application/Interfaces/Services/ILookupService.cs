using EvuEase.Application.DTOs;
using EvuEase.Application.DTOs.GradeRoster;

namespace EvuEase.Application.Interfaces.Services;

public interface ILookupService
{
    Task<List<LookupResponse>> GetModuleLookupAsync();
    Task<List<LookupResponse>> GetProgramsLookupAsync();
    Task<List<LookupResponse>> GetSyTermsLookupAsync();
    Task<List<LookupResponse>> GetCurriculaLookupAsync(long? programId = null, bool activeOnly = false);
    Task<List<LookupResponse>> GetCurriculumVersionsLookupAsync(string programCode, bool activeOnly = false);
    Task<List<LookupResponse>> GetCoursesLookupAsync();

    
    
    
    Task<IReadOnlyList<GradeRosterClassLookupResponse>> GetGradeRosterClassLookupAsync(
        string academicTerm,
        string? search,
        CancellationToken cancellationToken = default);
}

