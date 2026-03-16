using BlazorApp1.Models.Auth;
using BlazorApp1.Services;
// using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components.Pages.ResetPassword;

public partial class ResetPassword : ComponentBase
{
    private bool IsSuccess { get; set; } = false;
    private bool IsLoading { get; set; } = false;
    private string ErrorMessage { get; set; } = string.Empty;
    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;
    [SupplyParameterFromQuery]
    [Parameter]
    public string? token { get; set; }
    [SupplyParameterFromQuery]
    [Parameter]
    public string? userId { get; set; }
    public ResetPasswordViewModel ResetPasswordModel { get; set; } = new ResetPasswordViewModel();

    private async Task ResetPasswordHandler()
    {
        ErrorMessage = string.Empty;
        IsLoading = true;
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            ErrorMessage = "Invalid or expired reset link.";
            IsLoading = false;
            return;
        }
        try
        {
            var success = await AuthService.ResetPasswordAsync(userId!, token!, ResetPasswordModel.NewPassword);
            if (success)
            {
                IsSuccess = true;
            }
            else
            {
                IsSuccess = false;
                ErrorMessage = "Failed to reset password";
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Internal Server Error";
            IsSuccess = false;
            return;
        }
        finally
        {
            IsLoading = false;
        }
    }
}