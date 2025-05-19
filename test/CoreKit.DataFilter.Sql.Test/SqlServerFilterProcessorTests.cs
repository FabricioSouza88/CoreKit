using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Sql.Tests
{
    public class SqlServerFilterProcessorTests
    {
        [Fact]
        public void Should_Generate_SQLServer_Style_Pagination()
        {
            var request = new FilterRequest
            {
                Pagination = new Pagination
                {
                    Page = 2,
                    PageSize = 10
                }
            };

            var processor = new SqlServerFilterProcessor();
            var result = processor.BuildQuery(request);

            Assert.Contains("OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY", result);
        }

        [Fact]
        public void Should_Include_Where_OrderAndPagination()
        {
            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new()
                    {
                        new FilterRule
                        {
                            Field = "category",
                            Operator = FilterOperatorEnum.Equals,
                            Value = "Books"
                        }
                    }
                },
                Sort = new()
                {
                    new SortRule { Field = "price", Direction = "desc" }
                },
                Pagination = new Pagination { Page = 1, PageSize = 5 }
            };

            var processor = new SqlServerFilterProcessor();
            var result = processor.BuildQuery(request);

            Assert.Equal(
                "WHERE category = 'Books' ORDER BY price DESC OFFSET 0 ROWS FETCH NEXT 5 ROWS ONLY",
                result
            );
        }

        [Fact]
        public void Should_Not_Include_Pagination_If_Not_Provided()
        {
            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new()
                    {
                        new FilterRule
                        {
                            Field = "name",
                            Operator = FilterOperatorEnum.Contains,
                            Value = "test"
                        }
                    }
                }
            };

            var processor = new SqlServerFilterProcessor();
            var result = processor.BuildQuery(request);

            Assert.StartsWith("WHERE name LIKE '%test%'", result);
            Assert.DoesNotContain("FETCH NEXT", result);
            Assert.DoesNotContain("OFFSET", result);
        }
    }
}
