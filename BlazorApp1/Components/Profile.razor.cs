using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;


namespace BlazorApp1.Components;

public partial class Profile : ComponentBase
{
    [Inject] IJSRuntime JS {get; set;}
    [Inject] private IUserService UserService { get; set; }
    [Parameter] public int UserId { get; set; }

    [Inject] private NavigationManager Navigation { get; set; }
    [Inject] private AuthenticationStateProvider AuthState { get; set; }

    private UserPageData? UserData { get; set; } = null;
    private bool IsLoggedIn;
    private bool showModal { get; set; }
    private bool _notFound { get; set; } = false;

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        await JS.InvokeVoidAsync("window.scrollTo", 0, 0);
    }
}


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
                _notFound = true;
            }
            UserData = userPageDataRequest!;
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