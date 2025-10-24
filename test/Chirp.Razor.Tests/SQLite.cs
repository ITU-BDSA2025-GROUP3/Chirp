namespace Chirp.Razor.Tests;

public class SQLite : IClassFixture<TestDatabaseFixture>
{
    
    TestDatabaseFixture _databaseFixture;

    public SQLite(TestDatabaseFixture testDatabaseFixture)
    {
        _databaseFixture = testDatabaseFixture;
    }

    //Tests seeding of the database using IClassFixtures from xunit to ensure temporary database which uses the same
    //db context as before
    [Fact]
    public async Task seedDatabase()
    {
        await using var context = _databaseFixture.CreateContext();
        
        Assert.Equal(0, context.Cheeps.Count());
        DbInitializer.SeedDatabase(context);
        Assert.Equal(657, context.Cheeps.Count());
    }
    
}