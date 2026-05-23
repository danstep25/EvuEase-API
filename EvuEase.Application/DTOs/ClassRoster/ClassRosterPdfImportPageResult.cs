namespace EvuEase.Application.DTOs.ClassRoster;


public class ClassRosterPdfImportPageResult
{
    public int PageNumber { get; set; }

    public string CourseCode { get; set; } = string.Empty;

    public string ClassNumber { get; set; } = string.Empty;

    public string Section { get; set; } = string.Empty;

    public string ProgramCode { get; set; } = string.Empty;

    public string YearLevel { get; set; } = string.Empty;

    public long FacultyClassId { get; set; }

    public bool ClassCreatedFromPdf { get; set; }

    public int ImportedCount { get; set; }

    public int AutoCreatedCount { get; set; }

    public IReadOnlyList<RosterPdfStudentNotInRegistry> NotFoundInRegistry { get; set; } =
        Array.Empty<RosterPdfStudentNotInRegistry>();

    public IReadOnlyList<string> Warnings { get; set; } = Array.Empty<string>();

    
    public string? SkippedReason { get; set; }
}
