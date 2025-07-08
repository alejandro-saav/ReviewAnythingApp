using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components;

public partial class LikedReviews : ComponentBase
{
    [Inject] private IReviewService ReviewService { get; set; }
    private IEnumerable<LikesReviewsModel> _likesReviews { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var reviews = await ReviewService.GetLikesReviewsAsync();
            if (reviews.Any())
            {
                _likesReviews = reviews;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error on initialized async: " + ex.Message);
        }
    }
}