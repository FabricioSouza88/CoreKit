using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Sql
{
    public class MySqlFilterProcessor : GenericSqlFilterProcessor
    {
        protected override string BuildRule(FilterRule rule)
        {
            var field = rule.Field;
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
    }
}
