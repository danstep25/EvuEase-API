namespace EvuEase.Application.DTOs.ClassRoster;


public sealed class ClassListPdfPreviewResponse
{
    public string AcademicTerm { get; set; } = string.Empty;

    public IReadOnlyList<ClassListPdfPreviewPage> Pages { get; set; } = Array.Empty<ClassListPdfPreviewPage>();
}

public sealed class ClassListPdfPreviewPage
{
    public int PageNumber { get; set; }

    
    public string? SkippedReason { get; set; }

    public string? CourseCode { get; set; }

    public string? CourseTitle { get; set; }

    public decimal? TotalUnits { get; set; }

    public string? ProgramCode { get; set; }

    public string? ClassNumber { get; set; }

    public string? SectionLetter { get; set; }

    public string? YearLevel { get; set; }

    
    public string? AcademicTerm { get; set; }

    
    public bool CourseExistsInModule { get; set; }
}
