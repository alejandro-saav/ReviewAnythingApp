using BlazorApp1.Components;
using BlazorApp1.Controllers;
using BlazorApp1.Services;

var builder = WebApplication.CreateBuilder(args);

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
// httpClient
string? blazorAppBaseUrl = builder.Configuration["BlazorAppBaseUrl"];
builder.Services.AddHttpClient("BlazorAppApi",client =>
{
    client.BaseAddress = new Uri(blazorAppBaseUrl);

});
// builder.Services.AddHttpClient<AuthService>(client =>
// {
//     client.BaseAddress = new Uri(builder.Configuration["BlazorAppBaseUrl"] ?? "http://localhost:5032/");
// });
builder.Services.AddHttpClient("ReviewAnythingAPI",client =>
{
    // This base address MUST point to your separate .NET 9 API's URL
    client.BaseAddress = new Uri(builder.Configuration["YourApi:BaseUrl"] ?? "https://localhost:5026/"); // Set a fallback URL
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();

app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();