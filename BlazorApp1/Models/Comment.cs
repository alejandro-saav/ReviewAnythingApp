namespace BlazorApp1.Models;

public class Comment
{
    public int CommentId { get; set; }
    public string Content { get; set; }
    public int ReviewId { get; set; }
    public DateTime LastEditedDate { get; set; }
}