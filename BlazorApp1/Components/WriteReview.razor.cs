using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components;

public partial class WriteReview : ComponentBase
{
    private string SelectedCategory { get; set; }
    
    private int Rating = 0;
    private int HoverRating = 0;
    
    private void SetRating(int rating) => Rating = rating;
    private void SetHoverRating(int hoverRating) => HoverRating = hoverRating;
    private void ClearHover() => HoverRating = 0;

    private IEnumerable<string> Categories { get; set; } = ["Product", "Entertainment", "Place", "Service", "Software", "Experience"];

    private void SelectCategory(string category)
    {
        Console.WriteLine($"Selected category: {category}");
        SelectedCategory = category;
    }
    
}