using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components;

public partial class Profile : ComponentBase
{
    [Inject] private IUserService UserService { get; set; }
    [Parameter] public int UserId { get; set; }

    [Inject] private NavigationManager Navigation { get; set; }

    private UserPageData UserData { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var userPageDataRequest = await UserService.GetUserPageDataAsync(UserId);
            if (userPageDataRequest == null)
            {
                Navigation.NavigateTo("/");
            }
            UserData = userPageDataRequest!;
            Console.WriteLine($"HELLO: {UserData.UserSummary.CreationDate}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on initialized: {ex.Message}");
        }
    }

    private async Task FollowUser(int userId)
    {
        if (userSummary == null)
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
} 