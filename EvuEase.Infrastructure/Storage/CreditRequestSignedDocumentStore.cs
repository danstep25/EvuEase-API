using EvuEase.Application.Interfaces.Services;
using Microsoft.Extensions.Hosting;

namespace EvuEase.Infrastructure.Storage;

public sealed class CreditRequestSignedDocumentStore : ICreditRequestSignedDocumentStore
{
    private readonly string _rootPath;

    public CreditRequestSignedDocumentStore(IHostEnvironment environment)
    {
        _rootPath = Path.Combine(environment.ContentRootPath, "App_Data", "credit-request-documents");
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<(string StorageKey, string FileName)> SaveSignedPdfAsync(
        long creditRequestId,
        Stream content,
        string originalFileName,
        CancellationToken cancellationToken = default)
    {
        var safeName = SanitizeFileName(originalFileName);
        var storageKey = Path.Combine(creditRequestId.ToString(), $"{Guid.NewGuid():N}_{safeName}");
        var fullPath = GetFullPath(storageKey);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        await using var fileStream = new FileStream(
            fullPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);
        await content.CopyToAsync(fileStream, cancellationToken);

        return (NormalizeStorageKey(storageKey), safeName);
    }

    public Task<Stream?> OpenSignedPdfAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteSignedPdfAsync(string? storageKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            return Task.CompletedTask;
        }

        var fullPath = GetFullPath(storageKey);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    private string GetFullPath(string storageKey)
    {
        var normalized = NormalizeStorageKey(storageKey);
        var combined = Path.GetFullPath(Path.Combine(_rootPath, normalized));
        if (!combined.StartsWith(_rootPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Invalid storage key.");
        }

        return combined;
    }

    private static string NormalizeStorageKey(string storageKey)
    {
        return storageKey.Replace('\\', '/').TrimStart('/');
    }

    private static string SanitizeFileName(string fileName)
    {
        var name = Path.GetFileName(fileName.Trim());
        if (string.IsNullOrWhiteSpace(name))
        {
            return "signed-credit-request.pdf";
        }

        foreach (var invalid in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalid, '_');
        }

        return name;
    }
}
