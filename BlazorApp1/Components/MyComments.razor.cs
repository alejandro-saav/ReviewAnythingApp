using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components;

public partial class MyComments : ComponentBase
{
    [Inject] private IReviewService ReviewService { get; set; }
    private IEnumerable<MyCommentsPageModel> _myComments { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var comments = await ReviewService.GetMyCommentsPage();
            if (comments.Any())
            {
                _myComments = comments;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error on initialized async MyCommentsPage, :" + ex.Message);
        }
    }
}