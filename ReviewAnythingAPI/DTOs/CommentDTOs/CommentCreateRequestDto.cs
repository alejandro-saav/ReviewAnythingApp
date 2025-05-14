namespace ReviewAnythingAPI.DTOs.CommentDTOs;

public class CommentCreateRequestDto
{
    public int ReviewId { get; set; }
    public string Content { get; set; }
}