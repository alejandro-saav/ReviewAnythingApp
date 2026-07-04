using System.Data;
using System.Reflection;
using System.Text;
using Asp.Versioning;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Npgsql;
using Resend;
using ReviewAnythingAPI.Context;
using ReviewAnythingAPI.HelperClasses;
using ReviewAnythingAPI.Models;
using ReviewAnythingAPI.Repositories;
using ReviewAnythingAPI.Repositories.Interfaces;
using ReviewAnythingAPI.Services;
using ReviewAnythingAPI.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

// Enable Sentry Services
builder.WebHost.UseSentry();

// CORS Services
var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("GenericPolicy", policy =>
    {
        policy.WithOrigins(origins ?? ["https://reviewanything.site"])
        .WithMethods("GET", "POST", "PUT", "DELETE", "HEAD", "PATCH")
        .WithHeaders("Content-Type", "Authorization", "X-Client-Type")
        .AllowCredentials();
    });
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new HeaderApiVersionReader("X-Api-Version"),
        new QueryStringApiVersionReader("api-version")
    );
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
});

// Adding swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ReviewAnythin API",
        Version = "v1",
        Description = """
        A RESTful API for the ReviewAnything platform, enabling full CRUD operations 
        for reviews, comments, votes, and user management.
        
        ## Features
        - **Reviews** – Create, read, update, and delete reviews for any subject
        - **Comments** – Threaded comments on reviews with full CRUD support
        - **Votes** – Upvote/downvote system for both reviews and comments
        - **Users** – User profile management and account operations
        
        ## Authentication
        This API uses **JWT Bearer authentication**. To access protected endpoints:
        1. Obtain a token via the `/auth/login` endpoint
        2. Click the **Authorize** button above and enter: `Bearer <your_token>`
        3. All subsequent requests will include your credentials automatically
        """,
        Contact = new OpenApiContact
        {
            Name = "Oscar Castro",
            Email = "oacastro999@gmail.com",
            Url = new Uri("https://oscarcastro.site")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        },
        TermsOfService = new Uri("https://reviewanything.site/terms-of-service"),
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    options.AddServer(new OpenApiServer
    {
        Url = "https://api.reviewanything.site",
        Description = "Production"
    });

    options.AddServer(new OpenApiServer
    {
        Url = "http://localhost:5026",
        Description = "Local Development"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

// Configure Cloudinary settings
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
// Register Cloudinary as a singleton
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
    options.ApiToken = builder.Configuration["Resend:ApiKey"] ?? "";
});
builder.Services.AddTransient<IResend, ResendClient>();

// Add services to the container.
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<ILogService, LogService>();
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

builder.Services.AddControllers();

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
    options.RequireHttpsMetadata = true; // In production set to true
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

    // Check if react header is present if so set auth to cookie else use bearer
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var cookieToken = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrWhiteSpace(cookieToken))
            {
                context.Token = cookieToken;
            }

            return Task.CompletedTask;
        }
    };
});

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://*:80");
}

// builder.WebHost.ConfigureKestrel(options =>
// {
//     options.ListenAnyIP(5000); // Listen on port 5000
// });
var app = builder.Build();

// Set ip from X-Forwared-For header
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
});

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
    // await seeder.SeedReviewsAsync(reviewJson, userIds, categoryIds, 1000);
}
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "ReviewAnything API v1");
    options.RoutePrefix = string.Empty;
});


// app.UseHttpsRedirection();
app.UseCors("GenericPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();
