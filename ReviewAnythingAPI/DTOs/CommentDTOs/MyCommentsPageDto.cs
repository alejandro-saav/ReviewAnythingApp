namespace ReviewAnythingAPI.DTOs.CommentDTOs;

public class MyCommentsPageDto
{
    public int CommentId { get; set; }
    public string Content { get; set; }
    public int ReviewId { get; set; }
    public DateTime LastEditDate { get; set; }
    public int Likes { get; set; }
    public string ReviewTitle { get; set; }
    public string? UserName { get; set; }
    public string? ProfileImage { get; set; }
}