namespace EvuEase.Application.DTOs.ClassRoster;

public class ClassRosterBatchUploadResponse
{
    public int ImportedCount { get; set; }

    
    public IReadOnlyList<RosterPdfStudentNotInRegistry> NotFoundInRegistry { get; set; } =
        Array.Empty<RosterPdfStudentNotInRegistry>();

    public IReadOnlyList<string> Warnings { get; set; } = Array.Empty<string>();
}
