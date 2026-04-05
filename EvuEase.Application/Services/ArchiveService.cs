using EvuEase.Application.DTOs.Archive;
using EvuEase.Application.Interfaces.Repositories;
using EvuEase.Application.Interfaces.Services;

namespace EvuEase.Application.Services;

public class ArchiveService : IArchiveService
{
    private readonly IArchiveRepository _archiveRepository;

    public ArchiveService(IArchiveRepository archiveRepository)
    {
        _archiveRepository = archiveRepository;
    }

    public Task<IReadOnlyList<ArchivedProgramDto>> ListProgramsAsync(ArchiveProgramListQuery query, CancellationToken cancellationToken = default) =>
        _archiveRepository.ListArchivedProgramsAsync(query, cancellationToken);

    public Task<IReadOnlyList<ArchivedStudentDto>> ListStudentsAsync(ArchiveStudentListQuery query, CancellationToken cancellationToken = default) =>
        _archiveRepository.ListArchivedStudentsAsync(query, cancellationToken);

    public Task<IReadOnlyList<ArchivedSchoolYearDto>> ListSchoolYearsAsync(ArchiveSchoolYearListQuery query, CancellationToken cancellationToken = default) =>
        _archiveRepository.ListArchivedSchoolYearsAsync(query, cancellationToken);

    public Task<IReadOnlyList<ArchivedCurriculumDto>> ListCurriculaAsync(ArchiveListQuery query, CancellationToken cancellationToken = default) =>
        _archiveRepository.ListArchivedCurriculaAsync(query, cancellationToken);

    public Task RestoreProgramAsync(long id, CancellationToken cancellationToken = default) =>
        _archiveRepository.RestoreProgramAsync(id, cancellationToken);

    public Task RestoreStudentAsync(long id, CancellationToken cancellationToken = default) =>
        _archiveRepository.RestoreStudentAsync(id, cancellationToken);

    public Task RestoreSchoolYearAsync(long id, CancellationToken cancellationToken = default) =>
        _archiveRepository.RestoreSchoolYearAsync(id, cancellationToken);

    public Task RestoreCurriculumAsync(long id, CancellationToken cancellationToken = default) =>
        _archiveRepository.RestoreCurriculumAsync(id, cancellationToken);

    public Task PermanentDeleteProgramAsync(long id, CancellationToken cancellationToken = default) =>
        _archiveRepository.PermanentDeleteProgramAsync(id, cancellationToken);

    public Task PermanentDeleteStudentAsync(long id, CancellationToken cancellationToken = default) =>
        _archiveRepository.PermanentDeleteStudentAsync(id, cancellationToken);

    public Task PermanentDeleteSchoolYearAsync(long id, CancellationToken cancellationToken = default) =>
        _archiveRepository.PermanentDeleteSchoolYearAsync(id, cancellationToken);

    public Task PermanentDeleteCurriculumAsync(long id, CancellationToken cancellationToken = default) =>
        _archiveRepository.PermanentDeleteCurriculumAsync(id, cancellationToken);
}
