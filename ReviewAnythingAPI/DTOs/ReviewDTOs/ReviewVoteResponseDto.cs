using ReviewAnythingAPI.Enums;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class ReviewVoteResponseDto
{
    public int ReviewId { get; set; }
    public int UserVote { get; set; }
    public ActionType ActionType { get; set; }
    //public int TotalVotes { get; set; }
}