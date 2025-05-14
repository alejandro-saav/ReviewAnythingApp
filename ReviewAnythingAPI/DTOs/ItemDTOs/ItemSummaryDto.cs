using System.ComponentModel.DataAnnotations;

namespace ReviewAnythingAPI.DTOs.ItemDTOs;

public class ItemSummaryDto
{ 
    // itemName, totalReviews, createdByUserId, avg rating, creationdDate
    [Required]
    public int ItemId { get; set; }
    [Required]
    public string ItemName { get; set; }
    public int TotalReviews { get; set; }
    public string? CreatedBy { get; set; }
    public int AvgRating { get; set; }
    public DateTime CreationDate { get; set; }
}