using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.Services.Interfaces;

public class UserSeederService
{
    private readonly IAuthService _authService;
    private readonly IHttpClientFactory _httpClientFactory;

    public UserSeederService(IAuthService authService, IHttpClientFactory httpClientFactory)
    {
        _authService = authService;
        _httpClientFactory = httpClientFactory;
    }

    public async Task SeedUsersAsync(int count = 50)
    {
        var faker = new Faker("en");
        var client = _httpClientFactory.CreateClient();

        for (int i = 0; i < count; i++)
        {
            var firstName = faker.Name.FirstName();
            var lastName = faker.Name.LastName();
            var username = faker.Internet.UserName(firstName, lastName);
            var email = faker.Internet.Email(firstName, lastName);
            var password = "Password123!";
            var phone = faker.Phone.PhoneNumber();
            var bio = faker.Lorem.Sentence();

            // Descargar imagen de perfil
            var imageUrl = $"https://i.pravatar.cc/300?img={(i % 70) + 1}";
            byte[] imageBytes;
            try
            {
                imageBytes = await client.GetByteArrayAsync(imageUrl);
            }
            catch
            {
                Console.WriteLine($"Error downloading image for user {username}");
                continue;
            }

            var stream = new MemoryStream(imageBytes);
            var formFile = new FormFile(stream, 0, stream.Length, "ProfileImage", "profile.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            var userDto = new UserRegistrationRequestDto
            {
                UserName = username,
                Email = email,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                Bio = bio,
                ProfileImage = formFile
            };

            var result = await _authService.RegisterUserAsync(userDto);

            if (result.Success)
            {
                Console.WriteLine($"✅ Created: {username}");
            }
            else
            {
                Console.WriteLine($"❌ Failed: {username} - {result.ErrorMessage}");
            }

            await Task.Delay(200); // evita saturar
        }
    }
}
