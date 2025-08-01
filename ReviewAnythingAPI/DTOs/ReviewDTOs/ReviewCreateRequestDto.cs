using System.ComponentModel.DataAnnotations;
using ReviewAnythingAPI.HelperClasses.ValidationClasses;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class ReviewCreateRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(10000)]
    public string Content { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    
    public int? ItemId { get; set; }

    [MaxListLimit(5, ErrorMessage = "You can add a maximum of 5 tags.")]
    public List<string> Tags { get; set; } = new List<string>();
    
    [Required]
    [Range(1, 6)]
    public int CategoryId { get; set; }
}