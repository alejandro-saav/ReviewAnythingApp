using System.Security.Claims;
using BlazorApp1.Models;
using BlazorApp1.Services;
// using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorApp1.Components;

public partial class Review : ComponentBase
{
    [Inject] private IReviewService ReviewService { get; set; }
    [Parameter]
    public int ReviewId { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
    [Inject] private IUserService UserService { get; set; }
    [Inject] IHttpContextAccessor HttpContextAccessor { get; set; }
    private ReviewModel CurrentReview { get; set; } = new ReviewModel();
    private UserSummary? userSummary { get; set; } = null;
    private CreateComment createComment { get; set; } = new();
    private ReviewUserContextDto reviewUserContext { get; set; } = new();
    private bool showModal { get; set; }
    private bool IsLoading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            if (ReviewId == null || ReviewId == 0)
            {
                Navigation.NavigateTo("/");
                return;
            }
            // var review = await ReviewService.GetReviewByIdAsync(ReviewId);
            var jwt = HttpContextAccessor.HttpContext.Request.Cookies["jwt"];
            var reviewPageData = await ReviewService.GetReviewPageDataAsync(jwt, ReviewId);
            if (reviewPageData == null)
            {
                Navigation.NavigateTo("/");
                return;
            }
            else
            {
                CurrentReview = reviewPageData.Review;
                CurrentReview.Comments = reviewPageData.Comments;
                if (jwt != null)
                {
                    reviewUserContext.UserReviewVote = reviewPageData.UserReviewVote;
                    reviewUserContext.CommentVotes = reviewPageData.CommentVotes;
                    reviewUserContext.FollowedUserIds = reviewPageData.FollowedUserIds;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while trying to setting current review:" + ex.Message);
            Navigation.NavigateTo("/");
        }
    
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        string userId = "";
        if (user.Identity?.IsAuthenticated == true)
        {
            userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
        }
        if (int.TryParse(userId, out int id))
        {
            userSummary = await UserService.GetUserSummaryAsync(id);
        }
    }

    private async Task PostComment()
    {
        try
        {
            IsLoading = true;
            var jwt = HttpContextAccessor.HttpContext.Request.Cookies["jwt"];
            createComment.jwtToken = jwt;
            createComment.ReviewId = ReviewId;
            var newComment = await ReviewService.CreateCommentAsync(createComment);
            if (newComment != null)
            {
                CurrentReview.Comments = CurrentReview.Comments.Prepend(newComment).ToList();
                createComment.Content = "";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task PostReviewVote(int vote)
    {
        if (userSummary == null)
        {
            OpenModal();
            return;
        }

        try
        {
            var jwt = HttpContextAccessor.HttpContext.Request.Cookies["jwt"];
            var reviewVote = new ReviewVote
            {
                ReviewId = CurrentReview.ReviewId,
                VoteType = vote,
                jwtToken = jwt
            };
            var isSuccessVoteRequest = await ReviewService.ReviewVoteAsync(reviewVote);
            if (isSuccessVoteRequest)
            {
                if (vote == 1 && reviewUserContext.UserReviewVote == 1)
                {
                    CurrentReview.Likes--;
                    reviewUserContext.UserReviewVote = null;
                }
                else if (vote == 1 && reviewUserContext.UserReviewVote == null)
                {
                    CurrentReview.Likes++;
                    reviewUserContext.UserReviewVote = 1;
                } else if (vote == -1 && reviewUserContext.UserReviewVote == -1)
                {
                    reviewUserContext.UserReviewVote = null;
                } else if (vote == -1 && reviewUserContext.UserReviewVote == null)
                {
                    reviewUserContext.UserReviewVote = -1;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on PostReviewVote, error: {ex.Message}");
        }
    }
    
    private void OpenModal()
    {
        showModal = true;
    }
    
    private void CloseModal()
    {
        showModal = false;
    }
    
    private void HandleCommentFocus()
    {
        if (userSummary == null)
        {
            showModal = true;
        }
    }
    
    private string GetVoteClass(int commentId, int expectedVote)
    {
        var vote = reviewUserContext.CommentVotes
            .FirstOrDefault(cv => cv.CommentId == commentId);

        if (vote != null && vote.UserVote == expectedVote)
        {
            return "active";
        }

        return "";
    }
}