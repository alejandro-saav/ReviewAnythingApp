using BlazorApp1.Models.Auth;
using BlazorApp1.Services;
// using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components.Pages.Auth;

public partial class ForgotPassword : ComponentBase
{
    [Inject] private IAuthService AuthService {get; set;} = default!;
    
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    public ForgotPasswordRequest ForgotPasswordModel { get; set; } = new ForgotPasswordRequest();
    
    private string? ErrorMessage { get; set; }
    private bool IsLoading { get; set; }
    
    private bool IsSuccess { get; set; }

    protected override void OnInitialized()
    {
        ForgotPasswordModel = new ForgotPasswordRequest();
        ErrorMessage = null;
        IsLoading = false;
    }

    private async Task HandleResetPasswordRequest()
    {
        ErrorMessage = null;
        IsLoading = true;

        try
        {
            var success = await AuthService.ForgotPasswordAsync(ForgotPasswordModel);

            if (success)
            {
                IsSuccess = true;
            }
            else
            {
                IsSuccess = false;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "An unexpected error occured, please try again.";
        }
        finally
        {
            IsLoading = false;
        }
    }
}