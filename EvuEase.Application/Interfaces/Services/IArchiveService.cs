using EvuEase.Application.DTOs.Archive;

namespace EvuEase.Application.Interfaces.Services;

public interface IArchiveService
{
    Task<IReadOnlyList<ArchivedProgramDto>> ListProgramsAsync(ArchiveProgramListQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArchivedStudentDto>> ListStudentsAsync(ArchiveStudentListQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArchivedSchoolYearDto>> ListSchoolYearsAsync(ArchiveSchoolYearListQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArchivedCurriculumDto>> ListCurriculaAsync(ArchiveListQuery query, CancellationToken cancellationToken = default);

    Task RestoreProgramAsync(long id, CancellationToken cancellationToken = default);
    Task RestoreStudentAsync(long id, CancellationToken cancellationToken = default);
    Task RestoreSchoolYearAsync(long id, CancellationToken cancellationToken = default);
    Task RestoreCurriculumAsync(long id, CancellationToken cancellationToken = default);

    Task PermanentDeleteProgramAsync(long id, CancellationToken cancellationToken = default);
    Task PermanentDeleteStudentAsync(long id, CancellationToken cancellationToken = default);
    Task PermanentDeleteSchoolYearAsync(long id, CancellationToken cancellationToken = default);
    Task PermanentDeleteCurriculumAsync(long id, CancellationToken cancellationToken = default);
}
