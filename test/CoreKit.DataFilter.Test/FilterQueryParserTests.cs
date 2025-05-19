using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Tests;

public class FilterQueryParserTests
{
    [Fact]
    public void Parse_SingleRule_ReturnsExpectedFilterGroup()
    {
        // Arrange
        var query = "name:eq:Fabricio";

        // Act
        var result = FilterQueryParser.Parse(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Rules);
        var rule = result.Rules.First();
        Assert.Equal("name", rule.Field);
        Assert.Equal(FilterOperatorEnum.Equals, rule.Operator);
        Assert.Equal("Fabricio", rule.Value);
    }

    [Fact]
    public void Parse_MultipleRules_ReturnsAllRules()
    {
        var query = "name:eq:Fabricio;price:gt:10";

        var result = FilterQueryParser.Parse(query);

        Assert.Equal(2, result.Rules.Count);
        Assert.Contains(result.Rules, r => r.Field == "name" && r.Operator == FilterOperatorEnum.Equals && r.Value == "Fabricio");
        Assert.Contains(result.Rules, r => r.Field == "price" && r.Operator == FilterOperatorEnum.GreaterThan && r.Value == "10");
    }

    [Fact]
    public void Parse_UnknownOperator_IgnoresInvalidRule()
    {
        var query = "name:eq:Fabricio;email:like:abc";

        var result = FilterQueryParser.Parse(query);

        Assert.Single(result.Rules);
        Assert.Equal("name", result.Rules.First().Field);
    }

    [Fact]
    public void Parse_InOperator_ParsesListAsString()
    {
        var query = "category:in:1,2,3";

        var result = FilterQueryParser.Parse(query);

        Assert.Single(result.Rules);
        var rule = result.Rules.First();
        Assert.Equal("category", rule.Field);
        Assert.Equal(FilterOperatorEnum.In, rule.Operator);
        Assert.Equal("1,2,3", rule.Value);
    }

    [Fact]
    public void Parse_NullOperator_ParsesWithoutValue()
    {
        var query = "deletedAt:null";

        var result = FilterQueryParser.Parse(query);

        Assert.Single(result.Rules);
        var rule = result.Rules.First();
        Assert.Equal("deletedAt", rule.Field);
        Assert.Equal(FilterOperatorEnum.Null, rule.Operator);
        Assert.Null(rule.Value);
    }
}
