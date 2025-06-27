using Resend;
using ReviewAnythingAPI.Models;

namespace ReviewAnythingAPI.Services;

public class EmailService
{
    private readonly IConfiguration _configuration;
    private readonly ResendClient _resendClient;

    public EmailService(IConfiguration configuration, ResendClient resendClient)
    {
        _configuration = configuration;
        _resendClient = resendClient;
    }

    public async Task SendEmailConfirmationAsync(ApplicationUser user, string encodedToken)
    {
        var frontendUrl = _configuration["FrontendUrls:ConfirmEmailUrl"];
        var link = $"{frontendUrl}?userId={user.Id}&token={encodedToken}";

        var html = $@"
        <div style=""font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:1.6;color:#333;background:#f9f9f9;padding:20px;border-radius:8px;"">
            <h2 style=""color:#222;"">Welcome to <span style=""color:#007bff;"">ReviewAnything</span>!</h2>
            <p>Hi {user.FirstName},</p>
            <p>Thanks for signing up. Please confirm your email address by clicking the button below:</p>
            <p style=""text-align:center;"">
                <a href=""{link}"" 
                   style=""background-color:#007bff;color:#fff;padding:12px 20px;text-decoration:none;
                          border-radius:5px;display:inline-block;font-weight:bold;"">
                    Confirm Email
                </a>
            </p>
            <p>If the button above doesn't work, copy and paste this link into your browser:</p>
            <p><a href=""{link}"" style=""color:#007bff;"">{link}</a></p>
            <p>If you didn’t create this account, you can ignore this email.</p>
            <p style=""margin-top:30px;"">— The ReviewAnything Team</p>
        </div>";

        var message = new EmailMessage
        {
            From = "onboarding@resend.dev",
            Subject = "ReviewAnything Verification Email",
            HtmlBody = html
        };
        message.To.Add(user.Email!);

        await _resendClient.EmailSendAsync(message);
    }
}