using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;
using System.Text;

namespace CoreKit.DataFilter.Sql
{
    public class GenericSqlFilterProcessor : IFilterProcessor<string>
    {
        public string BuildWhereClause(FilterGroup group)
        {
            var clause = BuildGroup(group);
            return string.IsNullOrWhiteSpace(clause) ? string.Empty : $"WHERE {clause}";
        }

        public string BuildQuery(FilterRequest request)
        {
            var where = BuildWhereClause(request.Filter);
            var order = BuildOrderBy(request.Sort);
            var paging = BuildPagination(request.Pagination);

            var sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(where)) sb.Append(where + " ");
            if (!string.IsNullOrWhiteSpace(order)) sb.Append(order + " ");
            if (!string.IsNullOrWhiteSpace(paging)) sb.Append(paging);

            return sb.ToString().Trim();
        }

        protected virtual string BuildGroup(FilterGroup group)
        {
            var parts = new List<string>();

            foreach (var rule in group.Rules)
            {
                var expr = BuildRule(rule);
                if (!string.IsNullOrWhiteSpace(expr)) parts.Add(expr);
            }

            foreach (var subgroup in group.Groups)
            {
                var sub = BuildGroup(subgroup);
                if (!string.IsNullOrWhiteSpace(sub)) parts.Add($"({sub})");
            }

            var logic = group.Logic.Equals("or", StringComparison.OrdinalIgnoreCase) ? "OR" : "AND";
            return string.Join($" {logic} ", parts);
        }

        protected virtual string BuildRule(FilterRule rule)
        {
            var field = rule.Field; // no identifier quotes to remain generic
            var value = rule.Value;

            return rule.Operator switch
            {
                FilterOperatorEnum.Equals => $"{field} = '{Escape(value)}'",
                FilterOperatorEnum.NotEquals => $"{field} <> '{Escape(value)}'",
                FilterOperatorEnum.Contains => $"{field} LIKE '%{EscapeLike(value)}%'",
                FilterOperatorEnum.StartsWith => $"{field} LIKE '{EscapeLike(value)}%'",
                FilterOperatorEnum.EndsWith => $"{field} LIKE '%{EscapeLike(value)}'",
                FilterOperatorEnum.GreaterThan => $"{field} > '{Escape(value)}'",
                FilterOperatorEnum.GreaterThanOrEquals => $"{field} >= '{Escape(value)}'",
                FilterOperatorEnum.LessThan => $"{field} < '{Escape(value)}'",
                FilterOperatorEnum.LessThanOrEquals => $"{field} <= '{Escape(value)}'",
                FilterOperatorEnum.Null => $"{field} IS NULL",
                FilterOperatorEnum.NotNull => $"{field} IS NOT NULL",
                FilterOperatorEnum.In => BuildInClause(field, value),
                _ => string.Empty
            };
        }

        protected virtual string BuildInClause(string field, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;

            var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => $"'{Escape(v.Trim())}'");

            return $"{field} IN ({string.Join(", ", values)})";
        }

        protected virtual string BuildOrderBy(List<SortRule>? sortRules)
        {
            if (sortRules == null || sortRules.Count == 0) return string.Empty;

            var clauses = sortRules.Select(s =>
                $"{s.Field} {(s.Direction?.ToLower() == "desc" ? "DESC" : "ASC")}");

            return $"ORDER BY {string.Join(", ", clauses)}";
        }

        protected virtual string BuildPagination(Pagination? pagination)
        {
            if (pagination == null || pagination.PageSize <= 0) return string.Empty;

            var offset = (pagination.Page - 1) * pagination.PageSize;
            return $"LIMIT {pagination.PageSize} OFFSET {offset}";
        }

        protected virtual string Escape(string? input)
        {
            return input?.Replace("'", "''") ?? string.Empty;
        }

        protected virtual string EscapeLike(string? input)
        {
            return Escape(input); // minimal for now, can be expanded
        }
    }
}
