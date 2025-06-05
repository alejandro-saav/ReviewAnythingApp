using System.Text.RegularExpressions;
using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorApp1.Components;

public partial class WriteReview : ComponentBase
{
    private static readonly Regex TagRegex = new(@"^[a-zA-Z0-9-_]+$");
    private bool AnimateRatingError;
    private string ErrorMessage = "";
    private string ErrorTagMessage = "";
    private int HoverRating;
    private string NewTag = "";

    private int Rating;
    private readonly List<string> Tags = [];
    private string SelectedCategory { get; set; }
    private ReviewViewModel ReviewModel { get; } = new();

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IReviewService ReviewService { get; set; }

    private IEnumerable<Category> Categories { get; set; } = [];

    private void SetRating(int rating)
    {
        Rating = rating;
    }

    private void SetHoverRating(int hoverRating)
    {
        HoverRating = hoverRating;
    }

    private void ClearHover()
    {
        HoverRating = 0;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            // Call the JavaScript function once the component is rendered
            await JSRuntime.InvokeVoidAsync("blazorHelpers.preventEnterKeySubmission", "tag-input");
    }

    protected override async Task OnInitializedAsync()
    {
        var response = await ReviewService.GetAllReviewCategoriesAsync();
        Categories = response;
    }

    private async Task CreateNewReview()
    {
        if (Rating < 1 || Rating > 5)
        {
            AnimateRatingError = true;
            await Task.Delay(2000);
            AnimateRatingError = false;
            return;
        }

        try {
            var createdReview = await ReviewService.CreateReviewAsync(ReviewModel);
            Console.WriteLine("SUCCESS");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            ErrorMessage = "Error while creating the review please try again.";
        }
    }

    private void SelectCategory(string category)
    {
        SelectedCategory = category;
    }

    private void HandleKeyDown(KeyboardEventArgs e)
    {
        ErrorTagMessage = string.Empty;
        if (e.Key == "Enter" && !string.IsNullOrEmpty(NewTag))
        {
            if (!TagRegex.IsMatch(NewTag))
            {
                ErrorTagMessage = "No special characters allowed.";
                return;
            }

            if (!Tags.Contains(NewTag) && Tags.Count < 5) Tags.Add(NewTag.Trim().ToLower());
            NewTag = "";
        }
    }

    private void RemoveTag(string tag)
    {
        Tags.Remove(tag);
    }
}