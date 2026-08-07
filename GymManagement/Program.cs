using AutoMapper;
using GymManagement.DbContexts;
using GymManagementSystem.BLL;
using GymManagementSystem.BLL.Services.Classes;
using GymManagementSystem.BLL.Services.Interfaces;
using GymManagementSystem.DAL;
using GymManagementSystem.DAL.DataSeeding;
using GymManagementSystem.DAL.Repositories.Classes;
using GymManagementSystem.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddScoped<IPlanRepositories,PlanRepositories>(); //Dependency Injection tell CLR to create object of thease
//builder.Services.AddScoped<GymDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped(typeof(IGenericRepositories<>), typeof(GenericRepositories<>));

builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfile()));
builder.Services.AddScoped<IHomeService, HomeServices>();

builder.Services.AddScoped<IAttachmentService, AttachmentService>();




builder.Services.AddDbContext<GymDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});


var app = builder.Build();
var scope = app.Services.CreateScope();
var _context = scope.ServiceProvider.GetRequiredService<GymDbContext>();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var folderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "files");
var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
if (pendingMigrations.Any())
{
    await _context.Database.MigrateAsync();
}
await GymDataSeeding.SeedAsync(_context, logger, folderPath);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
