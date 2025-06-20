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
            var review = await ReviewService.GetReviewByIdAsync(ReviewId);
            if (review == null)
            {
                Navigation.NavigateTo("/");
                return;
            }
            else
            {
                CurrentReview = review;
                await GetCommentsAsync();
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
    
    private async Task GetCommentsAsync()
    {
        var commentsList = await ReviewService.GetCommentsByReviewIdAsync(CurrentReview.ReviewId);
        CurrentReview.Comments = commentsList.OrderByDescending(c => c.LastEditDate);
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
                if (vote == 1)
                {
                    CurrentReview.Likes++;
                }
                else
                {
                    CurrentReview.Likes--;
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
}