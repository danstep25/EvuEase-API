using EvuEase.Application.Common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace EvuEase.Infrastructure.Common
{
    internal static class PaginateQueryableBehavior
    {
        public static async Task<PagedResults<T>> PaginateAsync<T>(
            this IQueryable<T> query,
            int pageIndex = 1,
            int pageSize = 10,
            string? sortKey = "",
            string? sortDirection = "desc",
            int totalEntries = 0
            )
        {
            if(!string.IsNullOrWhiteSpace(sortKey) && !string.IsNullOrEmpty(sortDirection))
            {
                var sortExpression = $"{sortKey} {sortDirection}";
                query = query.OrderBy(sortExpression);
            }

            var totalRecords = await query.CountAsync();

            var entities = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new(pageIndex, pageSize, totalRecords, totalEntries, entities);
        }
    }
}
