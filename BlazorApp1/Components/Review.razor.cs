using System.Security.Claims;
using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp1.Components;

public partial class Review : ComponentBase
{
    [Inject] private IReviewService ReviewService { get; set; }
    [Parameter]
    public int ReviewId { get; set; }
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
    [Inject] private IUserService UserService { get; set; }
    private ReviewModel CurrentReview { get; set; } = new ReviewModel();
    private UserSummary? userSummary { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (ReviewId == null || ReviewId == 0)
            {
                Navigation.NavigateTo("/element-not-found");
                return;
            }
            var review = await ReviewService.GetReviewByIdAsync(ReviewId);
            if (review == null)
            {
                Navigation.NavigateTo("/element-not-found");
            }
            else
            {
                CurrentReview = review;
                await GetCommentsAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while trying to setting current review:" + ex.Message);
            Navigation.NavigateTo("/element-not-found");
        }

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

    private async Task GetCommentsAsync()
    {
        var commentsList = await ReviewService.GetCommentsByReviewIdAsync(CurrentReview.ReviewId);
        CurrentReview.Comments = commentsList;
    }
}