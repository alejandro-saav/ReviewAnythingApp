using BlazorApp1.Models;
using BlazorApp1.Models.Auth;
using BlazorApp1.Services;
// using BlazorApp1.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp1.Components.Pages.Auth;

public partial class SignUp : ComponentBase
{
    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;
    
    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    public RegisterRequestDto SignUpModel { get; set; } = new RegisterRequestDto();
    public string? ErrorMessage { get; set; }
    public string? ImageErrorMessage { get; set; }
    private string? ImageNameSelected { get; set; }
    
    private string? ImagePreviewUrl { get; set; }
    private IBrowserFile? ImageFile { get; set; } = null;
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
        ImageFile = null;
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
        ImageFile = file;
        SignUpModel.ProfileImage = file;

        // Manage the preview image display
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
                // var uploadCloudinarySuccess = await UploadImageToCloudinary();
                // if (uploadCloudinarySuccess)
                // {
                //     // var userToUpdate = new UserInfoUpdateRequestDto { ProfileImage = SignUpModel.ProfileImage };
                //     // await UserService.UpdateUserInfoAsync(userToUpdate);
                // }
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
    
    // private async Task<bool> UploadImageToCloudinary()
    // {
    //     try
    //     {
    //         if (ImageFile == null) return false;
    //         const long maxFileSize = 1024 * 1024 * 2;
    //         await using Stream stream = ImageFile.OpenReadStream(maxFileSize);
    //         var uploadParams = new ImageUploadParams()
    //         {
    //             File = new FileDescription(ImageFile.Name, stream),
    //             PublicId = $"profile-photos/{Guid.NewGuid().ToString()}", // Recommended: unique public ID
    //             Overwrite = true, // Set to true if you want to overwrite existing assets with the same public ID
    //             Folder = "ReviewAnythingAPP", // Optional: Organize uploads into a folder
    //         };
    //
    //         var uploadResult = await Cloudinary.UploadAsync(uploadParams);
    //
    //         if (uploadResult.Error == null)
    //         {
    //            SignUpModel.ProfileImage = uploadResult.SecureUrl.ToString();
    //             Console.WriteLine("SUCCESS");
    //            return true;
    //         }
    //         Console.WriteLine("Error");
    //         return false;
    //     }
    //     catch (Exception ex)
    //     {
    //         //asd
    //         return false;
    //     }
    // }
}