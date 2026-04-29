using Microsoft.EntityFrameworkCore;

namespace Persistence.Paging;

public static class IQueryablePaginateExtensions
{
    public static async Task<Paginate<T>> ToPaginateAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
        
        List<T> items = await source.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

        Paginate<T> paginatedList = new Paginate<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            DataCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            DataList = items
        };

        return paginatedList;
    }
}
