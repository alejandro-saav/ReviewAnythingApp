// using BlazorApp1.Models.Auth;
// using BlazorApp1.Services;
// using Microsoft.AspNetCore.Components;
//
// namespace BlazorApp1.Components.Pages.Auth;
//
// public partial class Login : ComponentBase
// {
//     [Inject] private IAuthService AuthService { get; set; } = default!;
//     
//     [Inject]
//     private NavigationManager Navigation { get; set; } = default!;
//     
//     public LoginRequest LoginModel { get; set; } = new LoginRequest();
//     public string? ErrorMessage { get; set; }
//     private bool IsLoading { get; set; }
//
//     private async Task HandleLogin()
//     {
//         IsLoading = true;
//         ErrorMessage = null;
//
//         try
//         {
//             var success = await AuthService.LoginAsync(LoginModel);
//
//             if (success)
//             {
//                 Navigation.NavigateTo("/");
//             }
//             else
//             {
//                 ErrorMessage = AuthService.LastErrorMessage ?? "Login failed. Please check your credentials.";
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Login error:{ex.Message}");
//             ErrorMessage = "An unexpected error occurred during login. Please try again.";
//         }
//         finally
//         {
//             IsLoading = false;
//             StateHasChanged();
//         }
//     }
//
//     private void LoginWithGoogle()
//     {
//         ErrorMessage = "Google login no implemented yet.";
//     }
// }

using BlazorApp1.Models.Auth;
using BlazorApp1.Services;
// using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components.Pages.Auth;

public partial class LoginBlazor : ComponentBase
{
    [Inject] private IAuthService AuthService { get; set; } = default!;
    
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;
    
    public LoginRequest LoginModel { get; set; } = new LoginRequest();
    public string? ErrorMessage { get; set; }
    private bool IsLoading { get; set; }

    protected override void OnInitialized()
    {
        // Clear any previous state
        LoginModel = new LoginRequest();
        ErrorMessage = null;
        IsLoading = false;
    }

    private async Task HandleLogin()
    {
        // Clear previous error messages
        ErrorMessage = null;
        IsLoading = true;
        
        // Force UI update to show loading state
        // await InvokeAsync(StateHasChanged);

        try
        {
            var success = await AuthService.LoginAsync(LoginModel);

            if (success.Success)
            {
                Navigation.NavigateTo("/test", forceLoad: true);
            }
            else
            {
                ErrorMessage = AuthService.LastErrorMessage != null ? "Internal Error please try again" : "Login failed. Please check your credentials.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            ErrorMessage = "An unexpected error occurred during login. Please try again.";
        }
        finally
        {
            IsLoading = false;
            // Force UI update
            // await InvokeAsync(StateHasChanged);
        }
    }

    private void LoginWithGoogle()
    {
        ErrorMessage = "Google login not implemented yet.";
        StateHasChanged();
    }

    // Method to clear error when user starts typing
    private void ClearErrorOnInput()
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ErrorMessage = null;
            StateHasChanged();
        }
    }
}