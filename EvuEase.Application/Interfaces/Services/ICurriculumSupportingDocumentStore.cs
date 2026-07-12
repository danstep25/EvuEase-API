namespace EvuEase.Application.Interfaces.Services;

public interface ICurriculumSupportingDocumentStore
{
    Task<(string StorageKey, string FileName)> SaveAsync(
        long curriculumId,
        Stream content,
        string originalFileName,
        CancellationToken cancellationToken = default);

    Task<Stream?> OpenAsync(string storageKey, CancellationToken cancellationToken = default);

    Task DeleteAsync(string? storageKey, CancellationToken cancellationToken = default);
}
