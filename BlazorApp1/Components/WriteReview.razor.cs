using System.Security.Claims;
using System.Text.RegularExpressions;
using BlazorApp1.Models;
using BlazorApp1.Models.Auth;
using BlazorApp1.Services;
// using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Category = BlazorApp1.Models.Category;

namespace BlazorApp1.Components;

public partial class WriteReview : ComponentBase
{
    private static readonly Regex TagRegex = new(@"^[a-zA-Z0-9-_]+$");
    private bool AnimateRatingError;
    private string ErrorMessage = "";
    private string ErrorTagMessage = "";
    private int HoverRating;
    private string NewTag = "";
    private UserSummary? userSummary = null;

    private string SelectedCategory { get; set; }
    private ReviewViewModel ReviewModel { get; } = new();

    [Inject] private IJSRuntime JSRuntime { get; set; }

    [Inject] private IReviewService ReviewService { get; set; }
    [Inject] private IUserService UserService { get; set; }
    [Inject] IHttpContextAccessor HttpContextAccessor { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }

    private IEnumerable<Category> Categories { get; set; } = [];

    private void SetRating(int rating)
    {
        ReviewModel.Rating = rating;
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
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        string userId = "";
        if (user.Identity?.IsAuthenticated == true)
        {
            userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
        }
        if (int.TryParse(userId, out int id))
        {
            userSummary = await UserService.GetUserSummaryAsync(id);
        }
    }

    private async Task CreateNewReview()
    {
        if (ReviewModel.Rating < 1 || ReviewModel.Rating > 5)
        {
            AnimateRatingError = true;
            await Task.Delay(2000);
            AnimateRatingError = false;
            return;
        }

        try
        {
            var createdReview = await ReviewService.CreateReviewAsync(ReviewModel);
            NavigationManager.NavigateTo($"/review/{createdReview.ReviewId}");
            Console.WriteLine("SUCCESS");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            ErrorMessage = "Error while creating the review please try again.";
        }
    }

    private void SelectCategory(Category category)
    {
        SelectedCategory = category.CategoryName;
        ReviewModel.CategoryId = category.CategoryId;
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

            if (!ReviewModel.Tags.Contains(NewTag) && ReviewModel.Tags.Count < 5) ReviewModel.Tags.Add(NewTag.Trim().ToLower());
            NewTag = "";
        }
    }

    private void RemoveTag(string tag)
    {
        ReviewModel.Tags.Remove(tag);
    }
}

// {
// "reviewId": 8,
// "title": "WHAT",
// "content": "WAIT WHAT",
// "creationDate": "2025-06-08T16:51:13.8079718Z",
// "lastEditDate": "2025-06-08T16:51:13.8080361Z",
// "rating": 1,
// "userId": null,
// "itemId": 12,
// "tags": [
// "funny",
// "letsgo",
// "xd"
//     ]
// }