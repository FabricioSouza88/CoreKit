namespace CoreKit.DataFilter.Models
{
    public class SimpleQueryRequest
    {
        public string? FilterQuery { get; set; } // ex: category:eq:Books;price:gt:100
        public string? Sort { get; set; }        // ex: "price"
        public string? SortDir { get; set; }     // "asc" or "desc"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public FilterRequest ToFilterRequest()
        {
            var filter = string.IsNullOrWhiteSpace(FilterQuery)
                ? new FilterGroup()
                : FilterQueryParser.Parse(FilterQuery);

            var sortRules = new List<SortRule>();

            if (!string.IsNullOrWhiteSpace(Sort))
            {
                sortRules.Add(new SortRule
                {
                    Field = Sort,
                    Direction = SortDir?.ToLower() == "desc" ? "desc" : "asc"
                });
            }

            return new FilterRequest
            {
                Filter = filter,
                Sort = sortRules,
                Pagination = new Pagination
                {
                    Page = this.Page,
                    PageSize = this.PageSize
                }
            };
        }
    }
}
