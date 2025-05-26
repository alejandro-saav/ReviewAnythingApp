using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Resend;
using ReviewAnythingAPI.DTOs.AuthDTOs;
using ReviewAnythingAPI.DTOs.UserDTOs;
using ReviewAnythingAPI.HelperClasses.CustomExceptions;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services.Interfaces;

namespace ReviewAnythingAPI.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IResend _resend;

    public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration, IResend resend)
    {
        _userManager = userManager;
        _configuration = configuration;
        _resend = resend;
    }

    public async Task<AuthResponseDto> RegisterUserAsync(UserRegistrationRequestDto userRegistrationDto)
    {
        // Check if user already exists
        var existingUserByUsername = await _userManager.FindByNameAsync(userRegistrationDto.UserName);
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
            UserName = userRegistrationDto.UserName,
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
        //var token = await GenerateJwtToken(user);
        // Email verification
        var verificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = WebUtility.UrlEncode(verificationToken);
        var verificationEmail = $"{_configuration["FrontendUrls:ConfirmEmailUrl"]}?userId={user.Id}&token={encodedToken}";
        var message = new EmailMessage();
        message.From = "onboarding@resend.dev";
        message.To.Add(user.Email);
        message.Subject = "ReviewAnything Verification Email";
        message.HtmlBody = $@"
    <div style=""font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:1.6;color:#333;background:#f9f9f9;padding:20px;border-radius:8px;"">
        <h2 style=""color:#222;"">Welcome to <span style=""color:#007bff;"">ReviewAnything</span>!</h2>
        
        <p>Hi {user.FirstName},</p>
        
        <p>Thanks for signing up. To get started, please confirm your email address by clicking the button below:</p>

        <p style=""text-align:center;"">
            <a href=""{verificationEmail}"" 
               style=""background-color:#007bff;color:#fff;padding:12px 20px;text-decoration:none;
                      border-radius:5px;display:inline-block;font-weight:bold;"">
                Confirm Email
            </a>
        </p>

        <p>If the button above doesn't work, copy and paste this link into your browser:</p>
        <p><a href=""{verificationEmail}"" style=""color:#007bff;"">{verificationEmail}</a></p>

        <p>If you didn’t create this account, you can safely ignore this email.</p>

        <p style=""margin-top:30px;"">— The ReviewAnything Team</p>
    </div>";

        await _resend.EmailSendAsync(message);
        return new AuthResponseDto
        {
            Success = true,
            Message = "Registration Successful! Please confirm your email by clicking on the link sent to your email address.",
            UserResponse = new UserResponseDto {
                UserId = user.Id,
                UserName = user.UserName,
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

    public async Task<AuthResponseDto> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new EntityNotFoundException("User not found!");
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Unable to confirm your email: {user.Email}");
        }

        var newJwtToken = await GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Success = true,
            Token = newJwtToken,
            UserResponse = new UserResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
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
                UserName = user.UserName,
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

    public async Task<AuthResponseDto> GoogleSignInAsync(string idTokenString)
    {
        var googleClientId = _configuration["Google:ClientId"];
        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idTokenString, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { googleClientId},
            });
        }
        catch (InvalidJwtException)
        {
            return new AuthResponseDto { Success = false, ErrorMessage = "Invalid Google Token" };
        }
        
        // Token is valid, extract user information
        var userEmail = payload.Email;
        var googleUserId = payload.Subject;
        var firstName = payload.GivenName;
        var lastName = payload.FamilyName;
        var profilePicture = payload.Picture;
        var emailVerified = payload.EmailVerified;
        if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(googleUserId))
        {
            return new AuthResponseDto { Success = false, ErrorMessage = "Required information not provided by Google." };
        }
        
        // Use ASP.NET Core Identity's external login mechanism
        var loginInfo = new UserLoginInfo("Google", googleUserId, "Google");

        var user = await _userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

        if (user == null)
        {
            user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = GenerateRandomUsername(),
                    Email = userEmail,
                    FirstName = firstName,
                    LastName = lastName,
                    ProfileImage = profilePicture,
                    EmailConfirmed = emailVerified,
                    CreationDate = DateTime.UtcNow,
                    IsActive = true
                };
                var createUserResult = await _userManager.CreateAsync(user);
                if (!createUserResult.Succeeded)
                {
                    return new AuthResponseDto { Success = false, ErrorMessage = "Could not create local user account.", Errors = createUserResult.Errors.Select(e => e.Description).ToList() };
                }
            }

            var addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
            if (!addLoginResult.Succeeded)
            {
                return new AuthResponseDto { Success = false, ErrorMessage = "Could not link google account to local user.", Errors = addLoginResult.Errors.Select(e => e.Description).ToList() };
            }
        }
        var localToken = await GenerateJwtToken(user);
        return new AuthResponseDto
        {
            Success = true,
            Token = localToken,
            UserResponse = new UserResponseDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                ProfileImage = user.ProfileImage,
                CreationDate = user.CreationDate,
                Phone = user.PhoneNumber
            }
        };
    }

    public string GenerateRandomUsername()
    {
        return "user_" + Guid.NewGuid().ToString("N").Substring(0,8);
    }
}