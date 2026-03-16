namespace ReviewAnythingAPI.Models;

public class Tag
{
    public int TagId { get; set; }
    public required string TagName { get; set; }
    
    public ICollection<ReviewTag> ReviewTags { get; set; } = new List<ReviewTag>();
}