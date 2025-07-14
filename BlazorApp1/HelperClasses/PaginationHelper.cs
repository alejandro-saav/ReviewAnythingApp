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
}