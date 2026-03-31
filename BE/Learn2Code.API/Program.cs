using System.Text;
using Learn2Code.Application.Interfaces;
using Learn2Code.Infrastructure.Data.Context;
using Learn2Code.Infrastructure.Persistence.UnitOfWork;
using Learn2Code.Application.Services;
using Learn2Code.Infrastructure.Options;
using Learn2Code.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Suppress EF Core SQL/migration noise in logs (keep app-level info logs)
builder.Logging
    .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning)
    .AddFilter("Microsoft.EntityFrameworkCore.Database.Transaction", LogLevel.Warning)
    .AddFilter("Microsoft.EntityFrameworkCore.Migrations", LogLevel.Warning)
    .AddFilter("Microsoft.EntityFrameworkCore.Infrastructure", LogLevel.Warning);

// Database
builder.Services.AddDbContext<Learn2CodeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMemoryCache();

// Piston Configuration
builder.Services.Configure<PistonOptions>(builder.Configuration.GetSection("Piston"));
builder.Services.AddHttpClient<IPistonService, PistonService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// PayOS Configuration (IOptions pattern)
builder.Services.Configure<PayOsOptions>(options =>
{
    options.ClientId = builder.Configuration["PAYOS_CLIENT_ID"]
        ?? builder.Configuration["PayOS:ClientId"]
        ?? throw new InvalidOperationException("PayOS ClientId not configured");

    options.ApiKey = builder.Configuration["PAYOS_API_KEY"]
        ?? builder.Configuration["PayOS:ApiKey"]
        ?? throw new InvalidOperationException("PayOS ApiKey not configured");

    options.ChecksumKey = builder.Configuration["PAYOS_CHECKSUM_KEY"]
        ?? builder.Configuration["PayOS:ChecksumKey"]
        ?? throw new InvalidOperationException("PayOS ChecksumKey not configured");
});

// PayOS HttpClient (timeout 30s)
builder.Services.AddHttpClient<IPayOsService, PayOsService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();
builder.Services.AddScoped<ITestCaseService, TestCaseService>();
builder.Services.AddScoped<IExerciseMediaService, ExerciseMediaService>();
builder.Services.AddScoped<IProgressService, ProgressService>();
builder.Services.AddScoped<ISubscriptionPackageService, SubscriptionPackageService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ILeaderboardService, LeaderboardService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IGamificationService, GamificationService>();
builder.Services.AddScoped<IUserXPService, UserXPService>();
builder.Services.AddScoped<IAchievementService, AchievementService>();
builder.Services.AddScoped<IDailyStreakService, DailyStreakService>();
builder.Services.AddScoped<IDiscussionService, DiscussionService>();


// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000") // Add FE URLs
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Learn2Code API",
        Version = "v1",
        Description = "API for Coding Learning Platform"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Chỉ cần paste token vào đây, Swagger sẽ tự thêm 'Bearer ' phía trước."
    });

    // Filter để chỉ show lock icon cho [Authorize] endpoints
    c.OperationFilter<AuthorizeOperationFilter>();
});

var app = builder.Build();

// Migrate + seed (ResetSchemaIfNeededAsync inside SeedAsync handles existing tables)
await Learn2CodeDbContextSeeder.SeedAsync(app.Services);

// Enable request body buffering for webhook signature verification
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    await next();
});

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.Run();

public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo.GetCustomAttributes(typeof(AuthorizeAttribute), false).Any()
            || context.MethodInfo.DeclaringType?.GetCustomAttributes(typeof(AuthorizeAttribute), false).Any() == true;

        if (hasAuthorize)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                        new List<string>()
                    }
                }
            };
        }
    }
}
