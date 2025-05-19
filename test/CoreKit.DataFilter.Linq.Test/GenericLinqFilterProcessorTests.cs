using CoreKit.DataFilter.Linq.Tests.InMemory;
using CoreKit.DataFilter.Linq.Tests.Models;
using CoreKit.DataFilter.Models;
using CoreKit.DataFilter.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace CoreKit.DataFilter.Linq.Tests
{
    public class GenericLinqFilterProcessorTests
    {
        private TestDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new TestDbContext(options);
            context.Products.AddRange(new List<Product>
            {
                new Product { Name = "Book A", Category = "Books", Price = 50, IsActive = true, CreatedAt = new DateTime(2023, 01, 01) },
                new Product { Name = "Book B", Category = "Books", Price = 30, IsActive = true, CreatedAt = new DateTime(2023, 02, 01) },
                new Product { Name = "Laptop", Category = "Electronics", Price = 3000, IsActive = false, CreatedAt = new DateTime(2023, 03, 01) },
                new Product { Name = "Smartphone", Category = "Electronics", Price = 1500, IsActive = true, CreatedAt = null },
            });
            context.SaveChanges();
            return context;
        }

        [Theory]
        [InlineData("Category", FilterOperatorEnum.Equals, "Books", 2)]
        [InlineData("Category", FilterOperatorEnum.NotEquals, "Books", 2)]
        [InlineData("Name", FilterOperatorEnum.Contains, "Book", 2)]
        [InlineData("Name", FilterOperatorEnum.StartsWith, "Book", 2)]
        [InlineData("Name", FilterOperatorEnum.EndsWith, "B", 1)]
        [InlineData("Price", FilterOperatorEnum.GreaterThan, 100, 2)]
        [InlineData("Price", FilterOperatorEnum.GreaterThanOrEquals, 1500, 2)]
        [InlineData("Price", FilterOperatorEnum.LessThan, 100, 2)]
        [InlineData("Price", FilterOperatorEnum.LessThanOrEquals, 50, 2)]
        [InlineData("Category", FilterOperatorEnum.In, "Books,Electronics", 4)]
        public void Should_Filter_By_Operator(string field, FilterOperatorEnum op, object value, int expectedCount)
        {
            var context = CreateDbContext();

            var request = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new()
                    {
                        new FilterRule
                        {
                            Field = field,
                            Operator = op,
                            Value = value.ToString()
                        }
                    }
                }
            };

            var processor = new GenericLinqFilterProcessor<Product>();
            var result = processor.ApplyFilter(context.Products, request).ToList();

            Assert.Equal(expectedCount, result.Count);
        }

        [Fact]
        public void Should_Filter_By_Null_And_NotNull()
        {
            var context = CreateDbContext();

            var nullRequest = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new()
                    {
                        new FilterRule
                        {
                            Field = "CreatedAt",
                            Operator = FilterOperatorEnum.Null
                        }
                    }
                }
            };

            var notNullRequest = new FilterRequest
            {
                Filter = new FilterGroup
                {
                    Rules = new()
                    {
                        new FilterRule
                        {
                            Field = "CreatedAt",
                            Operator = FilterOperatorEnum.NotNull
                        }
                    }
                }
            };

            var processor = new GenericLinqFilterProcessor<Product>();
            var nullResults = processor.ApplyFilter(context.Products, nullRequest).ToList();
            var notNullResults = processor.ApplyFilter(context.Products, notNullRequest).ToList();

            Assert.Single(nullResults);
            Assert.Equal(3, notNullResults.Count);
        }

        [Fact]
        public void Should_Apply_Pagination()
        {
            var context = CreateDbContext();

            var request = new FilterRequest
            {
                Pagination = new Pagination { Page = 2, PageSize = 2 }
            };

            var processor = new GenericLinqFilterProcessor<Product>();
            var result = processor.ApplyFilter(context.Products, request).ToList();

            Assert.Equal(2, result.Count);
        }

        [Theory]
        [InlineData("asc")]
        [InlineData("desc")]
        public void Should_Apply_Sorting(string direction)
        {
            var context = CreateDbContext();

            var request = new FilterRequest
            {
                Sort = new List<SortRule>
                {
                    new SortRule { Field = "Price", Direction = direction }
                }
            };

            var processor = new GenericLinqFilterProcessor<Product>();
            var result = processor.ApplyFilter(context.Products, request).ToList();

            var expectedOrder = direction == "asc"
                ? result.OrderBy(p => p.Price).Select(p => p.Id)
                : result.OrderByDescending(p => p.Price).Select(p => p.Id);

            Assert.Equal(expectedOrder, result.Select(p => p.Id));
        }
    }
}
