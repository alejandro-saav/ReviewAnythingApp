using System.Text.RegularExpressions;
using BlazorApp1.HelperClasses;
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
    private CancellationTokenSource _cts;

    protected override async Task OnInitializedAsync()
    {
        QueryParams = PaginationHelper.ReadFromUri(Navigation.ToAbsoluteUri(Navigation.Uri));
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
                StateHasChanged();
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

    private async Task HandleSortChange(ChangeEventArgs e)
    {
        var selectedValue = e.Value?.ToString();
        if (!string.IsNullOrEmpty(selectedValue))
        {
            QueryParams.Sort = selectedValue;
            UpdateQueryInUrl();
            await FetchReviews();
        }
    }

    private async Task HandleCategoryChange(string category)
    {
        QueryParams.Category = category;
        UpdateQueryInUrl();
        await FetchReviews();
    }

    private async Task HandleRatingFilter(int? rating)
    {
        QueryParams.Rating = rating;
        UpdateQueryInUrl();
        await FetchReviews();
    }

    private async Task HandleTagFilter(KeyboardEventArgs e)
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
    
    private async Task RemoveTag(string tag)
    {
        QueryParams.Tags.Remove(tag);
        UpdateQueryInUrl();
        await FetchReviews();
    }

    private async Task HandleSearchInput(ChangeEventArgs e)
    {
        QueryParams.Search = e.Value?.ToString() ?? "";
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        try
        {
            await Task.Delay(500, _cts.Token);
            UpdateQueryInUrl();
            await FetchReviews();
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine("Catch in Handle Search Input" + ex);
        }
    }

    private void UpdateQueryInUrl()
    {
        var uri = PaginationHelper.BuildUriFromParams("/explore", QueryParams);
        Navigation.NavigateTo(uri, forceLoad: false);
    }
    

    private void NavigatePage(int pageNumber)
    {
        QueryParams.Page = pageNumber;
        var pageParams = ReviewService.BuildQueryString(QueryParams);
        Navigation.NavigateTo($"/explore{pageParams}", forceLoad: true);
    }
}