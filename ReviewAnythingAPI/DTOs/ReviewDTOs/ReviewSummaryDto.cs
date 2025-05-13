using ReviewAnythingAPI.HelperClasses.ValidationClasses;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class ReviewSummaryDto
{
    public int ReviewId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastEditDate { get; set; }
    public int Rating { get; set; }
    
    public int? UserId {get; set;}
    public int ItemId { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
}