using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components;

public partial class Review : ComponentBase
{
    [Inject] private IReviewService ReviewService { get; set; }
    [Parameter]
    public int ReviewId { get; set; }
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    private ReviewModel CurrentReview { get; set; } = new ReviewModel();

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
    }

    private async Task GetCommentsAsync()
    { 
        var commentsList = await ReviewService.GetCommentsByReviewIdAsync(CurrentReview.ReviewId);
        CurrentReview.Comments = commentsList;
    }
}