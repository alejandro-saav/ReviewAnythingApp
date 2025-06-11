namespace BlazorApp1.Models;

public class ReviewModel
{
    public int ReviewId { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public int Likes { get; set; } 
    public DateTime CreationDate { get; set; }
    public DateTime LastEditDate { get; set; }
    public int Rating { get; set; }
    public string? UserName { get; set; } = "";
    public int? UserId { get; set; }
    public int ItemId { get; set; }
    public ICollection<string> Tags { get; set; } = [];
    public IEnumerable<Comment> Comments { get; set; } = [];
}