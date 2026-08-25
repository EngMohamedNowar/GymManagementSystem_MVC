using AutoMapper;
using GymManagement.DbContexts;
using GymManagementSystem.BLL;
using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.DataSeeding;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
})
.AddViewLocalization()
.AddDataAnnotationsLocalization();

builder.Services.AddLocalization();

builder.Services.AddScoped(typeof(IGenericRepositories<>), typeof(GenericRepositories<>));

builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<IBodyMeasurementService, BodyMeasurementService>();
builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfile()));
builder.Services.AddScoped<IHomeService, HomeServices>();

builder.Services.AddScoped<IAttachmentService, AttachmentService>();

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailSmsService, EmailSmsService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
builder.Services.AddHostedService<MembershipExpiryService>();

var supportedCultures = new[] { "en-US", "ar-EG" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("en-US")
           .AddSupportedCultures(supportedCultures)
           .AddSupportedUICultures(supportedCultures);
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\":\"Unauthorized\"}");
            }
        };
    });

builder.Services.AddDbContext<GymDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<GymDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();

try
{
    var context = services.GetRequiredService<GymDbContext>();

    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

    if (pendingMigrations.Any())
    {
        try
        {
            await context.Database.MigrateAsync(); // Update-database
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (
            ex.Number == 2714 ||
            ex.Message.Contains("already exists") ||
            ex.Message.Contains("There is already an object"))
        {
            // The database was created outside of EF migrations (unmanaged). Adopt the
            // initial schema as the baseline without executing its SQL, then apply only
            // the new feature migrations (Batch1Features, Batch2Features, ...).
            logger.LogWarning(ex, "Existing unmanaged database detected. Adopting initial migration as baseline.");

            var initialId = pendingMigrations.First();
            await context.Database.ExecuteSqlRawAsync(
                "IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory') " +
                "CREATE TABLE [__EFMigrationsHistory] ([MigrationId] nvarchar(150) NOT NULL, [ProductVersion] nvarchar(32) NOT NULL, CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])); " +
                "IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = {0}) " +
                "INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ({0}, '10.0.10');",
                initialId);

            await context.Database.MigrateAsync();
        }
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Database migration failed.");
    throw;
}

try
{
    var context = services.GetRequiredService<GymDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    var folderPath = Path.Combine(
        app.Environment.ContentRootPath,
        "wwwroot",
        "files"
    );

    await GymDataSeeding.SeedAsync(context, logger, folderPath);

await IdentityDataSeeding.SeedIdentityDataAsync(
    userManager,
    roleManager,
    logger,
    app.Configuration
);
}
catch (Exception ex)
{
    logger.LogError(ex, "Data seeding failed. Continuing application start.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseRequestLocalization();

app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<LoginRateLimiter>();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public sealed class LoginRateLimiter
{
    private readonly RequestDelegate _next;
    private readonly int _permitLimit = 5;
    private readonly TimeSpan _window = TimeSpan.FromMinutes(1);
    private readonly ConcurrentDictionary<string, FixedWindowRateLimiter> _limiters = new();

    public LoginRateLimiter(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Equals("/Account/Login", StringComparison.OrdinalIgnoreCase)
            && string.Equals(context.Request.Method, "POST", StringComparison.OrdinalIgnoreCase))
        {
            var key = context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";
            var limiter = _limiters.GetOrAdd(key, _ =>
                new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
                {
                    PermitLimit = _permitLimit,
                    Window = _window
                }));

            using var lease = await limiter.AcquireAsync(1, context.RequestAborted);
            if (!lease.IsAcquired)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = ((int)_window.TotalSeconds).ToString();
                await context.Response.WriteAsync("Too many login attempts. Please try again later.");
                return;
            }
        }

        await _next(context);
    }
}