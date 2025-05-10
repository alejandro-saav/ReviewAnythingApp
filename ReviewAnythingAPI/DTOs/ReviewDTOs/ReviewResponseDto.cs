namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class ReviewResponseDto
{
    public int ReviewId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime LastEditedDate { get; set; }
    public int Rating { get; set; }
    public int ItemId { get; set; }
}