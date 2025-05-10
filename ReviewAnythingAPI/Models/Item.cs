namespace ReviewAnythingAPI.Models;

public class Item
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public int CategoryId { get; set; }
    public DateTime CreationDate { get; set; }
    public int? CreatedByUserId { get; set; }
    
    // Navigation Properties
    public ApplicationUser? Creator { get; set; }
    public Category ItemCategory { get; set; }
}