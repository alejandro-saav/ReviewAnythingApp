using System.ComponentModel.DataAnnotations;

namespace BlazorApp1.Models;

public class UserInfoUpdateRequestDto
{
    [StringLength(30)]
    public string? FirstName { get; set; }
    [StringLength(30)]
    public string? LastName { get; set; }
    [Phone]
    public string? PhoneNumber { get; set; }
    [Url]
    [RegularExpression(@".*\.(gif|jpe?g|bmp|png)$", ErrorMessage = "URL must end with a valid image extension (.jpg, .jpeg, .png, .gif, or .bmp)")]
    [DataType(DataType.ImageUrl)]
    public string? ProfileImage { get; set; }
    [StringLength(500)]
    [DataType(DataType.MultilineText)]
    public string? Bio { get; set; }
}