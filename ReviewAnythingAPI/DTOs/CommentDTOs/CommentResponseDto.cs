namespace ReviewAnythingAPI.DTOs.CommentDTOs;

public class CommentResponseDto
{
    public int CommentId { get; set; }
    public string? CreatorUserName { get; set; }
    public DateTime LastEditDate { get; set; }
    public string Content { get; set; }
    public int ReviewId { get; set; }
}