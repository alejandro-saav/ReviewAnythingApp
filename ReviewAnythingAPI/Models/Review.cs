using NpgsqlTypes;

namespace ReviewAnythingAPI.Models;

public class Review
{
    public int ReviewId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastEditDate { get; set; }
    public int Rating { get; set; }
    public int? UserId { get; set; }
    public int ItemId { get; set; }
    // Property for fast search functionality
    public NpgsqlTsVector SearchVector { get; set; }
    
    // Navigation Properties
    public ApplicationUser? Creator { get; set; }
    public Item ReviewItem { get; set; }
    public ICollection<Comment> ReviewComments { get; set; }
    public ICollection<ReviewTag> ReviewTags { get; set; }
    public ICollection<ReviewVote> ReviewVotes { get; set; }
}