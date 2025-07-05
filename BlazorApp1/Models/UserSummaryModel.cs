using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp1.Models;

public class UserSummaryModel
{
    [Required]
    [StringLength(30)]
    public string FirstName { get; set; }

    [StringLength(30)] public string LastName { get; set; } = "";

    [StringLength(500)]
    [DataType(DataType.MultilineText)]
    public string Bio { get; set; } = "";

    public IBrowserFile? ProfileImage { get; set; } = null;
    public bool DeleteImage  { get; set; } = false;
}