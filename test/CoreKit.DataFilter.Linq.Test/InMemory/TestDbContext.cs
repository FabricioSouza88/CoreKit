using Microsoft.EntityFrameworkCore;
using CoreKit.DataFilter.Linq.Tests.Models;

namespace CoreKit.DataFilter.Linq.Tests.InMemory
{
    public class TestDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options) { }
    }
}
