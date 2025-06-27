namespace ReviewAnythingAPI.DTOs.FollowDTOs;

public class FollowResponseDto
{
    public int UserId { get; set; }
    public int TargetUserId { get; set; }
    public DateTime FollowDate { get; set; }
}