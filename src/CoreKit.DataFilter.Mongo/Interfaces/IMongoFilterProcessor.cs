using CoreKit.DataFilter.Models;
using MongoDB.Driver;

namespace CoreKit.DataFilter.Mongo.Interfaces
{
    public interface IMongoFilterProcessor<T>
    {
        FilterDefinition<T> BuildFilter(FilterRequest request);
        SortDefinition<T>? BuildSort(FilterRequest request);
        (int skip, int limit) BuildPagination(FilterRequest request);

        /// <summary>
        /// Combines filter, sort, and pagination into a full Mongo query.
        /// </summary>
        IFindFluent<T, T> ApplyAll(IMongoCollection<T> collection, FilterRequest request);
    }
}
