using System.Text.RegularExpressions;
using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorApp1.Components;

public partial class Explore : ComponentBase
{
    [Inject] NavigationManager Navigation { get; set; }
    [Inject] private IReviewService ReviewService { get; set; }
    private static readonly Regex TagRegex = new(@"^[a-zA-Z0-9-_]+$");
    private IEnumerable<LikesReviewsModel> reviews { get; set; } = [];
    public ExploreQueryParams QueryParams = new();
    private int TotalReviews = 0;
    private IEnumerable<Category> categories { get; set; } = [];
    public string ErrorTagMessage { get; set; } = "";
    private string NewTag { get; set; } = "";

    protected override async Task OnInitializedAsync()
    {
        ReadQuery();
        await FetchReviews();
        await FetchCategories();
    }

    private async Task FetchReviews()
    {
        try
        {
            var fetchReviews = await ReviewService.GetExplorePageReviewsAsync(QueryParams);
            if (fetchReviews.Any())
            {
                reviews = fetchReviews;
                TotalReviews = reviews.Select(r => r.Total).FirstOrDefault();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error try oninitializedasync, :" + ex);
        }
    }

    private async Task FetchCategories()
    {
        try
        {
            categories = await ReviewService.GetAllReviewCategoriesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Catch error on fetchcategories, : " + ex);
        }
    }

    private async void HandleSortChange(ChangeEventArgs e)
    {
        var selectedValue = e.Value?.ToString();
        if (!string.IsNullOrEmpty(selectedValue))
        {
            QueryParams.Sort = selectedValue;
            UpdateQueryInUrl();
            await FetchReviews();
        }
    }

    private async void HandleCategoryChange(string category)
    {
        QueryParams.Category = category;
        UpdateQueryInUrl();
        await FetchReviews();
    }

    private async void HandleRatingFilter(int rating)
    {
        QueryParams.Rating = rating;
        UpdateQueryInUrl();
        await FetchReviews();
    }

    private async void HandleTagFilter(KeyboardEventArgs e)
    {
        ErrorTagMessage = string.Empty;
        if (e.Key == "Enter" && !string.IsNullOrEmpty(NewTag))
        {
            if (!TagRegex.IsMatch(NewTag))
            {
                ErrorTagMessage = "No special characters allowed.";
                return;
            }

            if (!QueryParams.Tags.Contains(NewTag) && QueryParams.Tags.Count() < 5)
                QueryParams.Tags.Add(NewTag.Trim().ToLower());
            UpdateQueryInUrl();
            await FetchReviews();
            NewTag = "";
        }
    }

    private void UpdateQueryInUrl()
    {
        var parameters = new Dictionary<string, object?>
        {
            ["page"] = QueryParams.Page,
            ["sort"] = QueryParams.Sort,
            ["rating"] = QueryParams.Rating,
            ["Category"] = QueryParams.Category,
            ["tags"] = string.Join(",", QueryParams.Tags)
        };

        var filtered = parameters.Where(p => p.Value is not null && !string.IsNullOrEmpty(p.Value.ToString()))
            .ToDictionary(p => p.Key, p => p.Value?.ToString());

        var uri = Navigation.BaseUri + "explore" + QueryString.Create(filtered);
        Navigation.NavigateTo(uri, forceLoad: false);
    }

    private void ReadQuery()
    {
        var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);
        QueryParams.Page = query.TryGetValue("page", out var pageVal) && int.TryParse(pageVal, out var p) ? p : 1;

        QueryParams.Tags = query.TryGetValue("tag", out var tagVal)
            ? tagVal.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
            : [];

        QueryParams.Rating =
            query.TryGetValue("rating", out var rVal) && int.TryParse(rVal, out var rInt) ? rInt : null;

        QueryParams.Category = query.TryGetValue("category", out var catVal) ? catVal.ToString() : null;
        QueryParams.Sort = query.TryGetValue("sort", out var sortVal) ? sortVal.ToString() : null;
    }

    private void NavigatePage(int pageNumber)
    {
        Navigation.NavigateTo($"/explore?page={pageNumber}", forceLoad: true);
    }
}