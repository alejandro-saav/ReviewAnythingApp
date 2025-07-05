using BlazorApp1.Models;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorApp1.Components;

public partial class EditProfile : ComponentBase
{
    [Inject] private IUserService UserService { get; set; }
    private UserSummaryModel UserModel { get; set; } = new();
    private bool _notFound { get; set; } = false;
    private IBrowserFile? ImageFile { get; set; } = null;
    public string? ImageErrorMessage { get; set; }
    private string? ImagePreviewUrl { get; set; }
    private bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var response = await UserService.GetUserSummaryAsync();
            if (response == null)
            {
                _notFound = true;
                return;
            }
            UserModel.FirstName = response.FirstName;
            UserModel.LastName = response.LastName;
            UserModel.Bio = response.Bio;
            ImagePreviewUrl = response.ProfileImage;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on OnInitializedAsync, error: {ex.Message}");
            _notFound = true;
        }
    }

    private async Task HandleProfileEdit()
    {
        ErrorMessage = null;
        IsLoading = true;
        try
        {
            var newUserSummary = await UserService.UpdateUserSummaryAsync(UserModel);
            if (newUserSummary != null)
            {
               Console.WriteLine($"Success");
            }
            else
            {
                ErrorMessage = $"An error occured while updating user profile, please try again.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on HandleProfileEdit, error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
    
    private void HandleRemovePhoto()
    {
        ImageFile = null;
        ImageErrorMessage = null;
        ImagePreviewUrl =  null;
        UserModel.DeleteImage = true;
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
        UserModel.ProfileImage = file;
        UserModel.DeleteImage = false;

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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while loading the image: {ex.Message}");
        }
    }
}