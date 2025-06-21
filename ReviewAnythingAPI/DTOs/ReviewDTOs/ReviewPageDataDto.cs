
using ReviewAnythingAPI.DTOs.CommentDTOs;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class ReviewPageDataDto
{
    public ReviewDetailDto Review { get; set; } = new();
    public IEnumerable<CommentResponseDto> Comments { get; set; } = [];
    public bool HasUserLikedReview { get; set; } = false;
    public IEnumerable<int> LikedCommentIds { get; set; } = [];
    public IEnumerable<int> FollowedUserIds { get; set; } = [];
}