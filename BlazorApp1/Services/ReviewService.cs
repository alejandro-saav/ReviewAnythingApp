using BlazorApp1.Models;
// using BlazorApp1.Services.Interfaces;
using Category = BlazorApp1.Models.Category;

namespace BlazorApp1.Services;

public class ReviewService : IReviewService
{
    private readonly HttpClient _httpClient;
    public string? LastErrorMessage { get; private set; }

    public ReviewService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("ReviewAnythingAPI");
    }

    public async Task<IEnumerable<Category>> GetAllReviewCategoriesAsync()
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.GetAsync("api/categories");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<Category>>();
                return content ?? [];
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error fetching categories, status code: {response.StatusCode} - {errorContent}, error message: {errorContent}";
                return [];
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = "Something went wrong";
            return [];
        }
    }

    public async Task<ReviewModel> CreateReviewAsync(ReviewViewModel review)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/reviews", review);
            if (response.IsSuccessStatusCode)
            {
                var newReview = await response.Content.ReadFromJsonAsync<ReviewModel>();
                return newReview;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error creating review, status code: {response.StatusCode} - error message:{errorContent}";
                return new ReviewModel();
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = "Something went wrong";
            return new ReviewModel();
        }
    }

    public async Task<ReviewModel?> GetReviewByIdAsync(int reviewId)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.GetAsync($"api/reviews/{reviewId}");
            if (response.IsSuccessStatusCode)
            {
                var review = await response.Content.ReadFromJsonAsync<ReviewModel>();
                return review;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error while fetching the review, status code: {response.StatusCode} - error message:{errorContent}";
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"REVIEW SERVICE CATCH: {ex.Message}");
            LastErrorMessage = "Something went wrong";
            return null;
        }
    }

    public async Task<IEnumerable<Comment>> GetCommentsByReviewIdAsync(int reviewId)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.GetAsync($"api/comment/reviews/{reviewId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<Comment>>();
                return content;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error while fetching the review, status code: {response.StatusCode} - error message:{errorContent}";
                return [];
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = "Something went wrong";
            return [];
        }
    }

    public async Task<Comment?> CreateCommentAsync(CreateComment comment)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/comment", comment);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<Comment>();
                return content;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                LastErrorMessage = $"Error while fetching the review, status code: {response.StatusCode} - error message:{errorContent}";
                return null;
            }
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Something went wrong ${ex.Message}";
            return null;
        }
    }

    public async Task<bool> ReviewVoteAsync(ReviewVote reviewVote)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/reviews/review-votes", reviewVote);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Something went wrong ${ex.Message}";
            return false;
        }
    }

    public async Task<ReviewPageData?> GetReviewPageDataAsync(int reviewId)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.GetAsync($"api/reviews/{reviewId}/page-data");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<ReviewPageData>();
                return content;
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            LastErrorMessage = $"Error while fetching the review page data in GetReviewPageDataAsync, status code: {response.StatusCode} - error message:{errorContent}";
            return null;
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Something went wrong ${ex.Message}";
            return null;
        }
    }
    
    public async Task<bool> CommentVoteAsync(CommentVoteRequest commentVote)
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/comment/comment-votes", commentVote);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Something went wrong while CommentVoteAsync Service layer ${ex.Message}";
            return false;
        }
    }

    public async Task<IEnumerable<LikesReviewsModel>> GetMyReviewsAsync(ExploreQueryParams queryParams)
    {
        LastErrorMessage = null;
        var queryString = BuildQueryString(queryParams);
        try
        {
            var response = await _httpClient.GetAsync($"api/reviews/myreviews{queryString}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<LikesReviewsModel>>();
                return content;
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            LastErrorMessage = $"Error request unsuccessfull: {response.StatusCode} - error message:{errorContent}";
            Console.WriteLine($"{LastErrorMessage}");
            return [];
        }
        catch (Exception ex)
        {
            LastErrorMessage = $"Catch exception on GetMyReviewsAsync ${ex.Message}";
            Console.WriteLine(LastErrorMessage);
            return [];
        }
    }

    public async Task<IEnumerable<LikesReviewsModel>> GetLikesReviewsAsync(ExploreQueryParams queryParams)
    {
        LastErrorMessage = null;
        var queryString = BuildQueryString(queryParams);
        try
        {
            var response = await _httpClient.GetAsync($"api/reviews/liked-reviews{queryString}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<LikesReviewsModel>>();
                return content;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            LastErrorMessage = $"Error request unsuccessfull: {response.StatusCode} - error message:{errorContent}";
            Console.WriteLine($"{LastErrorMessage}");
            return [];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error On get likes reviews service:" + ex.Message);
            return [];
        }
    }

    public async Task<IEnumerable<MyCommentsPageModel>> GetMyCommentsPage()
    {
        LastErrorMessage = null;
        try
        {
            var response = await _httpClient.GetAsync($"api/comment/mycomments-page");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<MyCommentsPageModel>>();
                return content;
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            LastErrorMessage = $"Error request unsuccessfull on GetMyCommentsPage service: {response.StatusCode} - error message:{errorContent}";
            Console.WriteLine($"{LastErrorMessage}");
            return [];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Catch error on GetMyCommentsPage service, error:" + ex.Message);
            return [];
        }
    }
    
    public string BuildQueryString(ExploreQueryParams queryParams)
    {
        var parameters = new Dictionary<string, object?>
        {
            ["page"] = queryParams.Page,
            ["sort"] = queryParams.Sort,
            ["rating"] = queryParams.Rating,
            ["category"] = queryParams.Category,
            ["tags"] = string.Join(",", queryParams.Tags),
            ["search"] = queryParams.Search,
        };

        var filtered = parameters
            .Where(p => p.Value is not null && !string.IsNullOrEmpty(p.Value.ToString()))
            .ToDictionary(p => p.Key, p => p.Value!.ToString()!);

        return QueryString.Create(filtered).ToUriComponent();
    }

    public async Task<IEnumerable<LikesReviewsModel>> GetExplorePageReviewsAsync(ExploreQueryParams queryParams)
    {
        LastErrorMessage = null;
        try
        {
            var queryString = BuildQueryString(queryParams); 
            var response = await _httpClient.GetAsync($"api/reviews/explore{queryString}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<IEnumerable<LikesReviewsModel>>();
                return content;
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            LastErrorMessage = $"Error request unsuccessfull on GetExplorePageReviewsAsync service: {response.StatusCode} - error message:{errorContent}";
            Console.WriteLine($"{LastErrorMessage}");
            return [];
        }
        catch (Exception ex)
        {
            Console.WriteLine("Catch error on GetExplorePageReviewsAsync service, error:" + ex.Message);
            return [];
        }
    }
}