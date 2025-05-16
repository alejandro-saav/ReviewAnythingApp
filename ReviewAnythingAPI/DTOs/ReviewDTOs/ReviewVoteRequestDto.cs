using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.ReviewDTOs;

public class ReviewVoteRequestDto
{
    [Required]
    public int ReviewId { get; set; }
    [AllowedValues(-1,1)]
    public int VoteType { get; set; }
}