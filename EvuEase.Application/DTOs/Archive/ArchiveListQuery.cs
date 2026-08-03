namespace EvuEase.Application.DTOs.Archive;

public class ArchiveListQuery
{
    public string? SearchTerm { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? DeletedBy { get; set; }
}

public class ArchiveProgramListQuery : ArchiveListQuery
{
    public string? YearsOfCompletion { get; set; }
}

public class ArchiveStudentListQuery : ArchiveListQuery
{
    public string? ProgramCode { get; set; }
    public string? YearLevel { get; set; }
}

public class ArchiveSchoolYearListQuery : ArchiveListQuery
{
    public string? Semester { get; set; }
}
