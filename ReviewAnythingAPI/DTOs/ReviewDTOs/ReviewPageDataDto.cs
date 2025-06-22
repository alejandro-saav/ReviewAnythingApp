
using ReviewAnythingAPI.DTOs.CommentDTOs;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class ReviewPageDataDto
{
    public ReviewDetailDto Review { get; set; } = new();
    public IEnumerable<CommentResponseDto> Comments { get; set; } = [];
    public int? UserReviewVote { get; set; } = null;
    public IEnumerable<CommentVoteResponseDto> CommentVotes { get; set; } = [];
    public IEnumerable<int> FollowedUserIds { get; set; } = [];
}