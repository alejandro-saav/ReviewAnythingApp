using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class CommentVoteRequestDto
{
    [Required]
    public int CommentId { get; set; }
    [AllowedValues(-1,1)]
    public int VoteType { get; set; }
}