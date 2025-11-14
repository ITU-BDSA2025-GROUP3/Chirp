using Chirp.Core.DomainModel;
using Chirp.Infrastructure.Database;
using Chirp.Infrastructure.Repositories;
using Chirp.Core;

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
    
    private static void SeedDatabase(ChirpDbContext context)
    {

        //create a dictionary of how many cheeps to create for each author - ease of access
        var cheepsPerAuthor = new Dictionary<String, int>
        {
            { "Alice", 10 }, { "Bob", 5 }, { "Charlie", 2 }, { "David", 0 }
        };

        List<Author> authorList = [];
        List<Cheep> cheepList = [];
        var authorIdCounter = 1; //must be at least "1" as EF-Core expects this, lest breaking the system
        var timestampCounter = 0;
        foreach (var name in cheepsPerAuthor)
        {
            var author = new Author { AuthorId = authorIdCounter++, Name = name.Key, Email = $"{name}@{name}.com", Cheeps = new  List<Cheep>() };
            authorList.Add(author);
            for (int i = 0; i < name.Value; i++)
            {
                var cheep = new Cheep
                {
                    Text = "test",
                    TimeStamp = new DateTime(timestampCounter++),
                    AuthorId = author.AuthorId,
                    Author = author
                };
                author.Cheeps.Add(cheep);
                cheepList.Add(cheep);
            }
        }
        context.Cheeps.AddRange(cheepList);
        context.Authors.AddRange(authorList);
        context.SaveChanges();
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

    [Theory]
    [InlineData(1, 32, 17)]
    [InlineData(1,15, 15 )]
    [InlineData(2, 32, 0)]
    [InlineData(1, 5, 5)]
    [InlineData(2, 5, 5)]
    [InlineData(3, 5, 5)]
    [InlineData(4, 5, 2)]
    [InlineData(5, 5, 0)]
    public void ReadCheepsFromPageTest2(int page, int pageSize, int expected)
    {
        //arrange
        using var context = CreateFakeChirpDbContext();
        SeedDatabase(context); //using the mocked data of 17 cheeps and 4 authors
        var repo = new CheepRepository(context);
        
        //act
        var cheeps = repo.ReadCheeps(page, pageSize).Result;
        
        //assert
        Assert.Equal(expected, cheeps.Count);
    }

    [Fact]
    public async Task CreateCheep_WithInvalidAuthor()
    {
        using var context = CreateFakeChirpDbContext();
        // Arrange
        var repository = new CheepRepository(context);
        var dto = new CheepDTO{ Author = "invalidUser@email.com", Message = "I'm not a test", };
        
        // Act & assert
        var exception = await Assert.ThrowsAsync<Exception>(() => repository.CreateCheep(dto));
        
        Assert.Contains("Author does not exist", exception.Message);
    
    }
}