using Chirp.Core.DomainModel;
using Chirp.Infrastructure.Database;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Chirp.Infrastructure;

using Microsoft.AspNetCore.Authentication.Cookies;


var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddDefaultIdentity<Author>(options =>
        options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ChirpDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddAuthentication(options =>
    { })
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["authentication:github:clientId"] ?? string.Empty;
        options.ClientSecret = builder.Configuration["authentication:github:clientSecret"] ?? string.Empty;
        options.CallbackPath = "/signin-github";
    });

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

var app = builder.Build();

// sanity check to see if there's any cheeps in DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();
    db.Database.EnsureCreated();
    DbInitializer.SeedDatabase(db);
    Console.WriteLine($"[DEBUG] Cheeps in DB: {db.Cheeps.Count()}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

namespace Chirp.Web
{
    public partial class Program { }
} //public partial allows for testing according to https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0&pivots=xunit
