using Microsoft.AspNetCore.Identity;

namespace ReviewAnythingAPI.Models;

public class ApplicationUser : IdentityUser<int>
{
    //FirstName, LastName, ProfileImage, Bio, CreationDate, IsActive
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImage { get; set; }
    public string? Bio { get; set; }
    public DateTime? CreationDate { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation Properties
    public ICollection<Item> UserItems { get; set; } = new List<Item>();
    public ICollection<Review> UserReviews { get; set; } = new List<Review>();
    public ICollection<Comment> UserComments { get; set; } = new List<Comment>();
    public ICollection<Follow> UserFollows { get; set; } = new List<Follow>();
    public ICollection<Follow> UserFollowings { get; set; } = new List<Follow>();
    public ICollection<ReviewVote> UserReviewVotes { get; set; } = new List<ReviewVote>();
    public ICollection<CommentVote> UserCommentVotes { get; set; } = new List<CommentVote>();
    public ICollection<Report> UserReports { get; set; } = new List<Report>();
}



