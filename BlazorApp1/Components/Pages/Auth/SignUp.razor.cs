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
    public string? ImageErrorMessage { get; set; }
    private string? ImageNameSelected { get; set; }
    
    private string? ImagePreviewUrl { get; set; }
    private bool IsLoading { get; set; }

    protected override void OnInitialized()
    {
        // Clear any previous state
        SignUpModel = new RegisterRequestDto();
        ErrorMessage = null;
        IsLoading = false;
    }

    private void HandleRemovePhoto()
    {
        ImageNameSelected = "";
        ImagePreviewUrl = "";
        Console.WriteLine("HELLO?");
    }

    private async Task HandleImageSelected(InputFileChangeEventArgs e)
    {
        ImageErrorMessage = "";
        const long maxFileSize = 1024 * 1024 * 2;
        var file = e.File;

        if (file.Size > maxFileSize)
        {
            Console.WriteLine("File is too big");
            ImageErrorMessage = "File is too big, max size is 2MB";
            return;
        }

        try
        {
            await using var stream = file.OpenReadStream(maxFileSize);
            var buffer = new byte[file.Size];
            //await file.OpenReadStream().ReadAsync(buffer);
            await stream.ReadExactlyAsync(buffer);
            var base64 = Convert.ToBase64String(buffer);
            var imageType = file.ContentType;
            ImagePreviewUrl = $"data:{imageType};base64,{base64}";
            var fileName = file.Name;
            if (fileName.Length <= 15)
            {
                ImageNameSelected = fileName;
            }
            else
            {
                var extension = Path.GetExtension(fileName); // incluye el punto, ej: ".png"
                var nameOnly = Path.GetFileNameWithoutExtension(fileName);
                var shortened = nameOnly.Substring(0, 10); // primeros 10 caracteres
                ImageNameSelected = $"{shortened}...{extension}";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while loading the image: {ex.Message}");
        }
    }

    private async Task HandleSignUp()
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
                Navigation.NavigateTo("/email-confirmation-required", forceLoad: true);
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