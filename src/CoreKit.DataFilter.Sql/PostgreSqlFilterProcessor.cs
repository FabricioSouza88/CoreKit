using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Sql
{
    public class PostgreSqlFilterProcessor : GenericSqlFilterProcessor
    {
        protected override string BuildRule(FilterRule rule)
        {
            var field = rule.Field;
            var value = rule.Value;

            return rule.Operator switch
            {
                FilterOperatorEnum.Equals => $"{field} = '{Escape(value)}'",
                FilterOperatorEnum.NotEquals => $"{field} <> '{Escape(value)}'",
                FilterOperatorEnum.Contains => $"{field} ILIKE '%{EscapeLike(value)}%'",
                FilterOperatorEnum.StartsWith => $"{field} ILIKE '{EscapeLike(value)}%'",
                FilterOperatorEnum.EndsWith => $"{field} ILIKE '%{EscapeLike(value)}'",
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

        // Optional: if you need PostgreSQL-safe escaping
        protected override string EscapeLike(string? input)
        {
            return Escape(input); // You can expand this to escape % or _ if needed
        }
    }
}
