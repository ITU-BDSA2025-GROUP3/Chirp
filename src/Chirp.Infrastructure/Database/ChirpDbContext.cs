using Chirp.Core.DomainModel;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Database;

//Responsible for all database access
public class ChirpDbContext(DbContextOptions<ChirpDbContext> options) : IdentityDbContext<Author, IdentityRole<int>, int>(options)
{
    //private readonly string _connectionString; not in use?
    //private const int PAGE_SIZE = 32; // Fixed page size not in use?
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Core.DomainModel.Author> Authors { get; set; }
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
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Comment>()
            .HasIndex(c => c.CommentId)
            .IsUnique();
    }

    // method not in use??
    /*private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
    {
        // convert unix timestamp to human-readable date
        return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp)
            .ToLocalTime()
            .ToString("MM/dd/yy H:mm:ss");
    }*/

}