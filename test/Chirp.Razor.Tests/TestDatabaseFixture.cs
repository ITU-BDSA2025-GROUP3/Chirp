using System.Data.Common;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor.Tests;


//Code from: https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database
public class TestDatabaseFixture : IAsyncLifetime
{
    private static DbConnection? Connection;
    private static DbContextOptions<ChirpDbContext>? Options;

    public TestDatabaseFixture()
    {
    }
    
    public async Task InitializeAsync()
    {
        //Open up the connection to the database
        Connection = new SqliteConnection("DataSource=:memory:");
        await Connection.OpenAsync();
        //Set up the options to the database
        Options = new DbContextOptionsBuilder<ChirpDbContext>()
            .UseSqlite(Connection)
            .Options;
    }

    public async Task DisposeAsync()
    {
        if (Connection != null)
        {
            await Connection.DisposeAsync();
        }
    }
    
    public ChirpDbContext CreateContext()
    {
        if (Options == null)
        {
            //Options should NEVER be null
            throw new NullReferenceException($"{nameof(Options)} is null.");
        } 
        
        return new ChirpDbContext(Options);
    }
}