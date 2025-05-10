namespace ReviewAnythingAPI.Models;

public class Comment
{
    public int CommentId { get; set; }
    public string Content { get; set; }
    public int ReviewId { get; set; }
    public int? UserId { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastEditDate { get; set; }
    
    // Navigation Properties
    public Review Review { get; set; }
    public ApplicationUser? User { get; set; }
}