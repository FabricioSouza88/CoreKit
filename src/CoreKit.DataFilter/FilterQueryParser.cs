using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter;

public static class FilterQueryParser
{
    public static FilterGroup Parse(string query)
    {
        var group = new FilterGroup
        {
            Logic = "and", // default
            Rules = new List<FilterRule>(),
            Groups = new List<FilterGroup>()
        };

        var expressions = query.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (var expression in expressions)
        {
            var isOr = expression.StartsWith("or:", StringComparison.OrdinalIgnoreCase);
            var clean = isOr ? expression.Substring(3) : expression;

            var parts = clean.Split(':', 3);
            if (parts.Length < 2) continue;

            var field = parts[0];
            var opText = parts[1];
            var value = parts.Length == 3 ? parts[2] : null;

            if (!TryParseOperator(opText, out var op)) continue;

            var rule = new FilterRule
            {
                Field = field,
                Operator = op,
                Value = value
            };

            group.Rules.Add(rule);

            // If any rule is OR, override logic of group
            if (isOr)
                group.Logic = "or";
        }

        return group;
    }

    private static bool TryParseOperator(string opText, out FilterOperatorEnum op)
    {
        op = opText.ToLower() switch
        {
            "eq" => FilterOperatorEnum.Equals,
            "neq" => FilterOperatorEnum.NotEquals,
            "gt" => FilterOperatorEnum.GreaterThan,
            "lt" => FilterOperatorEnum.LessThan,
            "gte" => FilterOperatorEnum.GreaterThanOrEquals,
            "lte" => FilterOperatorEnum.LessThanOrEquals,
            "contains" => FilterOperatorEnum.Contains,
            "startswith" => FilterOperatorEnum.StartsWith,
            "endswith" => FilterOperatorEnum.EndsWith,
            "in" => FilterOperatorEnum.In,
            "null" => FilterOperatorEnum.Null,
            "notnull" => FilterOperatorEnum.NotNull,
            _ => (FilterOperatorEnum)(-1)
        };

        return Enum.IsDefined(typeof(FilterOperatorEnum), op);
    }
}
