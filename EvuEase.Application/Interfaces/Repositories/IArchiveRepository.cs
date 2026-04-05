using EvuEase.Application.DTOs.Archive;

namespace EvuEase.Application.Interfaces.Repositories;

public interface IArchiveRepository
{
    Task<IReadOnlyList<ArchivedProgramDto>> ListArchivedProgramsAsync(ArchiveProgramListQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArchivedStudentDto>> ListArchivedStudentsAsync(ArchiveStudentListQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArchivedSchoolYearDto>> ListArchivedSchoolYearsAsync(ArchiveSchoolYearListQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArchivedCurriculumDto>> ListArchivedCurriculaAsync(ArchiveListQuery query, CancellationToken cancellationToken = default);

    Task RestoreProgramAsync(long id, CancellationToken cancellationToken = default);
    Task RestoreStudentAsync(long id, CancellationToken cancellationToken = default);
    Task RestoreSchoolYearAsync(long id, CancellationToken cancellationToken = default);
    Task RestoreCurriculumAsync(long id, CancellationToken cancellationToken = default);

    Task PermanentDeleteProgramAsync(long id, CancellationToken cancellationToken = default);
    Task PermanentDeleteStudentAsync(long id, CancellationToken cancellationToken = default);
    Task PermanentDeleteSchoolYearAsync(long id, CancellationToken cancellationToken = default);
    Task PermanentDeleteCurriculumAsync(long id, CancellationToken cancellationToken = default);
}
