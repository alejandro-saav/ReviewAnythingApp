using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models;

public class CreateComment
{
    public int ReviewId { get; set; }
    [MaxLength(1000)]
    public string Content { get; set; }
}