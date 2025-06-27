using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace ReviewAnythingAPI.Services;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(Cloudinary cloudinary)
    {
        _cloudinary = cloudinary;
    }

    public async Task<string> UploadImageAsync(IFormFile file, int userId, string folder = "ReviewAnythingAPP")
    {
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = folder,
            PublicId = $"{folder}/{userId}",
            Overwrite = true
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        if (result.Error != null) throw new Exception(result.Error.Message);
        return result.SecureUrl.ToString();
    }
}