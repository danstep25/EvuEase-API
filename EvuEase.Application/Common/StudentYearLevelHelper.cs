using System.Text.RegularExpressions;

namespace EvuEase.Application.Common;

public static class StudentYearLevelHelper
{
    private static readonly Regex FirstYearShortPattern = new(@"^1Y\d", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex YearTermPattern = new(@"^(\d)Y([12])$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex YearTermSuffixPattern = new(@"Year\s*(\d)\s*Y\s*([12])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly Dictionary<string, char> YearLabelToDigit = new(StringComparer.OrdinalIgnoreCase)
    {
        ["first year"] = '1',
        ["second year"] = '2',
        ["third year"] = '3',
        ["fourth year"] = '4',
        ["fifth year"] = '5',
        ["year 1"] = '1',
        ["year 2"] = '2',
        ["year 3"] = '3',
        ["year 4"] = '4',
        ["year 5"] = '5',
        ["1st year"] = '1',
        ["2nd year"] = '2',
        ["3rd year"] = '3',
        ["4th year"] = '4',
        ["5th year"] = '5'
    };

    public static string NormalizeYearTerm(string? yearLevel)
    {
        var value = yearLevel?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(value))
        {
            return "1Y1";
        }

        var compact = Regex.Replace(value, @"\s+", string.Empty);
        var compactMatch = YearTermPattern.Match(compact);
        if (compactMatch.Success && IsValidYearDigit(compactMatch.Groups[1].Value))
        {
            return $"{compactMatch.Groups[1].Value}Y{compactMatch.Groups[2].Value}".ToUpperInvariant();
        }

        var suffixMatch = YearTermSuffixPattern.Match(value);
        if (suffixMatch.Success && IsValidYearDigit(suffixMatch.Groups[1].Value))
        {
            return $"{suffixMatch.Groups[1].Value}Y{suffixMatch.Groups[2].Value}".ToUpperInvariant();
        }

        if (YearLabelToDigit.TryGetValue(value, out var yearDigit))
        {
            return $"{yearDigit}Y1";
        }

        var yearOnlyMatch = Regex.Match(value, @"^Year\s*(\d)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (yearOnlyMatch.Success && IsValidYearDigit(yearOnlyMatch.Groups[1].Value))
        {
            return $"{yearOnlyMatch.Groups[1].Value}Y1";
        }

        var ordinalMatch = Regex.Match(value, @"^(\d)(?:st|nd|rd|th)\s+Year$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (ordinalMatch.Success && IsValidYearDigit(ordinalMatch.Groups[1].Value))
        {
            return $"{ordinalMatch.Groups[1].Value}Y1";
        }

        return "1Y1";
    }

    public static bool IsFirstYear(string? yearLevel)
    {
        if (string.IsNullOrWhiteSpace(yearLevel))
        {
            return false;
        }

        var normalized = NormalizeYearTerm(yearLevel);
        return normalized.StartsWith("1Y", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsValidYearDigit(string digit) =>
        digit is "1" or "2" or "3" or "4" or "5";
}
