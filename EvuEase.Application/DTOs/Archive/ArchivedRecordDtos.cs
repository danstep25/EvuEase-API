namespace EvuEase.Application.DTOs.Archive;

public class ArchivedProgramDto
{
    public string Id { get; set; } = string.Empty;
    public string DeletedAt { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public string ProgramCode { get; set; } = string.Empty;
    public string ProgramTitle { get; set; } = string.Empty;
    public int Years { get; set; }
    public int TotalUnits { get; set; }
}

public class ArchivedStudentDto
{
    public string Id { get; set; } = string.Empty;
    public string DeletedAt { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ProgramCode { get; set; } = string.Empty;
}

public class ArchivedSchoolYearDto
{
    public string Id { get; set; } = string.Empty;
    public string DeletedAt { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}

public class ArchivedCurriculumDto
{
    public string Id { get; set; } = string.Empty;
    public string DeletedAt { get; set; } = string.Empty;
    public string DeletedBy { get; set; } = string.Empty;
    public string CurriculumCode { get; set; } = string.Empty;
    public string CurriculumTitle { get; set; } = string.Empty;
}
