using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace ReviewAnythingAPI.DTOs.UserDTOs;

public class UserUpdateSummaryDto
{
    [Required]
    [StringLength(30)]
    public string FirstName { get; set; }
    [StringLength(30)]
    public string? LastName { get; set; }
    [StringLength(500)]
    [DataType(DataType.MultilineText)]
    public string? Bio { get; set; }
    public IFormFile? ProfileImage { get; set; }
}