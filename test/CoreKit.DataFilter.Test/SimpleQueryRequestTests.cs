using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Tests;

public class SimpleQueryRequestTests
{
    [Fact]
    public void Should_Convert_FilterQuery_To_FilterRequest()
    {
        var query = new SimpleQueryRequest
        {
            FilterQuery = "category:eq:Books;price:gt:100"
        };

        var result = query.ToFilterRequest();

        Assert.NotNull(result);
        Assert.NotNull(result.Filter);
        Assert.Equal(2, result.Filter.Rules.Count);

        Assert.Contains(result.Filter.Rules, r =>
            r.Field == "category" &&
            r.Operator == FilterOperatorEnum.Equals &&
            r.Value == "Books");

        Assert.Contains(result.Filter.Rules, r =>
            r.Field == "price" &&
            r.Operator == FilterOperatorEnum.GreaterThan &&
            r.Value == "100");
    }

    [Fact]
    public void Should_Apply_Default_Pagination_If_Not_Set()
    {
        var query = new SimpleQueryRequest();

        var result = query.ToFilterRequest();

        Assert.Equal(1, result.Pagination.Page);
        Assert.Equal(10, result.Pagination.PageSize);
    }

    [Fact]
    public void Should_Apply_Sorting_If_Provided()
    {
        var query = new SimpleQueryRequest
        {
            Sort = "price",
            SortDir = "desc"
        };

        var result = query.ToFilterRequest();

        Assert.Single(result.Sort);
        Assert.Equal("price", result.Sort[0].Field);
        Assert.Equal("desc", result.Sort[0].Direction);
    }

    [Fact]
    public void Should_Default_Sort_Direction_To_Asc_If_Not_Specified()
    {
        var query = new SimpleQueryRequest
        {
            Sort = "name"
        };

        var result = query.ToFilterRequest();

        Assert.Single(result.Sort);
        Assert.Equal("asc", result.Sort[0].Direction);
    }

    [Fact]
    public void Should_Apply_Custom_Pagination()
    {
        var query = new SimpleQueryRequest
        {
            Page = 3,
            PageSize = 25
        };

        var result = query.ToFilterRequest();

        Assert.Equal(3, result.Pagination.Page);
        Assert.Equal(25, result.Pagination.PageSize);
    }

    [Fact]
    public void Should_Parse_AND_Logic_By_Default()
    {
        var query = new SimpleQueryRequest
        {
            FilterQuery = "category:eq:Books;price:gt:100"
        };

        var result = query.ToFilterRequest();

        Assert.Equal("and", result.Filter.Logic.ToLower());
        Assert.Equal(2, result.Filter.Rules.Count);
    }

    [Fact]
    public void Should_Parse_OR_Logic_When_Prefixed()
    {
        var query = new SimpleQueryRequest
        {
            FilterQuery = "or:category:eq:Books;or:category:eq:Electronics"
        };

        var result = query.ToFilterRequest();

        Assert.Equal("or", result.Filter.Logic.ToLower());
        Assert.Equal(2, result.Filter.Rules.Count);
        Assert.All(result.Filter.Rules, r => Assert.Equal("category", r.Field));
    }

    [Fact]
    public void Should_Parse_IN_Operator()
    {
        var query = new SimpleQueryRequest
        {
            FilterQuery = "category:in:Books,Electronics"
        };

        var result = query.ToFilterRequest();

        Assert.Single(result.Filter.Rules);
        var rule = result.Filter.Rules.First();

        Assert.Equal("category", rule.Field);
        Assert.Equal(FilterOperatorEnum.In, rule.Operator);
        Assert.Equal("Books,Electronics", rule.Value);
    }

    [Fact]
    public void Should_Handle_Combination_Of_IN_And_Comparison()
    {
        var query = new SimpleQueryRequest
        {
            FilterQuery = "category:in:Books,Electronics;price:lt:500"
        };

        var result = query.ToFilterRequest();

        Assert.Equal(2, result.Filter.Rules.Count);

        var inRule = result.Filter.Rules.First(r => r.Operator == FilterOperatorEnum.In);
        var ltRule = result.Filter.Rules.First(r => r.Operator == FilterOperatorEnum.LessThan);

        Assert.Equal("category", inRule.Field);
        Assert.Equal("price", ltRule.Field);
    }
}
