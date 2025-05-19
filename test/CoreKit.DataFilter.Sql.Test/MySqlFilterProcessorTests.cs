using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Sql.Tests
{
    public class MySqlFilterProcessorTests
    {
        [Fact]
        public void Should_Build_Contains_With_Like()
        {
            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new()
                    {
                        new FilterRule { Field = "name", Operator = FilterOperatorEnum.Contains, Value = "test" }
                    }
                }
            };

            var processor = new MySqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal("WHERE name LIKE '%test%'", sql);
        }

        [Fact]
        public void Should_Build_Pagination_With_Limit_Offset()
        {
            var request = new FilterRequest
            {
                Pagination = new Pagination { Page = 3, PageSize = 25 }
            };

            var processor = new MySqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal("LIMIT 25 OFFSET 50", sql);
        }
    }
}
