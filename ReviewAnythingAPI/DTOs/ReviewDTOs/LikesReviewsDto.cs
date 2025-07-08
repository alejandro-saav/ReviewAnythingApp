using ReviewAnythingAPI.DTOs.UserDTOs;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class LikesReviewsDto
{
    public int ReviewId { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public int Likes { get; set; } 
    public DateTime LastEditDate { get; set; }
    public int Rating { get; set; }
    public ICollection<string> Tags { get; set; } = [];
    public int NumberOfComments { get; set; }
    public UserSummaryDto? User { get; set; }
    public int CreatorFollowers { get; set; }
}