using Chirp.Core.DomainModel;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Database;

/// <summary>
/// ChirpDbContext is the Chirp Applications main database context, containing the Cheeps, Authors and Comments that makes up the data model of the application.
/// ChirpDbContext uses the Author object for the ef core identity user.
/// </summary>
/// <param name="options"></param>
public class ChirpDbContext(DbContextOptions<ChirpDbContext> options) : IdentityDbContext<Author, IdentityRole<int>, int>(options)
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Author>()
            .HasIndex(c => c.UserName)
            .IsUnique();
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Author>()
            .HasIndex(c => c.Email)
            .IsUnique();
    }
}