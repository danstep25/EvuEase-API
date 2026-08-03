namespace EvuEase.Application.DTOs.ClassRoster;


public class ClassRosterPdfImportSummaryResponse
{
    public IReadOnlyList<ClassRosterPdfImportPageResult> Pages { get; set; } = Array.Empty<ClassRosterPdfImportPageResult>();
}
