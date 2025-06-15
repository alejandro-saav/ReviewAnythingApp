using BlazorApp1.Components;
using BlazorApp1.Controllers;
using BlazorApp1.Models;
using BlazorApp1.Services;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Configure Cloudinary settings
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

// Register Cloudinary as a singleton (recommended)
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<CloudinarySettings>>().Value;
    var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
    return new Cloudinary(account);
});
builder.Services.AddScoped<ImageUploadService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add service to log more information about some errors
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });


builder.Services.AddControllers();

// Custom services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();
// httpClient
string? blazorAppBaseUrl = builder.Configuration["BlazorAppBaseUrl"];
builder.Services.AddHttpClient("BlazorAppApi",client =>
{
    client.BaseAddress = new Uri(blazorAppBaseUrl);

});
    builder.Services.AddHttpClient("ReviewAnythingAPI",client =>
    {
        // This base address MUST point to your separate .NET 9 API's URL
        client.BaseAddress = new Uri(builder.Configuration["YourApi:BaseUrl"] ?? "https://localhost:5026/"); // Set a fallback URL
    });
// HttpContextAccessor for accessing the auth token
builder.Services.AddHttpContextAccessor();

//login service
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie( options =>
    {
        options.Cookie.Name = "auth_token";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
        options.LoginPath = "/login"; // Redirect here when not authenticated
    });
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
// for razor pages
builder.Services.AddRazorPages().WithRazorPagesRoot("/Components/Pages");;


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// for razor pages
app.MapRazorPages();


app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();