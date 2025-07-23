using System.Text;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Resend;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.HelperClasses;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services;
using ReviewAnythingAPI.Services.Interfaces;

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
// Cloudinary Service
builder.Services.AddSingleton<CloudinaryService>();

// Resend config
builder.Services.AddOptions();
builder.Services.AddHttpClient<ResendClient>();
builder.Services.Configure<ResendClientOptions>(options =>
{
    options.ApiToken = builder.Configuration["Resend:ApiKey"];
});
builder.Services.AddTransient<IResend, ResendClient>();

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CloudinaryService>();
// Add repositories to the container
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentVoteRepository, CommentVoteRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReviewVoteRepository, ReviewVoteRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IReviewTagRepository, ReviewTagRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
// For the generic repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
// DbContext
builder.Services.AddDbContext<ReviewAnythingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresSQL")));



// // DbContext
// builder.Services.AddDbContext<ReviewAnythingDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Seed services
builder.Services.AddHttpClient(); 
builder.Services.AddTransient<UserSeederService>();
builder.Services.AddTransient<ReviewSeederService>();

// Adding core identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password Settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign in settings
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
}).AddEntityFrameworkStores<ReviewAnythingDbContext>().AddDefaultTokenProviders(); // For password reset, email confirmation tokens etc.

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // In production set to true
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = jwtSettings["ValidAudience"],
        ValidIssuer = jwtSettings["ValidIssuer"],
        IssuerSigningKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException("JWT secret not configured."))),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // or small value if needed
    };
});

builder.WebHost.UseUrls("http://*:80");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // SEED 50 USERS;
    
    // using var scope = app.Services.CreateScope();
    // var seeder = scope.ServiceProvider.GetRequiredService<UserSeederService>();
    // await seeder.SeedUsersAsync(50);
    
    // SEED 11371 REVIEWS
    // using var scope = app.Services.CreateScope();
    // var seeder = scope.ServiceProvider.GetRequiredService<ReviewSeederService>();
    // var db = scope.ServiceProvider.GetRequiredService<ReviewAnythingDbContext>();
    //
    // var path = Path.Combine("SeedsDatabase", "reviews_sortv6.json");
    // if (!File.Exists(path))
    // {
    //     Console.WriteLine("❌ File not found: " + path);
    //     return;
    // }
    //
    // var reviewJson = File.ReadAllText(path);
    //
    // var userIds = await db.Users.Select(u => u.Id).ToListAsync();
    // var categoryIds = await db.Categories.Select(c => c.CategoryId).ToListAsync();
    //
    // await seeder.SeedReviewsAsync(reviewJson, userIds, categoryIds, 11371);
    app.MapOpenApi();
}

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();