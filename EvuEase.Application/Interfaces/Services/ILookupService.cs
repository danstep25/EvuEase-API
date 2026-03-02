using EvuEase.Application.DTOs;

namespace EvuEase.Application.Interfaces.Services;

public interface ILookupService
{
    Task<List<LookupResponse>> GetModuleLookupAsync();
    Task<List<LookupResponse>> GetProgramsLookupAsync();
    Task<List<LookupResponse>> GetSyTermsLookupAsync();
    Task<List<LookupResponse>> GetCurriculaLookupAsync(long? programId = null);
    Task<List<LookupResponse>> GetCurriculumVersionsLookupAsync(string programCode);
    Task<List<LookupResponse>> GetCoursesLookupAsync();
}

