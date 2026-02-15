namespace EvuEase.Application.Common
{
    public class PagedResults<T>
    {
        public int PageIndex { get; init; }
        public int PageSize { get; init; }
        public int TotalRecords { get; init; }
        public int TotalEntries { get; init; }
        public int TotalPages { get; init; }
        public List<T> Result { get; set; }

        public PagedResults(int pageIndex, int pageSize, int totalRecords, int totalEntries,  List<T> result)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalEntries = totalEntries;
            TotalPages =  (int)Math.Ceiling(totalRecords / (decimal)pageSize);
            Result = result;
        }
    }
}
