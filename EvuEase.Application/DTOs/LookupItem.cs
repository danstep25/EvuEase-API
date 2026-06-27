namespace EvuEase.Application.DTOs;

public class LookupItem
{
    public long Id { get; set; }
    public string Value { get; set; } = string.Empty;
    public string? DisplayText { get; set; }
    public int? NumericValue { get; set; }
}


