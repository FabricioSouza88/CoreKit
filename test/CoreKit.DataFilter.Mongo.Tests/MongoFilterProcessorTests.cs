using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;

namespace CoreKit.DataFilter.Mongo.Tests
{
    public class MongoFilterProcessorTests
    {
        [Fact]
        public void Should_Build_Filter_With_Equals()
        {
            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new List<FilterRule>
                    {
                        new FilterRule { Field = "status", Operator = FilterOperatorEnum.Equals, Value = "active" }
                    }
                }
            };

            var processor = new MongoFilterProcessor<Product>();
            var filter = processor.BuildFilter(request);

            Assert.NotNull(filter);
        }

        [Fact]
        public void Should_Build_Sort_Correctly()
        {
            var request = new FilterRequest
            {
                Sort = new List<SortRule>
                {
                    new SortRule { Field = "name", Direction = "desc" }
                }
            };

            var processor = new MongoFilterProcessor<Product>();
            var sort = processor.BuildSort(request);

            Assert.NotNull(sort);
        }

        [Fact]
        public void Should_Calculate_Pagination()
        {
            var request = new FilterRequest
            {
                Pagination = new Pagination { Page = 3, PageSize = 20 }
            };

            var processor = new MongoFilterProcessor<Product>();
            var (skip, limit) = processor.BuildPagination(request);

            Assert.Equal(40, skip);
            Assert.Equal(20, limit);
        }

        public class Product
        {
            public required string Name { get; set; }
            public required string Status { get; set; }
        }
    }
}
