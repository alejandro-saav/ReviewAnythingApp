using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorApp1.Components;

public partial class WriteReview : ComponentBase
{
    private string SelectedCategory { get; set; }
    private string NewTag = "";
    private List<string> Tags = [];
    
    [Inject]
    private IReviewService ReviewService { get; set; }
    
    private int Rating = 0;
    private int HoverRating = 0;
    
    private void SetRating(int rating) => Rating = rating;
    private void SetHoverRating(int hoverRating) => HoverRating = hoverRating;
    private void ClearHover() => HoverRating = 0;

    private IEnumerable<Category> Categories { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var response = await ReviewService.GetAllReviewCategoriesAsync();
        Categories = response;
    }
    
    private void SelectCategory(string category)
    {
        Console.WriteLine($"Selected category: {category}");
        SelectedCategory = category;
    }

    private void HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !string.IsNullOrEmpty(NewTag))
        {
            if (!Tags.Contains(NewTag) && Tags.Count < 5)
            {
                Tags.Add(NewTag.Trim());
            }
            NewTag = "";
        }
    }

    private void RemoveTag(string tag)
    {
        Tags.Remove(tag);
    }
    
}