namespace EvuEase.Application.DTOs;

public class LookupResponse
{
    public int Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string? DisplayText { get; set; }
    public int? NumericValue { get; set; }
}

