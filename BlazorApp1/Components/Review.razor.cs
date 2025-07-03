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
    [Parameter] public int ReviewId { get; set; }
    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private AuthenticationStateProvider authenticationStateProvider { get; set; }
    [Inject] private IUserService UserService { get; set; }
    [Inject] IHttpContextAccessor HttpContextAccessor { get; set; }
    private ReviewModel CurrentReview { get; set; } = new ReviewModel();
    // private UserSummary? userSummary { get; set; } = null;
    private bool IsLoggedIn;
    private string? UserProfileImage { get; set; } = "";
    private CreateComment createComment { get; set; } = new();
    private ReviewUserContextDto reviewUserContext { get; set; } = new();
    private bool showModal { get; set; }
    private bool IsLoading { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            IsLoggedIn = true;
            UserProfileImage = user.FindFirst("profile_image")?.Value;
        }
        try
        {
            if (ReviewId < 1)
            {
                Navigation.NavigateTo("/home");
                return;
            }
            var reviewPageData = await ReviewService.GetReviewPageDataAsync(ReviewId);
            if (reviewPageData == null)
            {
                Navigation.NavigateTo("/");
            }
            else
            {
                CurrentReview = reviewPageData.Review;
                CurrentReview.Comments = reviewPageData.Comments;
                if (user.Identity != null && user.Identity.IsAuthenticated)
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
    }

    private async Task PostComment()
    {
        try
        {
            IsLoading = true;
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
        if (!IsLoggedIn)
        {
            OpenModal();
            return;
        }

        try
        {
            var reviewVote = new ReviewVote
            {
                ReviewId = CurrentReview.ReviewId,
                VoteType = vote,
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
                }
                else if (vote == -1 && reviewUserContext.UserReviewVote == -1)
                {
                    reviewUserContext.UserReviewVote = null;
                }
                else if (vote == -1 && reviewUserContext.UserReviewVote == null)
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

    private async Task PostCommentVote(int vote, int commentId)
    {
        if (!IsLoggedIn)
        {
            OpenModal();
            return;
        }

        try
        {
            var commentVote = new CommentVoteRequest
            {
                CommentId = commentId,
                VoteType = vote,
                ReviewId = ReviewId,
            };
            var isSuccessVoteRequest = await ReviewService.CommentVoteAsync(commentVote);
            if (isSuccessVoteRequest)
            {
                // update commentVotesList
                var existingVote =
                    reviewUserContext.CommentVotes.FirstOrDefault(c => c.CommentId == commentVote.CommentId);
                if (existingVote != null)
                {
                    if (existingVote.UserVote == vote)
                    {
                        reviewUserContext.CommentVotes.Remove(existingVote);
                    }
                    else
                    {
                        existingVote.UserVote = vote;
                    }
                }
                else
                {
                    var newVote = new CommentVoteResponse
                    {
                        CommentId = commentVote.CommentId,
                        UserVote = vote,
                    };
                    reviewUserContext.CommentVotes.Add(newVote);
                }

                // Update CommentLikesCount
                var comment = CurrentReview.Comments.FirstOrDefault(c => c.CommentId == commentId);
                if (existingVote != null)
                {
                    if (vote == 1 && existingVote.UserVote == 1)
                    {
                        comment!.Likes--;
                    }
                    else if (vote == 1 && existingVote.UserVote == -1)
                    {
                        comment!.Likes++;
                    }
                }
                else
                {
                    if (vote == 1)
                    {
                        comment!.Likes++;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on PostReviewVote, error: {ex.Message}");
        }
    }

    private async Task FollowUser(int userId)
    {
        if (!IsLoggedIn)
        {
            OpenModal();
            return;
        }

        var followRequest = new FollowRequest
        {
            TargetUserId = userId,
        };
        if (reviewUserContext.FollowedUserIds.Contains(userId))
        {
            // Unfollow?
            var unfollowSuccess = await UserService.UnFollowUserAsync(followRequest);
            if (unfollowSuccess)
            {
                reviewUserContext.FollowedUserIds.Remove(userId);
            }
        }
        else
        {
            // follow
            var follow = await UserService.FollowUserAsync(followRequest);
            if (follow != null)
            {
                reviewUserContext.FollowedUserIds.Add(userId);
            }
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
        if (!IsLoggedIn)
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