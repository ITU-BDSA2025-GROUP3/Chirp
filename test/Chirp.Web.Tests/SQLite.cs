using Chirp.Web.DomainModel;

namespace Chirp.Web.Tests;

public class SQLite : IClassFixture<TestDatabaseFixture>, IAsyncLifetime
{
    
    private readonly TestDatabaseFixture _databaseFixture;
    private ChirpDbContext? Context;

    public SQLite(TestDatabaseFixture testDatabaseFixture)
    {
        _databaseFixture = testDatabaseFixture;
    }
    
    //Before any test starts running get a new context, ensure the context is deleted and ensure the context is created again.
    public async Task InitializeAsync()
    {
        Context = _databaseFixture.CreateContext();
        
        await Context.Database.EnsureDeletedAsync(); 
        await Context.Database.EnsureCreatedAsync(); 
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    //Tests seeding of the database using IClassFixtures from xunit to ensure temporary database which uses the same
    //db context as before
    [Fact]
    public void seedDatabase()
    {
        //Context should NEVER be null in our test cases
        if (Context == null)
        {
            throw new NullReferenceException($"{nameof(Context)} is null.");
        }
        
        Assert.Equal(0, Context.Cheeps.Count());
        DbInitializer.SeedDatabase(Context);
        Assert.Equal(657, Context.Cheeps.Count());
    }
    
    //This test is specifically made to ensure our testing is using a new in-memory database upon each test.
    [Fact] 
    public async Task saveCheepsInMemoryDatabase()
    {

        //Context should NEVER be null in our test cases
        if (Context == null)
        {
            throw new NullReferenceException($"{nameof(Context)} is null.");
        }
        
        Assert.Equal(0, Context.Cheeps.Count());
        
        var a1 = new Author() { AuthorId = 1, Name = "Roger Histand", Email = "Roger+Histand@hotmail.com", Cheeps = new List<Cheep>() };
        var a2 = new Author() { AuthorId = 2, Name = "Luanna Muro", Email = "Luanna-Muro@ku.dk", Cheeps = new List<Cheep>() };
        
        Context.Authors.Add(a1);
        Context.Authors.Add(a2);
        await Context.SaveChangesAsync();
        
        //Ensure authors can be saved to the memory database
        Assert.Equal(2, Context.Authors.Count());
        
        var c14 = new Cheep() { CheepId = 14, AuthorId = a1.AuthorId, Author = a1, Text = "You are here for at all?", TimeStamp = DateTime.Parse("2023-08-01 13:13:18") };
        var c57 = new Cheep() { CheepId = 57, AuthorId = a2.AuthorId, Author = a2, Text = "See how that murderer could be from any trivial business not connected with her.", TimeStamp = DateTime.Parse("2023-08-01 13:13:21") };
        
        //Ensure that cheeps can be saved to the memory database
        Context.Cheeps.Add(c14);
        Context.Cheeps.Add(c57);
        await Context.SaveChangesAsync();
        
        Assert.Equal(2, Context.Cheeps.Count());
        
    }
    
}