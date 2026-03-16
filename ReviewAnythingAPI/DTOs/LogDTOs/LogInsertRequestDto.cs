using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.LogDTOs;

public class LogInsertRequestDto
{
    [Required]
    public required string IpAddress { get; set; }
    [Required]
    public required string UserAgent { get; set; }
    [Required]
    public required string AcceptLanguage { get; set; }
    [Required]
    public required DateTime CreatedAt { get; set; }
}