using CoreKit.DataFilter.Models;

namespace CoreKit.DataFilter.Linq.Models.Interfaces
{
    public interface ILinqFilterProcessor<T>
    {
        IQueryable<T> ApplyFilter(IQueryable<T> query, FilterRequest request);
    }
}
