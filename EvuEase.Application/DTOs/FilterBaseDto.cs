namespace EvuEase.Application.DTOs
{
    public abstract class FilterBaseDto
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortDirection { get; set; } = "desc";
        public string? SortKey { get; set; } = string.Empty;
    }
}
