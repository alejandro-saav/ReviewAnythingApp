using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models;

public class ReviewViewModel
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; }
    
    // [Required(ErrorMessage = "Please select a star rating.")]
    public int Rating { get; set; }
    
    public int? ItemId { get; set; }
    
    public List<string> Tags { get; set; } = new List<string>();
    
    public int CategoryId { get; set; }
}