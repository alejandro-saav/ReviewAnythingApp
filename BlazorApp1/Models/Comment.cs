using BlazorApp1.Models.Auth;

namespace BlazorApp1.Models;

public class Comment
{
    public int CommentId { get; set; }
    public string Content { get; set; }
    public int ReviewId { get; set; }
    public DateTime LastEditDate { get; set; }
    public int UserId { get; set; }
    public UserCommentInformation User { get; set; } = new UserCommentInformation();
    public int Likes { get; set; }
}