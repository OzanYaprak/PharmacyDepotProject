using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Persistence.Paging;

public static class IQueryablePaginateExtensions
{
    public static async Task<Paginate<T>> ToPaginateAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
        int deletedDataCount = await GetDeletedDataCountAsync(source, cancellationToken).ConfigureAwait(false);

        List<T> items = await source.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

        Paginate<T> paginatedList = new Paginate<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            DataCount = count,
            DeletedDataCount = deletedDataCount,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            DataList = items,
        };

        return paginatedList;
    }

    private static async Task<int> GetDeletedDataCountAsync<T>(IQueryable<T> source, CancellationToken cancellationToken)
    {
        var deletedDateProperty = typeof(T).GetProperty("DeletedDate");

        if (deletedDateProperty is null || deletedDateProperty.PropertyType != typeof(DateTime?))
        {
            return 0;
        }

        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        MemberExpression deletedDate = Expression.Property(parameter, deletedDateProperty);
        BinaryExpression predicateBody = Expression.NotEqual(deletedDate, Expression.Constant(null, typeof(DateTime?)));
        Expression<Func<T, bool>> predicate = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);

        return await source.Where(predicate).CountAsync(cancellationToken).ConfigureAwait(false);
    }
}
