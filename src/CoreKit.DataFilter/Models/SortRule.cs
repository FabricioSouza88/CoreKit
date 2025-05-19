namespace CoreKit.DataFilter.Models;

public class SortRule
{
    public string Field { get; set; } = string.Empty;
    public string Direction { get; set; } = "asc"; // asc | desc
}
