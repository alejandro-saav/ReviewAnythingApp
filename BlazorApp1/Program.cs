using BlazorApp1.Components;
using BlazorApp1.Models;
using BlazorApp1.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add service to log more information about some errors
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

// local storage service
builder.Services.AddBlazoredLocalStorage();

// Custom services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();
// httpClient
string? blazorAppBaseUrl = builder.Configuration["BlazorAppBaseUrl"];
builder.Services.AddHttpClient("BlazorAppApi", client =>
{
    client.BaseAddress = new Uri(blazorAppBaseUrl);
});

// Http bearer handler
builder.Services.AddTransient<BearerTokenHandler>();

builder.Services.AddHttpClient("ReviewAnythingAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["YourApi:BaseUrl"] ?? "https://localhost:5026/");
}).AddHttpMessageHandler<BearerTokenHandler>();

// HttpContextAccessor for accessing the auth token
builder.Services.AddHttpContextAccessor();

//login service
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth_token";
        options.Cookie.MaxAge = TimeSpan.FromMinutes(30);
        options.LoginPath = "/login"; // Redirect here when not authenticated
    });
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorizationCore();
// for razor pages
builder.Services.AddRazorPages().WithRazorPagesRoot("/Components/Pages"); ;


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