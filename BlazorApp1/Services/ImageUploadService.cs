using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace BlazorApp1.Services;

public class ImageUploadService
{
    private readonly Cloudinary _cloudinary;

    public ImageUploadService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string?> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) return null;
        
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "",
            UseFilename = true,
            UniqueFilename = false,
            Overwrite = true,
        };
        
        var result = await _cloudinary.UploadAsync(uploadParams);
        
        return result.SecureUrl?.AbsoluteUri;
    }
}