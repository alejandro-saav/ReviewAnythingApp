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
    public ICollection<Item> UserItems { get; set; }
    public ICollection<Review> UserReviews { get; set; }
    public ICollection<Comment> UserComments { get; set; }
    public ICollection<Follow> UserFollows { get; set; }
    public ICollection<Follow> UserFollowings { get; set; }
    public ICollection<ReviewVote> UserReviewVotes { get; set; }
    public ICollection<CommentVote> UserCommentVotes { get; set; }
    public ICollection<Report> UserReports { get; set; }
}



