using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Sql.Tests
{
    public class PostgreSqlFilterProcessorTests
    {
        [Fact]
        public void Should_Use_ILike_For_Contains()
        {
            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new()
                    {
                        new FilterRule { Field = "name", Operator = FilterOperatorEnum.Contains, Value = "book" }
                    }
                }
            };

            var processor = new PostgreSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal("WHERE name ILIKE '%book%'", sql);
        }

        [Fact]
        public void Should_Build_IN_Clause()
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

            var processor = new PostgreSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal("WHERE status IN ('active', 'pending')", sql);
        }

        [Fact]
        public void Should_Build_OrderBy_And_LimitOffset()
        {
            var request = new FilterRequest
            {
                Sort = new()
                {
                    new SortRule { Field = "createdAt", Direction = "asc" }
                },
                Pagination = new Pagination { Page = 2, PageSize = 10 }
            };

            var processor = new PostgreSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal("ORDER BY createdAt ASC LIMIT 10 OFFSET 10", sql);
        }
    }
}
