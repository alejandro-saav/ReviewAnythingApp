using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components;

public partial class Review : ComponentBase
{
    [Inject] private IReviewService ReviewService { get; set; }
    [Inject] private IUserService UserService { get; set; }
    [SupplyParameterFromQuery]
    [Parameter]
    public int ReviewId { get; set; }
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    private ReviewModel CurrentReview { get; set; } = null;

    protected override async Task OnInitializedAsync()
    {
        if (ReviewService.CreatedReview != null)
        {
            CurrentReview = ReviewService.CreatedReview;
        }
        else
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
            }
        } 
    }

    private async Task GetCommentsAsync()
    { 
        var comments = await ReviewService.GetCommentsByReviewIdAsync(CurrentReview.ReviewId);
    }
}