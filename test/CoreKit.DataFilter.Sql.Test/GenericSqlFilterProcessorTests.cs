using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Sql.Tests
{
    public class GenericSqlFilterProcessorTests
    {
        [Fact]
        public void Should_Generate_WhereClause_With_Multiple_Operators()
        {
            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new()
                    {
                        new FilterRule { Field = "name", Operator = FilterOperatorEnum.Contains, Value = "book" },
                        new FilterRule { Field = "price", Operator = FilterOperatorEnum.GreaterThan, Value = "50" }
                    },
                    Logic = "and"
                }
            };

            var processor = new GenericSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal(
                "WHERE name LIKE '%book%' AND price > '50'",
                sql
            );
        }

        [Fact]
        public void Should_Generate_Or_Logic_When_Requested()
        {
            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Logic = "or",
                    Rules = new()
                    {
                        new FilterRule { Field = "category", Operator = FilterOperatorEnum.Equals, Value = "Books" },
                        new FilterRule { Field = "category", Operator = FilterOperatorEnum.Equals, Value = "Electronics" }
                    }
                }
            };

            var processor = new GenericSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal(
                "WHERE category = 'Books' OR category = 'Electronics'",
                sql
            );
        }

        [Fact]
        public void Should_Generate_IN_Clause()
        {
            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new()
                    {
                        new FilterRule
                        {
                            Field = "status",
                            Operator = FilterOperatorEnum.In,
                            Value = "active,pending"
                        }
                    }
                }
            };

            var processor = new GenericSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal("WHERE status IN ('active', 'pending')", sql);
        }

        [Fact]
        public void Should_Generate_OrderBy_And_LimitOffset()
        {
            var request = new FilterRequest
            {
                Sort = new()
                {
                    new SortRule { Field = "createdAt", Direction = "desc" }
                },
                Pagination = new Pagination { Page = 2, PageSize = 10 }
            };

            var processor = new GenericSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal("ORDER BY createdAt DESC LIMIT 10 OFFSET 10", sql);
        }

        [Fact]
        public void Should_Exclude_Empty_Sections()
        {
            var request = new FilterRequest();

            var processor = new GenericSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal(string.Empty, sql);
        }
    }
}
