using AspNetCoreHero.Results;
using Microsoft.EntityFrameworkCore;
using Travl.Application.Interfaces;

namespace Travl.Infrastructure.Implementations
{
    public class PaginationHelper : IPaginationHelper
    {
        public async Task<PaginatedResult<T>> ApplyPaginationAsync<T>(
        IQueryable<T> query,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken = default)
        {
            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // If pageNumber or pageSize is null, return all data
            if (pageNumber == null || pageSize == null)
            {
                var allData = await query.ToListAsync(cancellationToken);
                return PaginatedResult<T>.Success(allData, totalCount, pageNumber ?? 1, pageSize ?? totalCount);
            }

            // Apply pagination (skip and take)
            var paginatedQuery = query.Skip((pageNumber.Value - 1) * pageSize.Value)
                                      .Take(pageSize.Value);

            // Fetch the paginated data
            var paginatedData = await paginatedQuery.ToListAsync(cancellationToken);

            // Return a PaginatedResult from AspNetCoreHero
            return PaginatedResult<T>.Success(paginatedData, totalCount, pageNumber.Value, pageSize.Value);
        }

    }
}
