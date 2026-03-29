using EvuEase.Application.DTOs;

namespace EvuEase.Application.DTOs.Student;

public class StudentRequest : FilterBaseDto
{
    public string? ProgramCode { get; set; }
    public string? YearLevel { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
}
