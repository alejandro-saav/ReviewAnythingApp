namespace ReviewAnythingAPI.DTOs.UserDTOs;

public class UserPageDataDto
{
    public UserSummaryDto UserSummary { get; set; } = new();
    public int TotalReviews { get; set; }
    public int TotalComments { get; set; }
    public IEnumerable<UserSummaryDto> Followers { get; set; } = [];
    public IEnumerable<UserSummaryDto> Following { get; set; } = [];
    public bool IsCurrentUserFollowing { get; set; } = false;
}