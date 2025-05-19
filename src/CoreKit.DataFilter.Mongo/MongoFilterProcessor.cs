using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;
using CoreKit.DataFilter.Mongo.Interfaces;
using MongoDB.Driver;

namespace CoreKit.DataFilter.Mongo
{
    public class MongoFilterProcessor<T> : IMongoFilterProcessor<T>
    {
        private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;
        private readonly SortDefinitionBuilder<T> _sortBuilder = Builders<T>.Sort;

        public FilterDefinition<T> BuildFilter(FilterRequest request)
        {
            return BuildGroup(_filterBuilder, request.Filter) ?? _filterBuilder.Empty;
        }

        public SortDefinition<T>? BuildSort(FilterRequest request)
        {
            if (request.Sort == null || request.Sort.Count == 0)
                return null;

            SortDefinition<T>? sort = null;

            foreach (var s in request.Sort)
            {
                var field = s.Field;
                var dir = s.Direction?.ToLower() == "desc"
                    ? _sortBuilder.Descending(field)
                    : _sortBuilder.Ascending(field);

                sort = sort == null ? dir : _sortBuilder.Combine(sort, dir);
            }

            return sort;
        }

        public (int skip, int limit) BuildPagination(FilterRequest request)
        {
            if (request.Pagination == null || request.Pagination.PageSize <= 0)
                return (0, 0);

            int skip = (request.Pagination.Page - 1) * request.Pagination.PageSize;
            return (skip, request.Pagination.PageSize);
        }

        public IFindFluent<T, T> ApplyAll(IMongoCollection<T> collection, FilterRequest request)
        {
            var filter = BuildFilter(request);
            var sort = BuildSort(request);
            var (skip, limit) = BuildPagination(request);

            var query = collection.Find(filter);

            if (sort != null)
                query = query.Sort(sort);

            if (skip > 0)
                query = query.Skip(skip);

            if (limit > 0)
                query = query.Limit(limit);

            return query;
        }

        private FilterDefinition<T>? BuildGroup(FilterDefinitionBuilder<T> builder, FilterGroup group)
        {
            var filters = new List<FilterDefinition<T>>();

            foreach (var rule in group.Rules)
            {
                var field = rule.Field;
                var value = rule.Value;

                FilterDefinition<T>? f = rule.Operator switch
                {
                    FilterOperatorEnum.Equals => builder.Eq(field, value),
                    FilterOperatorEnum.NotEquals => builder.Ne(field, value),
                    FilterOperatorEnum.Contains => builder.Regex(field, new MongoDB.Bson.BsonRegularExpression(value, "i")),
                    FilterOperatorEnum.StartsWith => builder.Regex(field, new MongoDB.Bson.BsonRegularExpression($"^{value}", "i")),
                    FilterOperatorEnum.EndsWith => builder.Regex(field, new MongoDB.Bson.BsonRegularExpression($"{value}$", "i")),
                    FilterOperatorEnum.GreaterThan => builder.Gt(field, value),
                    FilterOperatorEnum.GreaterThanOrEquals => builder.Gte(field, value),
                    FilterOperatorEnum.LessThan => builder.Lt(field, value),
                    FilterOperatorEnum.LessThanOrEquals => builder.Lte(field, value),
                    FilterOperatorEnum.Null => builder.Eq(field, null as string),
                    FilterOperatorEnum.NotNull => builder.Ne(field, null as string),
                    FilterOperatorEnum.In => builder.In(field, value?.Split(',').Select(v => v.Trim())),
                    _ => null
                };

                if (f != null)
                    filters.Add(f);
            }

            foreach (var subgroup in group.Groups)
            {
                var subfilter = BuildGroup(builder, subgroup);
                if (subfilter != null)
                    filters.Add(subfilter);
            }

            return group.Logic?.ToLower() == "or"
                ? builder.Or(filters)
                : builder.And(filters);
        }
    }
}
