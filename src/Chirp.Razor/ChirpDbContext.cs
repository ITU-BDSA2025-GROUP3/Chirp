using Chirp.Razor.DomainModel;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

//Responsible for all database access
public class ChirpDbContext : DbContext
{
    //private readonly string _connectionString; not in use?
    //private const int PAGE_SIZE = 32; // Fixed page size not in use?
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Author>()
            .HasIndex(c => c.Name)
            .IsUnique();
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Author>()
            .HasIndex(c => c.Email)
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