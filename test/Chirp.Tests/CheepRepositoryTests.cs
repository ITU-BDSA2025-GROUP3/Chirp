using Chirp.Core.DomainModel;
using Chirp.Infrastructure.Database;
using Chirp.Infrastructure.Repositories;

using Microsoft.Data.Sqlite;

using Moq;
using Microsoft.EntityFrameworkCore;

using Xunit.Abstractions;

namespace Chirp.Tests;

public class CheepRepositoryTests(ITestOutputHelper testOutputHelper)
{
    private static ChirpDbContext CreateFakeChirpDbContext()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        var contextOptions = new DbContextOptionsBuilder<ChirpDbContext>().UseSqlite(connection).Options;
        
        var context = new ChirpDbContext(contextOptions);
        context.Database.EnsureDeletedAsync(); 
        context.Database.EnsureCreatedAsync(); 
        return context;
    }
    
    [Theory]
    [InlineData(1, 32, 32)]
    [InlineData(2, 32, 32)]
    [InlineData(3, 32, 32)]
    [InlineData(100, 32, 0)]
    public void ReadCheepsFromPageTestNumberOfCheeps(int page, int pageSize, int expected)
    {
        //arrange
        using var context = CreateFakeChirpDbContext();
        DbInitializer.SeedDatabase(context);
        var repo = new CheepRepository(context);
        //act
        var cheeps = repo.ReadCheeps(page, pageSize).Result.ToList();
        //assert
        Assert.Equal(cheeps.Count, expected);
    }

    [Fact]
    public void ReadCheepsFromPageTest()
    {
        //arrange
        using var context = CreateFakeChirpDbContext();
        var author1 = new Author { AuthorId = 1, Name = "Alice", Email = "Alice@Alice.com", Cheeps = new List<Cheep>() };
        var author2 = new Author { AuthorId = 2, Name = "Bob", Email = "Bob@Bob.com", Cheeps = new List<Cheep>() };
        var cheep1 = new Cheep { Text = "Hello", TimeStamp = new DateTime(0), AuthorId = 1, Author = author1 };
        var cheep2 = new Cheep { Text = "Hello", TimeStamp = new DateTime(1), AuthorId = 2, Author = author2 };
        author1.Cheeps.Add(cheep1);
        author2.Cheeps.Add(cheep2);
        context.Authors.Add(author1);
        context.Authors.Add(author2);
        context.Cheeps.Add(cheep1);
        context.Cheeps.Add(cheep2);
        context.SaveChanges();
        var repo = new CheepRepository(context);
        
        //act
        var cheeps = repo.ReadCheeps(1, 1).Result.ToList();
        
        //assert
        Assert.Equal(cheep2, cheeps[0]); //the newest cheep must be the first in the list
    }
}