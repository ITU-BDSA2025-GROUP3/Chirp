using Chirp.Core.DomainModel;
using Chirp.Infrastructure.Database;
using Chirp.Infrastructure.Repositories;
using Chirp.Core;

using Microsoft.Data.Sqlite;

using Moq;
using Microsoft.EntityFrameworkCore;

using Xunit.Abstractions;

using System.Globalization;
using System.Runtime.InteropServices;

using NuGet.Packaging;

namespace Chirp.Tests;

public class RepositoryTests /*(ITestOutputHelper testOutputHelper)*/ /*unused*/
{
    
    [Theory]
    [InlineData(1, 32, 32)]
    [InlineData(2, 32, 32)]
    [InlineData(3, 32, 32)]
    [InlineData(100, 32, 0)]
    public async Task ReadCheepsFromPageTestNumberOfCheeps(int page, int pageSize, int expected)
    {
        //arrange
        await using var context = Chirp.Tests.Utility.CreateFakeChirpDbContext();
        DbInitializer.SeedDatabase(context);
        var repo = new CheepRepository(context);
        //act
        var cheeps = await repo.ReadPublicCheeps(page, pageSize);
        //assert
        Assert.Equal(cheeps.Count, expected);
    }

    [Fact]
    public async Task ReadCheepsFromPageTest()
    {
        //arrange
        await using var context = Utility.CreateFakeChirpDbContext();
        var author1 = new Author { Id = 1, UserName = "Alice", Email = "Alice@Alice.com", Cheeps = new List<Cheep>(), Follows = new  List<Author>() };
        var author2 = new Author { Id = 2, UserName = "Bob", Email = "Bob@Bob.com", Cheeps = new List<Cheep>(), Follows = new  List<Author>() };
        var cheep1 = new Cheep { Text = "Hello", TimeStamp = new DateTime(0), IdOfAuthor = 1, Author = author1 };
        var cheep2 = new Cheep { Text = "Hello", TimeStamp = new DateTime(1), IdOfAuthor = 2, Author = author2 };
        author1.Cheeps.Add(cheep1);
        author2.Cheeps.Add(cheep2);
        context.Authors.Add(author1);
        context.Authors.Add(author2);
        context.Cheeps.Add(cheep1);
        context.Cheeps.Add(cheep2);
        context.SaveChanges();
        var repo = new CheepRepository(context);
        
        //act
        var cheeps = await repo.ReadPublicCheeps(1, 1);
        
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
    public async Task ReadCheepsFromPageTest2(int page, int pageSize, int expected)
    {
        //arrange
        await using var context = Utility.CreateFakeChirpDbContext();
        Utility.SeedDatabase(context); //using the mocked data of 17 cheeps and 4 authors
        var repo = new CheepRepository(context);
        
        //act
        var cheeps = await repo.ReadPublicCheeps(page, pageSize);
        
        //assert
        Assert.Equal(expected, cheeps.Count);
    }

    [Fact]
    public async Task CreateCheep_WithInvalidAuthor()
    {
        await using var context = Utility.CreateFakeChirpDbContext();
        // Arrange
        var repository = new CheepRepository(context);
        var dto = new CheepDTO{ UserName = "invalidUser@email.com", Message = "I'm not a test", };
        
        // Act & assert
        var exception = await Assert.ThrowsAsync<Exception>(() => repository.CreateCheep(dto));
        
        Assert.Contains("Author does not exist", exception.Message);
    
    }

    [Fact]
    public async Task CreateCheep_SavesCheepToDatabase()
    {
        //Arrange
        await using var context = Utility.CreateFakeChirpDbContext();
        Utility.SeedDatabase(context);
        
        var repo = new CheepRepository(context);

        var initialCount = context.Cheeps.Count();

        var message = new string('a', 160);
        var dto = new CheepDTO
        {
            UserName = "Alice",
            Message = message,
            TimeStamp = new DateTimeOffset(DateTime.UtcNow)
                .ToLocalTime()
                .ToString("MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
        };
        
        //Act
        await repo.CreateCheep(dto);
        
        //Assert
        var finalCount = context.Cheeps.Count();
        Assert.Equal(initialCount + 1, finalCount);

        var savedCheep = context.Cheeps.Include(cheep => cheep.Author)
            .Single(c => c.Author.UserName == "Alice" && c.Text == message);

        Assert.Equal("Alice", savedCheep.Author.UserName);
        Assert.Equal(message, savedCheep.Text);
    }

    /// <summary>
    /// Tests that an author's information is deleted from places that references it in the database.
    /// </summary>
    /// <param name="author"></param>
    /// <param name="expectedAuthors"></param>
    /// <param name="expectedCheeps"></param>
    [Theory]
    [InlineData("Alice", 3, 7)]
    [InlineData("Alice@Alice.com", 3, 7)]
    [InlineData("Bob", 3, 12)]
    [InlineData("Bob@Bob.com", 3, 12)]
    [InlineData("Charlie", 3, 15)]
    [InlineData("Charlie@Charlie.com", 3, 15)]
    [InlineData("David", 3, 17)]
    [InlineData("David@David.com", 3, 17)]
    public async Task DeleteAuthorTest_Succeeds(string author, int expectedAuthors, int expectedCheeps)
    {
        //Arrange
        await using var context = Utility.CreateFakeChirpDbContext();
        Utility.SeedDatabase(context);
        
        var authorRepo = new AuthorRepository(context);
        var cheepRepo = new CheepRepository(context);
            //add followings
        await authorRepo.AddAuthorToFollows("Bob", "Alice");
        await authorRepo.AddAuthorToFollows("Charlie", "Alice");
        await authorRepo.AddAuthorToFollows("Alice", "Bob");
        await authorRepo.AddAuthorToFollows("Alice", "Charlie");
        await authorRepo.AddAuthorToFollows("Bob", "Charlie");
        await authorRepo.AddAuthorToFollows("David", "Charlie");
        var authorToRemove = await context.Authors.Where(a => a.UserName == author || a.Email == author).Select(a => a).FirstAsync();
        
        //act
        Task<Author> removedAuthor = null!;
        var exception = await Record.ExceptionAsync(() => removedAuthor = authorRepo.DeleteAuthor(author));
        var amountOfAuthors = context.Authors.Count();
        var amountOfCheeps = context.Cheeps.Count();
        
        //assert
        Assert.Null(exception);
        Assert.Equal(expectedAuthors, amountOfAuthors);
        Assert.Equal(expectedCheeps, amountOfCheeps);
        Assert.Equal(authorToRemove, await removedAuthor);
        
        foreach (var author1 in context.Authors)
        {
            var follows = await authorRepo.GetFollowedList(author1.UserName!);
            Assert.DoesNotContain(follows, a => a.UserName == author);
        }
    }
    
    [Theory]
    [InlineData("Aalice")]
    [InlineData("alice")]
    [InlineData("alice@alice.com")]
    [InlineData("Alice@alice.com")]
    [InlineData("Bob@b0b.com")]
    [InlineData("B0b")]
    [InlineData("Alice&Alice.com")]
    [InlineData(".39m0f?3")]
    public async Task DeleteAuthorTest_Fails(string authorToDelete)
    {
        //Arrange
        await using var context = Utility.CreateFakeChirpDbContext();
        Utility.SeedDatabase(context);
        var authorRepo = new AuthorRepository(context);
        var cheepRepo = new CheepRepository(context);
        
        //act & assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => authorRepo.DeleteAuthor(authorToDelete));
        Assert.Equal(4, context.Authors.Count());
        Assert.Equal(17, await cheepRepo.GetTotalPublicCheeps());
    } 
}