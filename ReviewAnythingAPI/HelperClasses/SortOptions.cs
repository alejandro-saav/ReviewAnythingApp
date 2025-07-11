namespace ReviewAnythingAPI.HelperClasses;

public class SortOptions
{
    public const string RatingAsc = "rating_asc";
    public const string RatingDesc = "rating_desc";

    public const string DateAsc = "date_asc";
    public const string DateDesc = "date_desc";

    public static readonly HashSet<string> All = new()
    {
        RatingAsc,
        RatingDesc,
        DateAsc,
        DateDesc
    };
}