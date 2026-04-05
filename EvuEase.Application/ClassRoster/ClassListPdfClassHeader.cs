using System.Globalization;
using System.Text.RegularExpressions;

namespace EvuEase.Application.ClassRoster;


public sealed record ParsedClassHeader(
    string CourseCode,
    string CourseTitle,
    
    decimal? TotalUnits,
    string ClassNumber,
    
    string ProgramCode,
    
    string SectionLetter,
    int? EnrolledFromHeader);

public static class ClassListPdfClassHeaderParser
{
    
    
    
    
    private static readonly Regex CourseSummaryLine = new(
        @"^(\d+)\s+([A-Z0-9]+)\s+(.+?)\s+(\d+\.\d{2})\s+(\S+)\s+(\d+)\s+([A-Za-z]+\-\d+[A-Za-z])\s+(\d+)\s*$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex SectionToken = new(
        @"^([A-Za-z]+)\-(\d+)([A-Za-z])$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex AcademicTermRegex = new(
        @"SY\s*&\s*(?:Term|Semester)\s*:\s*([^\r\n]+)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex WordTermAsPeriod = new(
        @"\bTerm\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex SlashRuns = new(
        @"\s*/\s*",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    
    
    
    
    public static string NormalizeAcademicPeriodLabel(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value?.Trim() ?? string.Empty;
        }

        var s = WordTermAsPeriod.Replace(value.Trim(), "Semester");
        s = SlashRuns.Replace(s, " / ");
        return s;
    }

    
    
    
    
    public static IReadOnlyList<string> GetAcademicPeriodMatchKeys(string? value)
    {
        var n = NormalizeAcademicPeriodLabel(value ?? string.Empty);
        if (string.IsNullOrEmpty(n))
        {
            return Array.Empty<string>();
        }

        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { n };
        var compact = n.Replace(" / ", "/", StringComparison.Ordinal);
        if (!string.Equals(n, compact, StringComparison.Ordinal))
        {
            set.Add(compact);
        }

        return set.ToList();
    }

    public static string? TryParseAcademicTerm(string fullText)
    {
        if (string.IsNullOrWhiteSpace(fullText))
        {
            return null;
        }

        var m = AcademicTermRegex.Match(fullText);
        if (!m.Success)
        {
            return null;
        }

        var raw = m.Groups[1].Value.Trim();
        var cut = raw.IndexOf("STI", StringComparison.OrdinalIgnoreCase);
        if (cut > 0)
        {
            raw = raw[..cut].Trim();
        }

        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        return NormalizeAcademicPeriodLabel(raw);
    }

    
    public static bool TryParseClassHeaderFromPage(string pageText, out ParsedClassHeader header)
    {
        header = default!;
        if (string.IsNullOrWhiteSpace(pageText))
        {
            return false;
        }

        foreach (var raw in pageText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None))
        {
            var line = raw.Trim();
            if (line.Length == 0)
            {
                continue;
            }

            var m = CourseSummaryLine.Match(line);
            if (!m.Success)
            {
                continue;
            }

            var courseCode = m.Groups[2].Value.Trim();
            var title = m.Groups[3].Value.Trim();
            var unitsStr = m.Groups[4].Value.Trim();
            var classNo = m.Groups[6].Value.Trim();
            var sectionTok = m.Groups[7].Value.Trim();
            var enrolledStr = m.Groups[8].Value.Trim();

            var sm = SectionToken.Match(sectionTok);
            if (!sm.Success)
            {
                continue;
            }

            var programCode = sm.Groups[1].Value.ToUpperInvariant();
            var sectionLetter = sm.Groups[3].Value.ToUpperInvariant();

            int? enrolled = int.TryParse(enrolledStr, out var en) ? en : null;
            decimal? totalUnits = decimal.TryParse(
                unitsStr,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out var u)
                ? u
                : null;

            header = new ParsedClassHeader(
                CourseCode: courseCode,
                CourseTitle: title,
                TotalUnits: totalUnits,
                ClassNumber: classNo,
                ProgramCode: programCode,
                SectionLetter: sectionLetter,
                EnrolledFromHeader: enrolled);

            return true;
        }

        return false;
    }
}
