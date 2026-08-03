using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace EvuEase.Application.ClassRoster;





public static class ClassListPdfParser
{
    
    private const string StudentNumberPattern = @"\d{8,15}";

    private static readonly Regex EndOfTablePage = new(
        @"^\s*Page\s+\d+\s*/\s*\d+",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex RowIndexThenStudentNo = new(
        @"^\s*(\d{1,4})\s+(\d{8,15})\b",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex StartsWithStudentNo = new(
        @"^\s*(\d{8,15})\b",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex AnyStudentNo = new(
        $@"\b({StudentNumberPattern})\b",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex UnitsToken = new(
        @"^\d+([.,]\d{1,2})?$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex UnitsNormalized = new(
        @"^\d+(\.\d{1,2})?$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex LevelToken = new(
        @"^\d+Y\d+$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex ProgramToken = new(
        @"^[A-Za-z0-9\-]{2,20}$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public sealed record ParsedRow(string StudentNumber, string DisplayName, string ProgramCode, string YearLevel);

    
    public static IReadOnlyList<string> ExtractPageTexts(Stream pdfStream)
    {
        using var copy = new MemoryStream();
        pdfStream.CopyTo(copy);
        copy.Position = 0;
        using var document = PdfDocument.Open(copy);
        var pages = new List<string>();
        foreach (var page in document.GetPages())
        {
            var ordered = ContentOrderTextExtractor.GetText(page);
            if (string.IsNullOrWhiteSpace(ordered))
            {
                ordered = page.Text;
            }

            pages.Add(ordered);
        }

        return pages;
    }

    public static IReadOnlyList<ParsedRow> Parse(Stream pdfStream)
    {
        var sb = new StringBuilder();
        foreach (var pageText in ExtractPageTexts(pdfStream))
        {
            sb.AppendLine(pageText);
        }

        var text = sb.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            return Array.Empty<ParsedRow>();
        }

        return ParseText(text);
    }

    
    public static string MajorityYearLevel(IReadOnlyList<ParsedRow> rows)
    {
        if (rows == null || rows.Count == 0)
        {
            return string.Empty;
        }

        return rows
            .GroupBy(r => r.YearLevel.Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
    }

    public static IReadOnlyList<ParsedRow> ParseText(string text)
    {
        var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
        var result = new List<ParsedRow>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        void TryAddLine(string line)
        {
            var row = TryParseEnrollmentLine(line);
            if (row != null && seen.Add(row.StudentNumber))
            {
                result.Add(row);
            }
        }

        var inTable = false;
        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0)
            {
                continue;
            }

            if (!inTable)
            {
                if (LooksLikeStudentTableHeader(line))
                {
                    inTable = true;
                }

                continue;
            }

            if (LooksLikeEndOfStudentTable(line))
            {
                break;
            }

            TryAddLine(line);
        }

        if (result.Count > 0)
        {
            return result;
        }

        
        foreach (var raw in lines)
        {
            TryAddLine(raw.Trim());
        }

        return result;
    }

    private static bool LooksLikeStudentTableHeader(string line)
    {
        var u = line.ToUpperInvariant();
        return u.Contains("STUDENT", StringComparison.Ordinal) &&
               (u.Contains(" NO", StringComparison.Ordinal) ||
                u.Contains("NO.", StringComparison.Ordinal) ||
                u.Contains("NUMBER", StringComparison.Ordinal) ||
                u.Contains("NAME", StringComparison.Ordinal));
    }

    private static bool LooksLikeEndOfStudentTable(string line)
    {
        var u = line.ToUpperInvariant();
        if (u.Contains("NOTES", StringComparison.Ordinal) || u.Contains("LEGEND", StringComparison.Ordinal))
        {
            return true;
        }

        return EndOfTablePage.IsMatch(line);
    }

    
    
    
    
    
    
    
    
    internal static bool TryExtractStudentNoColumn(string line, out string studentNumber, out ReadOnlySpan<char> remainder)
    {
        studentNumber = string.Empty;
        remainder = ReadOnlySpan<char>.Empty;

        var s = line.Trim();
        if (s.Length == 0)
        {
            return false;
        }

        
        
        if (!HasWhitespaceBeforeFirstLetter(s) &&
            TryExtractGluedRowIndexThenStudentNo(s, out studentNumber, out remainder))
        {
            return true;
        }

        
        var withRowIndex = RowIndexThenStudentNo.Match(s);
        if (withRowIndex.Success)
        {
            studentNumber = withRowIndex.Groups[2].Value;
            var after = s.AsSpan(withRowIndex.Index + withRowIndex.Length).TrimStart();
            remainder = after;
            return true;
        }

        
        var startsWithStudentNo = StartsWithStudentNo.Match(s);
        if (startsWithStudentNo.Success)
        {
            studentNumber = startsWithStudentNo.Groups[1].Value;
            var after = s.AsSpan(startsWithStudentNo.Index + startsWithStudentNo.Length).TrimStart();
            remainder = after;
            return true;
        }

        
        var any = AnyStudentNo.Match(s);
        if (!any.Success)
        {
            return false;
        }

        studentNumber = any.Groups[1].Value;
        remainder = s.AsSpan(any.Index + any.Length).TrimStart();
        return true;
    }

    
    
    
    
    private static bool TryExtractGluedRowIndexThenStudentNo(string s, out string studentNumber, out ReadOnlySpan<char> remainder)
    {
        studentNumber = string.Empty;
        remainder = ReadOnlySpan<char>.Empty;

        if (s.Length < 1 + 8 + 1 || !char.IsDigit(s[0]))
        {
            return false;
        }

        var maxRow = Math.Min(3, s.Length - 9);
        for (var rowLen = maxRow; rowLen >= 1; rowLen--)
        {
            var maxStud = Math.Min(15, s.Length - rowLen - 1);
            for (var studLen = maxStud; studLen >= 8; studLen--)
            {
                if (rowLen + studLen >= s.Length)
                {
                    continue;
                }

                var slice = s.AsSpan(rowLen, studLen);
                if (!IsAllAsciiDigits(slice))
                {
                    continue;
                }

                var boundary = s[rowLen + studLen];
                if (char.IsDigit(boundary))
                {
                    continue;
                }

                studentNumber = slice.ToString();
                remainder = s.AsSpan(rowLen + studLen).TrimStart();
                return true;
            }
        }

        return false;
    }

    private static bool IsAllAsciiDigits(ReadOnlySpan<char> span)
    {
        foreach (var c in span)
        {
            if (c is < '0' or > '9')
            {
                return false;
            }
        }

        return !span.IsEmpty;
    }

    
    private static bool HasWhitespaceBeforeFirstLetter(string s)
    {
        foreach (var c in s)
        {
            if (char.IsLetter(c))
            {
                return false;
            }

            if (char.IsWhiteSpace(c))
            {
                return true;
            }
        }

        return false;
    }

    
    
    
    
    internal static ParsedRow? TryParseEnrollmentLine(string line)
    {
        if (!TryExtractStudentNoColumn(line, out var studentNumber, out var afterSpan))
        {
            return null;
        }

        var after = afterSpan.ToString().Trim();
        var parts = after.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 4)
        {
            return null;
        }

        
        if (parts.Length >= 5 && UnitsToken.IsMatch(parts[^1]))
        {
            return TryParseTail(parts, hasUnits: true, studentNumber);
        }

        
        if (LevelToken.IsMatch(parts[^1]))
        {
            return TryParseTail(parts, hasUnits: false, studentNumber);
        }

        return null;
    }

    private static ParsedRow? TryParseTail(string[] parts, bool hasUnits, string studentNumber)
    {
        int tail = hasUnits ? 4 : 3;
        if (parts.Length < tail)
        {
            return null;
        }

        string level;
        string program;
        string career;

        if (hasUnits)
        {
            var units = parts[^1].Replace(',', '.');
            if (!UnitsNormalized.IsMatch(units))
            {
                return null;
            }

            level = parts[^2];
            program = parts[^3];
            career = parts[^4];
        }
        else
        {
            level = parts[^1];
            program = parts[^2];
            career = parts[^3];
        }

        if (!LevelToken.IsMatch(level))
        {
            return null;
        }

        if (!ProgramToken.IsMatch(program))
        {
            return null;
        }

        if (career.Length < 2 || career.Length > 12)
        {
            return null;
        }

        var nameParts = parts[..^(tail)];
        if (nameParts.Length == 0)
        {
            return null;
        }

        var displayName = string.Join(" ", nameParts);
        return new ParsedRow(studentNumber, displayName, program.ToUpperInvariant(), level.ToUpperInvariant());
    }
}
