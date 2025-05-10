namespace ReviewAnythingAPI.Models;

public class Tag
{
    public int TagId { get; set; }
    public string TagName { get; set; }
    
    public ICollection<ReviewTag> ReviewTags { get; set; }
}