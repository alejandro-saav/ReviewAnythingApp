namespace BlazorApp1.Models;

public class ExploreQueryParams
{
    public int Page { get; set; } = 1;
    public int? Rating { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = [];
    public string? Sort { get; set; }
    public string? Search { get; set; }
}