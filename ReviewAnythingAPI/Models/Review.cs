using NpgsqlTypes;

namespace ReviewAnythingAPI.Models;

public class Review
{
    public int ReviewId { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastEditDate { get; set; }
    public int Rating { get; set; }
    public int? UserId { get; set; }
    public int ItemId { get; set; }
    // Property for fast search functionality
    public NpgsqlTsVector SearchVector { get; set; } = null!;
    
    // Navigation Properties
    public ApplicationUser? Creator { get; set; }
    public Item? ReviewItem { get; set; }
    public ICollection<Comment> ReviewComments { get; set; } = new List<Comment>();
    public ICollection<ReviewTag> ReviewTags { get; set; } = new List<ReviewTag>();
    public ICollection<ReviewVote> ReviewVotes { get; set; } = new List<ReviewVote>();
}