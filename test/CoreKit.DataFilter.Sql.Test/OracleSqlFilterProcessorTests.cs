using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Sql.Tests
{
    public class OracleSqlFilterProcessorTests
    {
        [Fact]
        public void Should_Use_Rownum_For_Pagination()
        {
            var request = new FilterRequest
            {
                Pagination = new Pagination { Page = 1, PageSize = 10 }
            };

            var processor = new OracleSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Contains("OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY", sql);
        }

        [Fact]
        public void Should_Handle_Equals_Operator()
        {
            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new()
                    {
                        new FilterRule { Field = "status", Operator = FilterOperatorEnum.Equals, Value = "active" }
                    }
                }
            };

            var processor = new OracleSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            Assert.Equal("WHERE status = 'active'", sql);
        }
    }
}
