namespace EvuEase.Application.Common;

public static class CourseValidationConstants
{
    public const int MaxTitleLength = 250;
    public const int MaxPrerequisitesLength = 200;

    public static string TitleTooLongMessage =>
        $"Course title must be {MaxTitleLength} characters or fewer.";

    public static string PrerequisitesTooLongMessage =>
        "Pre-requisites exceed the maximum length.";

    public static bool IsTitleLengthMessage(string message) =>
        message.StartsWith("Course title must", StringComparison.OrdinalIgnoreCase);
}
