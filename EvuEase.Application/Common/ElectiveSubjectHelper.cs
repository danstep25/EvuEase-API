using System.Text.RegularExpressions;

namespace EvuEase.Application.Common;

public static class ElectiveSubjectHelper
{
    private static readonly Regex ElectiveSlotTitlePattern = new(
        @"\b(?:CS|IT|GE|BSCS|BSIT|Professional|General\s+Education|Prof\.?|Gen\.?\s+Ed\.?)\s+Elective\s*(?:\d+|[IVXLC]+)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex GenericElectiveSlotTitlePattern = new(
        @"\bElective\s*(?:\d+|[IVXLC]+)\b",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex ElectiveSlotCodePattern = new(
        @"^ELEC",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public static bool IsElectiveSlot(string? courseTitle, string? courseCode)
    {
        var title = courseTitle?.Trim() ?? string.Empty;
        var code = courseCode?.Trim() ?? string.Empty;

        if (title.Length > 0 &&
            (ElectiveSlotTitlePattern.IsMatch(title) || GenericElectiveSlotTitlePattern.IsMatch(title)))
        {
            return true;
        }

        return code.Length > 0 && ElectiveSlotCodePattern.IsMatch(code);
    }

    public static bool ResolveIsElectiveSlot(string? courseTitle, string? courseCode, bool? requested)
    {
        return requested ?? IsElectiveSlot(courseTitle, courseCode);
    }

    public static bool ResolveIsElectiveOption(bool isElectiveSlot, bool? requested)
    {
        if (isElectiveSlot)
        {
            return false;
        }

        return requested ?? false;
    }
}
