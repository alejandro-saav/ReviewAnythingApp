using BlazorApp1.Models.Auth;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp1.Components.Pages.Auth;

public partial class SignUp : ComponentBase
{
    [Inject] private IAuthService AuthService { get; set; } = default!;
    
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;
    
    public RegisterRequestDto SignUpModel { get; set; } = new RegisterRequestDto();
    public string? ErrorMessage { get; set; }
    private bool IsLoading { get; set; }

    protected override void OnInitialized()
    {
        // Clear any previous state
        SignUpModel = new RegisterRequestDto();
        ErrorMessage = null;
        IsLoading = false;
    }

    private async Task HandleImageSelected(InputFileChangeEventArgs e)
    {
        var file = e.File;
        Console.WriteLine("File selected is:", file.Name);
    }

    private async Task HandleLogin()
    {
        // Clear previous error messages
        ErrorMessage = null;
        IsLoading = true;
        
        // Force UI update to show loading state
        await InvokeAsync(StateHasChanged);

        try
        {
            var success = await AuthService.RegisterAsync(SignUpModel);

            if (success)
            {
                Navigation.NavigateTo("/", forceLoad: true);
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
            await InvokeAsync(StateHasChanged);
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