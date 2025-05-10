namespace ReviewAnythingAPI.Models;

public class ReviewTag
{
    public int ReviewId { get; set; }
    public int TagId { get; set; }
    
    // Navigation Properties
    public Review TagReview { get; set; }
    public Tag Tag { get; set; }
    //public ICollection<Tag> Tags { get; set; }
}