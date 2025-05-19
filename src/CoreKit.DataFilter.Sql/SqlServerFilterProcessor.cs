using CoreKit.DataFilter.Models;

namespace CoreKit.DataFilter.Sql
{
    public class SqlServerFilterProcessor : GenericSqlFilterProcessor
    {
        protected override string BuildPagination(Pagination? pagination)
        {
            if (pagination == null || pagination.PageSize <= 0)
                return string.Empty;

            var offset = (pagination.Page - 1) * pagination.PageSize;
            return $"OFFSET {offset} ROWS FETCH NEXT {pagination.PageSize} ROWS ONLY";
        }
    }
}
