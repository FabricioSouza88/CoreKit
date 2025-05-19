namespace CoreKit.DataFilter.Models;

public class FilterRequest
{
    public FilterGroup Filter { get; set; } = new();
    public List<SortRule> Sort { get; set; } = new();
    public Pagination? Pagination { get; set; }
}
