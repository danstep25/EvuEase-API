using EvuEase.Application.Interfaces.Services;
using Microsoft.Extensions.Hosting;

namespace EvuEase.Infrastructure.Storage;

public sealed class CurriculumSupportingDocumentStore : ICurriculumSupportingDocumentStore
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".png", ".jpg", ".jpeg", ".webp", ".doc", ".docx"
    };

    private readonly string _rootPath;

    public CurriculumSupportingDocumentStore(IHostEnvironment environment)
    {
        _rootPath = Path.Combine(environment.ContentRootPath, "App_Data", "curriculum-supporting-documents");
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<(string StorageKey, string FileName)> SaveAsync(
        long curriculumId,
        Stream content,
        string originalFileName,
        CancellationToken cancellationToken = default)
    {
        var safeName = SanitizeFileName(originalFileName);
        ValidateExtension(safeName);

        var storageKey = Path.Combine(curriculumId.ToString(), $"{Guid.NewGuid():N}_{safeName}");
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

    public Task<Stream?> OpenAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var fullPath = GetFullPath(storageKey);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<Stream?>(null);
        }

        Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Task.FromResult<Stream?>(stream);
    }

    public Task DeleteAsync(string? storageKey, CancellationToken cancellationToken = default)
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
            return "curriculum-supporting-document.pdf";
        }

        foreach (var invalid in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalid, '_');
        }

        return name;
    }

    private static void ValidateExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException(
                "Unsupported file type. Allowed types: PDF, PNG, JPG, JPEG, WEBP, DOC, DOCX.");
        }
    }
}
