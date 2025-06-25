using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.FollowDTOs;

public class FollowRequestDto
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public int TargetUserId { get; set; }
}