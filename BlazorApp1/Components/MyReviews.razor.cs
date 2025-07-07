using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components;

public partial class MyReviews : ComponentBase
{
    [Inject] private IReviewService ReviewService { get; set; }
    private IEnumerable<MyReviewsModel> myReviews {get; set;} = [];

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var reviews = await  ReviewService.GetMyReviewsAsync();
            if (reviews.Any())
            {
                myReviews = reviews;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception caugth oninitializedasync method:" + ex.Message);
        }
    }
}