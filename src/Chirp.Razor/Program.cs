using Chirp.Razor;
using Chirp.Razor.DomainModel;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDbContext>(options => options.UseSqlite(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();


var app = builder.Build();

// TEMPORARY: sanity check to see if there's any cheeps in DB, and seed some cheep msgs
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ChirpDbContext>();
    db.Database.EnsureCreated();

    if (!db.Authors.Any())
    {
        var alice = new Author { Name = "alice", Email = "alice@example.com" };
        var bob   = new Author { Name = "bob",   Email = "bob@example.com" };
        db.AddRange(alice, bob);
        db.SaveChanges();

        db.Cheeps.AddRange(
            new Cheep { AuthorId = alice.AuthorId, Text = "hello chirp!",     TimeStamp = DateTime.UtcNow },
            new Cheep { AuthorId = bob.AuthorId,   Text = "first cheep",   TimeStamp = DateTime.UtcNow.AddMinutes(-1) },
            new Cheep { AuthorId = alice.AuthorId, Text = "nice day today!",  TimeStamp = DateTime.UtcNow.AddMinutes(-2) }
        );
        db.SaveChanges();

        Console.WriteLine("[DEBUG] Seeded sample data.");
    }

    Console.WriteLine($"[DEBUG] Cheeps in DB after seed: {db.Cheeps.Count()}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();

public partial class Program { } //public partial allows for testing according to https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0&pivots=xunit
