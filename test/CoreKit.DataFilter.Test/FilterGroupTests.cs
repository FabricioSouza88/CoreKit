using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Tests
{
    public class FilterGroupTests
    {
        [Fact]
        public void Should_Create_SimpleGroupWithTwoRules_AndLogicAnd()
        {
            // Arrange
            var group = new FilterGroup
            {
                Logic = "and",
                Rules = new()
                {
                    new FilterRule { Field = "status", Operator = FilterOperatorEnum.Equals, Value = "active" },
                    new FilterRule { Field = "deleted", Operator = FilterOperatorEnum.Equals, Value = "false" }
                }
            };

            // Assert
            Assert.Equal("and", group.Logic);
            Assert.NotNull(group.Rules);
            Assert.Equal(2, group.Rules.Count);
            Assert.Empty(group.Groups);

            Assert.Equal("status", group.Rules[0].Field);
            Assert.Equal(FilterOperatorEnum.Equals, group.Rules[0].Operator);
            Assert.Equal("active", group.Rules[0].Value);
        }

        [Fact]
        public void Should_Create_GroupWithSubgroup_UsingMixedLogic()
        {
            // Arrange
            var group = new FilterGroup
            {
                Logic = "and",
                Rules = new()
                {
                    new FilterRule { Field = "status", Operator = FilterOperatorEnum.Equals, Value = "active" }
                },
                Groups = new()
                {
                    new FilterGroup
                    {
                        Logic = "or",
                        Rules = new()
                        {
                            new FilterRule { Field = "name", Operator = FilterOperatorEnum.Contains, Value = "Fabricio" },
                            new FilterRule { Field = "email", Operator = FilterOperatorEnum.EndsWith, Value = "@gmail.com" }
                        }
                    }
                }
            };

            // Assert root group
            Assert.Equal("and", group.Logic);
            Assert.Single(group.Rules);
            Assert.Single(group.Groups);

            // Assert root rule
            var rootRule = group.Rules.First();
            Assert.Equal("status", rootRule.Field);
            Assert.Equal(FilterOperatorEnum.Equals, rootRule.Operator);
            Assert.Equal("active", rootRule.Value);

            // Assert subgroup
            var subgroup = group.Groups.First();
            Assert.Equal("or", subgroup.Logic);
            Assert.Equal(2, subgroup.Rules.Count);
            Assert.Equal("name", subgroup.Rules[0].Field);
            Assert.Equal(FilterOperatorEnum.Contains, subgroup.Rules[0].Operator);
            Assert.Equal("Fabricio", subgroup.Rules[0].Value);
        }
    }
}
