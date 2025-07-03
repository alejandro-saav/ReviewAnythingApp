using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp1.Components;

public partial class Profile : ComponentBase
{
    [Inject] private IUserService UserService { get; set; }
    [Parameter] public int UserId { get; set; }

    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private AuthenticationStateProvider AuthState { get; set; }

    private UserPageData UserData { get; set; } = new();
    private bool IsLoggedIn;
    private bool showModal { get; set; }


    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthState.GetAuthenticationStateAsync();
        var user = authState.User;
        IsLoggedIn = user.Identity != null && user.Identity.IsAuthenticated;
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

    private async Task FollowUser()
    {
        if (!IsLoggedIn)
        {
            OpenModal();
            return;
        }
        var followRequest = new FollowRequest
        {
            TargetUserId = UserId,
        };
        if (UserData.IsCurrentUserFollowing)
        {
            // Unfollow?
            var unfollowSuccess = await UserService.UnFollowUserAsync(followRequest);
            if (unfollowSuccess)
            {
                UserData.IsCurrentUserFollowing = false;
            }
        }
        else
        {
            // follow
            var follow = await UserService.FollowUserAsync(followRequest);
            if (follow != null)
            {
                UserData.IsCurrentUserFollowing = true;
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
} 