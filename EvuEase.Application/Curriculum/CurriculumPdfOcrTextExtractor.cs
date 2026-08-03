using System.Text;
using Tesseract;
using UglyToad.PdfPig;

namespace EvuEase.Application.Curriculum;

public static class CurriculumPdfOcrTextExtractor
{
    private static readonly object EngineLock = new();

    public static bool IsAvailable(string? tessDataPath = null)
    {
        return ResolveTessDataPath(tessDataPath) != null;
    }

    public static string? ExtractText(Stream pdfStream, string? tessDataPath = null)
    {
        var dataPath = ResolveTessDataPath(tessDataPath);
        if (dataPath == null)
        {
            return null;
        }

        using var copy = new MemoryStream();
        pdfStream.CopyTo(copy);
        copy.Position = 0;

        using var document = PdfDocument.Open(copy);
        var sb = new StringBuilder();

        lock (EngineLock)
        {
            using var engine = new TesseractEngine(dataPath, "eng", EngineMode.Default);
            engine.DefaultPageSegMode = PageSegMode.Auto;

            foreach (var page in document.GetPages())
            {
                foreach (var image in page.GetImages())
                {
                    var bytes = image.RawBytes.ToArray();
                    if (bytes.Length == 0)
                    {
                        continue;
                    }

                    using var pix = Pix.LoadFromMemory(bytes);
                    using var ocrPage = engine.Process(pix);
                    var text = ocrPage.GetText();
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        sb.AppendLine(text);
                    }
                }
            }
        }

        var result = sb.ToString().Trim();
        return result.Length > 0 ? result : null;
    }

    public static string? ResolveTessDataPath(string? overridePath = null)
    {
        if (!string.IsNullOrWhiteSpace(overridePath)
            && File.Exists(Path.Combine(overridePath, "eng.traineddata")))
        {
            return overridePath;
        }

        foreach (var candidate in CandidateTessDataPaths())
        {
            if (File.Exists(Path.Combine(candidate, "eng.traineddata")))
            {
                return candidate;
            }
        }

        return null;
    }

    private static IEnumerable<string> CandidateTessDataPaths()
    {
        yield return Path.Combine(AppContext.BaseDirectory, "tessdata");

        var current = Directory.GetCurrentDirectory();
        if (!string.IsNullOrWhiteSpace(current))
        {
            yield return Path.Combine(current, "tessdata");
        }
    }
}
