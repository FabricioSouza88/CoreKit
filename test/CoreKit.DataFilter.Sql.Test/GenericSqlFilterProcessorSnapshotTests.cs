using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Sql.Tests
{
    public class GenericSqlFilterProcessorSnapshotTests
    {
        [Fact]
        public void Should_Generate_Full_Query_With_Filter_Sort_And_Pagination()
        {
            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Logic = "and",
                    Rules = new()
                    {
                        new FilterRule { Field = "status", Operator = FilterOperatorEnum.Equals, Value = "active" },
                        new FilterRule { Field = "price", Operator = FilterOperatorEnum.LessThanOrEquals, Value = "100" }
                    },
                    Groups = new()
                    {
                        new FilterGroup
                        {
                            Logic = "or",
                            Rules = new()
                            {
                                new FilterRule { Field = "category", Operator = FilterOperatorEnum.Equals, Value = "Books" },
                                new FilterRule { Field = "category", Operator = FilterOperatorEnum.Equals, Value = "Stationery" }
                            }
                        }
                    }
                },
                Sort = new()
                {
                    new SortRule { Field = "createdAt", Direction = "desc" }
                },
                Pagination = new Pagination
                {
                    Page = 2,
                    PageSize = 20
                }
            };

            var processor = new GenericSqlFilterProcessor();
            var sql = processor.BuildQuery(request);

            var expected = string.Join(" ", new[]
            {
                "WHERE status = 'active' AND price <= '100' AND",
                "(category = 'Books' OR category = 'Stationery')",
                "ORDER BY createdAt DESC",
                "LIMIT 20 OFFSET 20"
            });

            Assert.Equal(expected, sql);
        }
    }
}
