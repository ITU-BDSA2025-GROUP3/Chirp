using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.Tests;


//Code from: https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database
public class TestDatabaseFixture : IClassFixture<TestDatabaseFixture>
{
    private const string ConnectionString = "Data Source=test.db";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

                _databaseInitialized = true;
            }
        }
    }
    
    public ChirpDbContext CreateContext()
        => new ChirpDbContext(
            new DbContextOptionsBuilder<ChirpDbContext>()
                .UseSqlite(ConnectionString)
                .Options);
}