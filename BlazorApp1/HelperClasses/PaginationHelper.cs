using BlazorApp1.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace BlazorApp1.HelperClasses;

public static class PaginationHelper
{
    public static List<int> GetPaginationRange(int currentPage, int totalPages, int visiblePages = 5)
    {
        int half = visiblePages / 2;
        int start = currentPage - half;
        int end = currentPage + half;

        if (start < 1)
        {
            end += (1 - start);
            start = 1;
        }

        if (end > totalPages)
        {
            start -= (end - totalPages);
            end = totalPages;
        }

        if (start < 1)
        {
            start = 1;
        }

        var range = new List<int>();
        for (int i = start; i <= end; i++) range.Add(i);

        return range;
    }
    public static ExploreQueryParams ReadFromUri(Uri uri)
    {
        var parsed = new ExploreQueryParams();
        var query = QueryHelpers.ParseQuery(uri.Query);

        parsed.Page = query.TryGetValue("page", out var pageVal) && int.TryParse(pageVal, out var p) ? p : 1;

        parsed.Tags = query.TryGetValue("tags", out var tagVal)
            ? tagVal.ToString().Split(",", StringSplitOptions.RemoveEmptyEntries).ToList()
            : [];

        parsed.Rating = query.TryGetValue("rating", out var rVal) && int.TryParse(rVal, out var rInt) ? rInt : null;

        parsed.Category = query.TryGetValue("category", out var catVal) ? catVal.ToString() : null;
        parsed.Sort = query.TryGetValue("sort", out var sortVal) ? sortVal.ToString() : null;
        parsed.Search = query.TryGetValue("search", out var searchVal) ? searchVal.ToString() : null;

        return parsed;
    }
    
    public static string BuildUriFromParams(string basePath, ExploreQueryParams queryParams)
    {
        var parameters = new Dictionary<string, object?>
        {
            ["page"] = queryParams.Page,
            ["sort"] = queryParams.Sort,
            ["rating"] = queryParams.Rating,
            ["category"] = queryParams.Category,
            ["tags"] = string.Join(",", queryParams.Tags),
            ["search"] = queryParams.Search
        };

        var filtered = parameters
            .Where(p => p.Value is not null && !string.IsNullOrEmpty(p.Value.ToString()))
            .ToDictionary(p => p.Key, p => p.Value?.ToString());

        return basePath + QueryString.Create(filtered);
    }

}