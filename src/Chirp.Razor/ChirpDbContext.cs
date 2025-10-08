using System.Reflection;

using Chirp.Razor.DomainModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace Chirp.Razor;
using Microsoft.Data.Sqlite;

//Responsible for all database access
public class ChirpDbContext : DbContext
{
    private readonly string _connectionString;
    private const int PAGE_SIZE = 32; // Fixed page size
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    public ChirpDbContext(DbContextOptions<ChirpDbContext> options) : base(options)
    {
    }
   
    
   
    private static string UnixTimeStampToDateTimeString(long unixTimeStamp)
    {
        // convert unix timestamp to human readable date
        return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp)
            .ToLocalTime()
            .ToString("MM/dd/yy H:mm:ss");
    }

}