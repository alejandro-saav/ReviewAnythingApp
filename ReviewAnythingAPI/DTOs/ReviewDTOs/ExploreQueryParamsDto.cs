using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.HelperClasses;

public class ExploreQueryParamsDto
{
    public int Page { get; set; } = 1;
    public int? Rating { get; set; }
    public string? Category { get; set; }
    public List<string> Tags { get; set; } = [];
    public string? Sort { get; set; }
}