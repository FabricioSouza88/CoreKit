using CoreKit.DataFilter.Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq.Dynamic.Core;
using CoreKit.DataFilter.Linq.Models.Interfaces;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Linq;

public class GenericLinqFilterProcessor<T> : ILinqFilterProcessor<T>
{
    public IQueryable<T> ApplyFilter(IQueryable<T> query, FilterRequest request)
    {
        // WHERE
        if (request.Filter != null && (request.Filter.Rules.Any() || request.Filter.Groups.Any()))
        {
            var predicate = BuildExpression(request.Filter);
            query = query.Where(predicate);
        }

        // ORDER BY (requires Dynamic.Core)
        if (request.Sort?.Any() == true)
        {
            var ordering = string.Join(", ", request.Sort.Select(s => $"{s.Field} {s.Direction}"));
            query = query.OrderBy(ordering);
        }

        // PAGINATION
        if (request.Pagination != null && request.Pagination.PageSize > 0)
        {
            int skip = (request.Pagination.Page - 1) * request.Pagination.PageSize;
            query = query.Skip(skip).Take(request.Pagination.PageSize);
        }

        return query;
    }

    public Expression<Func<T, bool>> BuildExpression(FilterGroup group)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var body = BuildGroupExpression(group, parameter);

        return Expression.Lambda<Func<T, bool>>(body ?? Expression.Constant(true), parameter);
    }

    private Expression? BuildGroupExpression(FilterGroup group, ParameterExpression param)
    {
        var expressions = new List<Expression>();

        foreach (var rule in group.Rules)
        {
            var expr = BuildRuleExpression(rule, param);
            if (expr != null)
                expressions.Add(expr);
        }

        foreach (var subgroup in group.Groups)
        {
            var nested = BuildGroupExpression(subgroup, param);
            if (nested != null)
                expressions.Add(nested);
        }

        if (!expressions.Any()) return null;

        return group.Logic.ToLower() == "or"
            ? expressions.Aggregate(Expression.OrElse)
            : expressions.Aggregate(Expression.AndAlso);
    }

    private Expression? BuildRuleExpression(FilterRule rule, ParameterExpression param)
    {
        var property = typeof(T).GetProperty(rule.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (property == null) return null;

        var member = GetNestedMemberExpression(param, rule.Field);
        if (member == null) return null;

        var constant = GetTypedConstant(property.PropertyType, rule.Value);

        return rule.Operator switch
        {
            FilterOperatorEnum.Equals => Expression.Equal(member, constant),
            FilterOperatorEnum.NotEquals => Expression.NotEqual(member, constant),
            FilterOperatorEnum.GreaterThan => Expression.GreaterThan(member, constant),
            FilterOperatorEnum.GreaterThanOrEquals => Expression.GreaterThanOrEqual(member, constant),
            FilterOperatorEnum.LessThan => Expression.LessThan(member, constant),
            FilterOperatorEnum.LessThanOrEquals => Expression.LessThanOrEqual(member, constant),
            FilterOperatorEnum.Null => Expression.Equal(member, Expression.Constant(null, property.PropertyType)),
            FilterOperatorEnum.NotNull => Expression.NotEqual(member, Expression.Constant(null, property.PropertyType)),
            FilterOperatorEnum.Contains => BuildStringCall(member, "Contains", constant),
            FilterOperatorEnum.StartsWith => BuildStringCall(member, "StartsWith", constant),
            FilterOperatorEnum.EndsWith => BuildStringCall(member, "EndsWith", constant),
            FilterOperatorEnum.In => BuildInExpression(member, property.PropertyType, rule.Value),
            _ => null
        };
    }

    private MemberExpression? GetNestedMemberExpression(ParameterExpression param, string path)
    {
        Expression current = param;
        foreach (var part in path.Split('.'))
        {
            var prop = current.Type.GetProperty(part,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null) return null;

            current = Expression.Property(current, prop);
        }
        return current as MemberExpression;
    }

    private Expression? BuildStringCall(MemberExpression member, string methodName, ConstantExpression constant)
    {
        if (member.Type != typeof(string)) return null;
        var method = typeof(string).GetMethod(methodName, new[] { typeof(string) });
        return method != null ? Expression.Call(member, method, constant) : null;
    }

    private Expression? BuildInExpression(MemberExpression member, Type propertyType, string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var elementType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        var parsedValues = value.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(v => Convert.ChangeType(v.Trim(), elementType))
            .ToArray();

        var valuesArray = Array.CreateInstance(elementType, parsedValues.Length);
        parsedValues.CopyTo(valuesArray, 0);

        var arrayExpression = Expression.Constant(valuesArray);

        var containsMethod = typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(elementType);

        return Expression.Call(null, containsMethod, arrayExpression, member);
    }

    private ConstantExpression GetTypedConstant(Type type, string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return Expression.Constant(null, type);

        var targetType = Nullable.GetUnderlyingType(type) ?? type;

        object? value = targetType switch
        {
            var t when t == typeof(string) => raw,
            var t when t == typeof(bool) => bool.Parse(raw),
            var t when t == typeof(Guid) => Guid.Parse(raw),
            var t when t == typeof(DateTime) => DateTime.Parse(raw),
            var t when t.IsEnum => Enum.Parse(t, raw),
            _ => Convert.ChangeType(raw, targetType)
        };

        return Expression.Constant(value, type);
    }
}
