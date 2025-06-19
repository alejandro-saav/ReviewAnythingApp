using BlazorApp1.Services;
// using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components.Pages.Auth;

public partial class ConfirmEmail : ComponentBase
{
    private bool IsSuccess { get; set; } = false;
    private string ErrorMessage { get; set; } = string.Empty;
    [Inject]
    private IAuthService AuthService { get; set; }
    [Inject]
    private NavigationManager Navigation { get; set; }
    [SupplyParameterFromQuery]
    [Parameter]
    public string? userId { get; set; }
    [SupplyParameterFromQuery]
    [Parameter]
    public string? token { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
        {
            ErrorMessage = "Verification link is invalid or incomplete";
            IsSuccess = false;
            return;
        }

        try
        {
            var success = await AuthService.ConfirmEmailAsync(userId, token);
            if (success)
            {
                IsSuccess = true;
            }
            else
            {
                ErrorMessage = "Failed to confirm email, please try again";
                IsSuccess = false;
            }
        }
        catch (Exception ex)
        {
            IsSuccess = false;
            ErrorMessage = "An error occurred during verification";
            return;
        }
    }
    private async Task ResendConfirmationEmail(){}
}