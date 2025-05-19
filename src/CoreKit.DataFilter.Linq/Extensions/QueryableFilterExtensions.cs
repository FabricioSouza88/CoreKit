using CoreKit.DataFilter.Models;

namespace CoreKit.DataFilter.Linq.Extensions;

public static class QueryableFilterExtensions
{
    public static IQueryable<T> ApplyFilterRequest<T>(this IQueryable<T> query, FilterRequest request)
    {
        var processor = new GenericLinqFilterProcessor<T>();
        return processor.ApplyFilter(query, request);
    }
}
