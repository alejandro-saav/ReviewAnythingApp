using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterUserAsync(UserRegistrationRequestDto userRegistrationDto)
    {
        // Check if user already exists
        var existingUserByUsername = await _userManager.FindByNameAsync(userRegistrationDto.Username);
        if (existingUserByUsername != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                ErrorMessage = "Username already exists!"
            };
        }

        var existingUserByEmail = await _userManager.FindByEmailAsync(userRegistrationDto.Email);
        if (existingUserByEmail != null)
        {
            return new AuthResponseDto
            {
                Success = false,
                ErrorMessage = "Email already exists!"
            };
        }
        
        var user = new ApplicationUser
        {
            UserName = userRegistrationDto.Username,
            Email = userRegistrationDto.Email,
            FirstName = userRegistrationDto.FirstName,
            LastName = userRegistrationDto.LastName,
            Bio = userRegistrationDto.Bio,
            CreationDate = DateTime.UtcNow,
            IsActive = true,
            PhoneNumber = userRegistrationDto.Phone,
            ProfileImage = userRegistrationDto.ProfileImage,
        };
        
        var result = await _userManager.CreateAsync(user, userRegistrationDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            return new AuthResponseDto
            {
                Success = false,
                ErrorMessage = "User registration failed!",
                Errors = errors
            };
        }

        // Generate Token
        var token = await GenerateJwtToken(user);
        return new AuthResponseDto
        {
            Success = true,
            Token = token,
            UserResponse = new UserResponseDto {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                ProfileImage = user.ProfileImage,
                CreationDate = user.CreationDate,
                Phone = user.PhoneNumber
            },
        };
    }

    public async Task<AuthResponseDto> LoginUserAsync(UserLoginRequestDto userLoginDto)
    {
        // Find user
        var user = await _userManager.FindByEmailAsync(userLoginDto.Email);
        if (user == null || !user.IsActive)
        {
            return new AuthResponseDto
            {
                Success = false,
                ErrorMessage = "Invalid email or password!"
            };
        }
        
        // Check if account is locked out
        if (await _userManager.IsLockedOutAsync(user))
        {
            return new AuthResponseDto
            {
                Success = false,
                ErrorMessage = "Account is locked out!"
            };
        }
        
        // Verify password
        var passwordValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password);
        if (!passwordValid)
        {
            await _userManager.AccessFailedAsync(user);
            return new AuthResponseDto
            {
                Success = false,
                ErrorMessage = "Invalid email or password!"
            };
        }
        // if login success reset access failed count
        await _userManager.ResetAccessFailedCountAsync(user);
        // Generate Token
        var token = await GenerateJwtToken(user);
        return new AuthResponseDto
        {
            Success = true,
            Token = token,
            UserResponse = new UserResponseDto
            {
                UserId = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                ProfileImage = user.ProfileImage,
                CreationDate = user.CreationDate,
                Phone = user.PhoneNumber
            },
        };
    }

    public async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT secret not configured");
        var expiryInMinutes = Convert.ToInt32(jwtSettings["ExpiryInMinutes"]);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        };
        
        var customUserClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(customUserClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiryInMinutes),
            SigningCredentials = creds,
            Issuer = jwtSettings["ValidIssuer"],
            Audience = jwtSettings["ValidAudience"],
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}