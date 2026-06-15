using System.Text;
using System.Text.RegularExpressions;
using EvuEase.Application.Common;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace EvuEase.Application.Curriculum;

public static class CurriculumStructurePdfParser
{
    private static readonly Regex CourseCodeStart = new(
        @"^[A-Z]{2,12}\d{3,4}\b",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex YearTermSection = new(
        @"(?<year>(?:First|Second|Third|Fourth|Fifth)\s+Year|\d+(?:st|nd|rd|th)?\s+Year)\s*[,:\-–]\s*(?<term>(?:First|Second|Third|Summer)\s+(?:Term|Semester)|(?:1st|2nd|3rd)\s+Semester)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex YearTermInline = new(
        @"(?<year>(?:First|Second|Third|Fourth|Fifth)\s+Year|\d+(?:st|nd|rd|th)?\s+Year).{0,40}?(?<term>(?:First|Second|Third|Summer)\s+(?:Term|Semester)|(?:1st|2nd|3rd)\s+Semester)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex PrerequisiteTail = new(
        @"(?<prereq>(?:[A-Z]{2,12}\d{3,4}\s*(?:;\s*[A-Z]{2,12}\d{3,4}\s*)+|[A-Z]{2,12}\d{3,4}))\s*$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex UnitsTail = new(
        @"^(?<title>.+?)\s+(?<lec>\d{1,2})\s+(?<lab>\d{1,2})\s+(?<total>\d{1,2})\s*$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex UnitsTailTwo = new(
        @"^(?<title>.+?)\s+(?<lec>\d{1,2})\s+(?<total>\d{1,2})\s*$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex OcrCodePrefix = new(
        @"^[@#]\s*",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex SkipLine = new(
        @"^(?:course\s*code|course\s*description|units|lec|lab|total|pre-?requisites?|curriculum\s*structure|school\s*year|certified|approved|contents\s*noted|page\s+\d+|sti\b|college\b|bachelor\b|master\b|total\s*units\b|\*|\-+$)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex ReferenceStrict = new(
        @"^[A-Z]{2,12}-\d{2}-\d{2}$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex ReferenceLine = new(
        @"Reference\s*(?:No\.?|Number)?\s*:?\s*(?<ref>[A-Z]{2,12}-\d{2}-\d{2})\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex ReferenceToken = new(
        @"\b(?<ref>[A-Z]{2,12}-\d{2}-\d{2})\b",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex SchoolYearLine = new(
        @"School\s*Year\s*(?<sy>\d{4}\s*[-–]\s*\d{4})",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public sealed record ParsedCourseRow(
        int SourceLineNumber,
        string CourseCode,
        string CourseTitle,
        int LecUnits,
        int LabUnits,
        int TotalUnits,
        string? Prerequisites,
        string YearLevel,
        string Semester);

    public sealed record ReferenceMetadata(
        string ReferenceNumber,
        string ProgramCode,
        string CurriculumCode,
        string? SchoolYear);

    public sealed class ParseResult
    {
        public List<ParsedCourseRow> Rows { get; } = new();
        public List<string> Warnings { get; } = new();
        public int SkippedLines { get; set; }
        public ReferenceMetadata? Reference { get; set; }
        public bool UsedOcr { get; set; }
    }

    public static ParseResult Parse(Stream pdfStream)
    {
        var extraction = ExtractFullText(pdfStream);
        var result = ParseText(extraction.Text);
        result.UsedOcr = extraction.UsedOcr;
        if (extraction.UsedOcr)
        {
            result.Warnings.Insert(
                0,
                "Scanned PDF detected — course text was recovered using OCR. Review rows carefully before importing.");
        }

        return result;
    }

    public static (string Text, bool UsedOcr) ExtractFullText(Stream pdfStream)
    {
        using var copy = new MemoryStream();
        pdfStream.CopyTo(copy);
        copy.Position = 0;

        using var document = PdfDocument.Open(copy);
        var sb = new StringBuilder();
        foreach (var page in document.GetPages())
        {
            var ordered = ContentOrderTextExtractor.GetText(page);
            if (string.IsNullOrWhiteSpace(ordered))
            {
                ordered = page.Text;
            }

            if (!string.IsNullOrWhiteSpace(ordered))
            {
                sb.AppendLine(ordered);
            }
        }

        var text = sb.ToString();
        if (!string.IsNullOrWhiteSpace(text))
        {
            return (text, false);
        }

        copy.Position = 0;
        var ocrText = CurriculumPdfOcrTextExtractor.ExtractText(copy);
        return (ocrText ?? string.Empty, ocrText != null);
    }

    public static string ExtractFullTextAsString(Stream pdfStream) => ExtractFullText(pdfStream).Text;

    public static ReferenceMetadata? ExtractReferenceMetadata(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var referenceNumber = TryExtractReferenceNumber(text);
        if (string.IsNullOrWhiteSpace(referenceNumber))
        {
            return null;
        }

        var programCode = referenceNumber.Split('-', StringSplitOptions.RemoveEmptyEntries)[0].ToUpperInvariant();
        var schoolYearMatch = SchoolYearLine.Match(text);
        var schoolYear = schoolYearMatch.Success
            ? Regex.Replace(schoolYearMatch.Groups["sy"].Value.Trim(), @"\s+", "-")
            : null;

        return new ReferenceMetadata(
            referenceNumber,
            programCode,
            referenceNumber,
            schoolYear);
    }

    public static ParseResult ParseText(string text)
    {
        var result = new ParseResult();
        if (string.IsNullOrWhiteSpace(text))
        {
            result.Warnings.Add("The PDF contains no readable text.");
            return result;
        }

        result.Reference = ExtractReferenceMetadata(text);

        var currentYear = string.Empty;
        var currentSemester = string.Empty;
        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var lineNumber = 0;

        foreach (var rawLine in text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
        {
            lineNumber++;
            var line = NormalizeLine(rawLine);
            if (line.Length == 0)
            {
                continue;
            }

            if (TryParseYearTermSection(line, out var yearLevel, out var semester))
            {
                currentYear = yearLevel;
                currentSemester = semester;
                continue;
            }

            if (SkipLine.IsMatch(line))
            {
                result.SkippedLines++;
                continue;
            }

            var row = TryParseCourseLine(line, currentYear, currentSemester, lineNumber);
            if (row == null)
            {
                if (CourseCodeStart.IsMatch(line))
                {
                    result.Warnings.Add($"Line {lineNumber}: Could not parse course row \"{Truncate(line, 80)}\".");
                }

                continue;
            }

            if (!seenCodes.Add(row.CourseCode))
            {
                result.Warnings.Add($"Line {lineNumber}: Duplicate course code {row.CourseCode} was skipped.");
                result.SkippedLines++;
                continue;
            }

            if (string.IsNullOrWhiteSpace(row.YearLevel) || string.IsNullOrWhiteSpace(row.Semester))
            {
                result.Warnings.Add(
                    $"Line {lineNumber}: Course {row.CourseCode} has no year/semester section — assign manually or fix the PDF section headers.");
            }

            result.Rows.Add(row);
        }

        if (result.Rows.Count == 0)
        {
            result.Warnings.Add(
                "No course rows were detected. Use the official STI curriculum structure PDF export with selectable text.");
        }

        return result;
    }

    private static string? TryExtractReferenceNumber(string text)
    {
        var labeled = ReferenceLine.Match(text);
        if (labeled.Success)
        {
            return NormalizeReferenceNumber(labeled.Groups["ref"].Value);
        }

        var head = text.Length > 4000 ? text[..4000] : text;
        var token = ReferenceToken.Match(head);
        if (token.Success)
        {
            return NormalizeReferenceNumber(token.Groups["ref"].Value);
        }

        return null;
    }

    private static string? NormalizeReferenceNumber(string raw)
    {
        var normalized = raw.Trim().ToUpperInvariant();
        return ReferenceStrict.IsMatch(normalized) ? normalized : null;
    }

    private static string NormalizeLine(string line)
    {
        var normalized = line.Trim()
            .Replace('|', ' ');
        normalized = OcrCodePrefix.Replace(normalized, string.Empty);
        normalized = Regex.Replace(normalized, @"(\d)\.\s+(?=\d)", "$1 ");
        normalized = Regex.Replace(normalized, @"\s+", " ");
        return normalized.Trim();
    }

    private static bool TryParseYearTermSection(string line, out string yearLevel, out string semester)
    {
        yearLevel = string.Empty;
        semester = string.Empty;

        var match = YearTermSection.Match(line) is { Success: true } direct
            ? direct
            : YearTermInline.Match(line);

        if (!match.Success)
        {
            return false;
        }

        yearLevel = CourseBatchImportHelper.NormalizeYearLevel(match.Groups["year"].Value.Trim());
        semester = CourseBatchImportHelper.NormalizeSemester(match.Groups["term"].Value.Trim());
        return yearLevel.Length > 0 && semester.Length > 0;
    }

    private static ParsedCourseRow? TryParseCourseLine(
        string line,
        string yearLevel,
        string semester,
        int sourceLineNumber)
    {
        var codeMatch = CourseCodeStart.Match(line);
        if (!codeMatch.Success)
        {
            return null;
        }

        var courseCode = codeMatch.Value.ToUpperInvariant();
        var remainder = line[codeMatch.Length..].Trim();

        if (CourseBatchImportHelper.IsSkippableRow(courseCode, remainder))
        {
            return null;
        }

        var prerequisites = ExtractPrerequisites(ref remainder);
        if (!TryParseUnits(remainder, out var title, out var lec, out var lab, out var total))
        {
            return null;
        }

        if (title.Length == 0)
        {
            return null;
        }

        return new ParsedCourseRow(
            sourceLineNumber,
            courseCode,
            title,
            lec,
            lab,
            total,
            prerequisites,
            yearLevel,
            semester);
    }

    private static bool TryParseUnits(
        string remainder,
        out string title,
        out int lec,
        out int lab,
        out int total)
    {
        title = string.Empty;
        lec = lab = total = 0;

        var three = UnitsTail.Match(remainder);
        if (three.Success)
        {
            title = three.Groups["title"].Value.Trim();
            lec = int.Parse(three.Groups["lec"].Value);
            lab = int.Parse(three.Groups["lab"].Value);
            total = int.Parse(three.Groups["total"].Value);
            return true;
        }

        var two = UnitsTailTwo.Match(remainder);
        if (!two.Success)
        {
            return false;
        }

        title = two.Groups["title"].Value.Trim();
        lec = int.Parse(two.Groups["lec"].Value);
        lab = 0;
        total = int.Parse(two.Groups["total"].Value);
        return true;
    }

    private static string? ExtractPrerequisites(ref string remainder)
    {
        var match = PrerequisiteTail.Match(remainder);
        if (!match.Success)
        {
            return null;
        }

        var raw = match.Groups["prereq"].Value.Trim();
        remainder = remainder[..match.Index].Trim();
        return CourseBatchImportHelper.NormalizePrerequisites(raw);
    }

    private static string Truncate(string value, int maxLength)
    {
        if (value.Length <= maxLength)
        {
            return value;
        }

        return value[..maxLength] + "…";
    }
}
