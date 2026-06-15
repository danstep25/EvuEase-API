namespace EvuEase.Application.Interfaces.Services;

public interface ICreditRequestSignedDocumentStore
{
    Task<(string StorageKey, string FileName)> SaveSignedPdfAsync(
        long creditRequestId,
        Stream content,
        string originalFileName,
        CancellationToken cancellationToken = default);

    Task<Stream?> OpenSignedPdfAsync(string storageKey, CancellationToken cancellationToken = default);

    Task DeleteSignedPdfAsync(string? storageKey, CancellationToken cancellationToken = default);
}
