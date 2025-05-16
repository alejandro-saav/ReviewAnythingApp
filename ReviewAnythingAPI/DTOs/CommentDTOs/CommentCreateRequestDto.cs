using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.CommentDTOs;

public class CommentCreateRequestDto
{
    [Required]
    public int ReviewId { get; set; }
    [Required]
    [MaxLength(1000)]
    public string Content { get; set; }
}