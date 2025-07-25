using Bogus;
using System.Text.Json;
using ReviewAnythingAPI.DTOs.ReviewDTOs;
using ReviewAnythingAPI.Services.Interfaces;

public class ReviewSeederService
{
    private readonly IReviewService _reviewService;

    public ReviewSeederService(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    public async Task SeedReviewsAsync(string jsonReviews, List<int> userIds, List<int> categoryIds, int maxCount = 1000)
    {
        var reviews = JsonSerializer.Deserialize<List<ExternalReview>>(jsonReviews);
        var faker = new Faker();

        var tagPool = new[]
        {
            "Adventure", "Romance", "Fantasy", "Science", "History",
            "Horror", "Mystery", "Biography", "Politics", "Economics",
            "Self-help", "Education", "Thriller", "Classic", "Poetry",
            "Drama", "Philosophy", "Psychology", "Technology", "Travel",
            "Cooking", "Health", "Art", "Religion", "Crime",
            "Parenting", "Comics", "Memoir", "War", "Business"
        };

        int created = 0;

        foreach (var r in reviews)
        {
            if (created >= maxCount) break;
            if (string.IsNullOrWhiteSpace(r.review_text)) continue;

            var title = faker.Lorem.Sentence(5);
            var content = r.review_text.Length > 2000 ? r.review_text.Substring(0, 2000) : r.review_text;
            var rating = r.rating >= 1 && r.rating <= 5 ? r.rating : faker.Random.Int(1, 5);
            var userId = faker.PickRandom(userIds);
            var categoryId = faker.PickRandom(categoryIds);

            // Pick between 1 and 5 tags randomly
            var tags = faker.PickRandom(tagPool, faker.Random.Int(1, 5)).ToList();

            var dto = new ReviewCreateRequestDto
            {
                Title = title,
                Content = content,
                Rating = rating,
                CategoryId = categoryId,
                Tags = tags
            };

            try
            {
                await _reviewService.CreateReviewAsync(dto, userId);
                Console.WriteLine($"✅ Created review {created + 1} by user {userId} with rating {rating}");
                created++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Failed: {ex.Message}");
            }

            // Optional: throttle a bit to avoid DB overload
            if (created % 100 == 0)
                await Task.Delay(500);
        }
    }
}

public class ExternalReview
{
    public string review_text { get; set; }
    public int rating { get; set; }
}