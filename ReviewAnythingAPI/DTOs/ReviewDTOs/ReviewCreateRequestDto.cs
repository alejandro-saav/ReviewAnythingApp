using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class ReviewCreateRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; }
    
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
    
    public int? ItemId { get; set; }
    
    public string Tags { get; set; }
    
    [Required]
    [Range(1, 6)]
    public int CategoryId { get; set; }
}