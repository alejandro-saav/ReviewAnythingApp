namespace BlazorApp1.Models;

public class UserPageData
{
    public UserSummary UserSummary { get; set; } = new();
    public int TotalReviews { get; set; }
    public int TotalComments { get; set; }
    public IEnumerable<UserSummary> Followers { get; set; } = [];
    public IEnumerable<UserSummary> Following { get; set; } = [];
}