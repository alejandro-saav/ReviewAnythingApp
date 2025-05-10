using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class ReviewUpdateRequestDto
{
    [Required]
    public int ReviewId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    
    [Required]
    public int ItemId { get; set; }
    
    public string Tags { get; set; }
    
    [Required]
    [Range(1, 6)]
    public int CategoryId { get; set; }
}