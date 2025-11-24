using Chirp.Core;
using Chirp.Core.DomainModel;
using Chirp.Core.RepositoryInterfaces;
using Chirp.Infrastructure.Services;
using System.ComponentModel.DataAnnotations;

using Chirp.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Tests;

public class CheepServiceTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(32, 1)]
    [InlineData(33, 2)]
    [InlineData(64, 2)]
    [InlineData(65, 3)]
    [InlineData(1024, 32)]
    [InlineData(1025, 33)]
    public async Task GetTotalCheeps_ReturnsExpectedPageCount(int totalCheeps, int expectedPages)
    {
        var repository = new FakeCheepRepository(totalCheeps, new Dictionary<int, int>());
        var service = new CheepService(repository);

        var pages = await service.GetTotalCheeps();

        Assert.Equal(expectedPages, pages);
    }

    [Theory]
    [InlineData("alice", 0, 1)]
    [InlineData("calice", 32, 1)]
    [InlineData("palice", 33, 2)]
    [InlineData("bob", 64, 2)]
    [InlineData("bob", 65, 3)]
    [InlineData("charlie", -5, 1)]
    public async Task GetTotalAuthorCheeps_ReturnsExpectedPageCount(string author, int totalCheeps, int expectedPages)
    {
        var authorIds = new Dictionary<string, int> { { author, 42 } };
        var authorCheepTotals = new Dictionary<int, int> { { 42, totalCheeps } };

        var authorRepository = new FakeAuthorRepository(authorIds);
        var cheepRepository = new FakeCheepRepository(totalCheeps, authorCheepTotals);
        var service = new AuthorService(authorRepository, cheepRepository);

        var pages = await service.GetTotalAuthorCheeps(author);

        Assert.Equal(expectedPages, pages);
    }
    
    [Fact]
    public async Task GetTotalAuthorCheeps_ReturnsOneWhenAuthorMissing()
    {
        var service = new AuthorService(new FakeAuthorRepository(), new FakeCheepRepository(0,new Dictionary<int, int>()));
        var pages = await service.GetTotalAuthorCheeps(" ");
        Assert.Equal(1, pages);
    }

    
    private sealed class FakeAuthorRepository : IAuthorRepository
    {
        private readonly Dictionary<string, int> _authorIds;
    
        public FakeAuthorRepository(Dictionary<string, int>? authorIds = null)
        {
            _authorIds = authorIds ?? new Dictionary<string, int>();
        }

        public Task<int> GetAuthorId(string authorNameOrEmail)
        {
            if (string.IsNullOrWhiteSpace(authorNameOrEmail))
            {
                return Task.FromResult<int>(0);
            }
            var key = authorNameOrEmail.Trim();
            return Task.FromResult(_authorIds.TryGetValue(key, out var id) ? id : 0);
        }

        public Task<List<int>> GetAuthorIDs(int authorId)
        {
            if (authorId == 0) return Task.FromResult(new List<int>());
            return Task.FromResult(new List<int> { authorId });
        }

        public Task<List<Author>> GetFollowedList(string authorName)
        {
            throw new NotImplementedException();
        }

        public Task AddAuthorToFollows(string nameOfAuthorToAdd, string nameOfAuthorFollowing)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAuthorFromFollows(string nameOfAuthorToRemove, string nameOfAuthorFollowing)
        {
            throw new NotImplementedException();
        }

        public Task<Author> DeleteAuthor(string authorNameOrEmail)
        {
            throw new NotImplementedException();
        }
    }
    
    private sealed class FakeCheepRepository : ICheepRepository
    {
        private readonly int _totalCheeps;
        private readonly Dictionary<int, int> _authorTotals;
        private readonly List<CheepDTO> _createdCheeps = new();

        public FakeCheepRepository(int totalCheeps, Dictionary<int, int>? authorTotals)
        {
            _totalCheeps = totalCheeps;
            _authorTotals = authorTotals ?? new Dictionary<int, int>();
        }

        public Task<List<Cheep>> ReadPublicCheeps(int page, int pageSize) => Task.FromResult(new List<Cheep>());
        public Task<List<Cheep>> ReadTimelineCheeps(int page, int pageSize, List<int> authorIds) => Task.FromResult(new List<Cheep>());
        public Task<int> GetTotalPublicCheeps() => Task.FromResult(_totalCheeps);
        public Task<int> GetTotalTimelineCheeps(List<int> authorIds) => Task.FromResult(_authorTotals.TryGetValue(authorIds.FirstOrDefault(), out var total) ? total : 0);

        public Task CreateCheep(CheepDTO newCheep)
        {
            _createdCheeps.Add(newCheep);
            return Task.CompletedTask;
        }
        public List<CheepDTO> GetCreatedCheeps() => _createdCheeps;
        public Task AddCheep(Cheep cheep)
        {
            throw new NotImplementedException();
        }
    }
    
    [Fact]
    public async Task CreateCheep_WithValidAuthor()
    {
        // Arrange
        var fakeCheepRepository = new FakeCheepRepository(1, new Dictionary<int, int>());
        var service = new CheepService(fakeCheepRepository);
        
        // Act
        await service.AddNewCheep("alice@alice.com", "Hey this is a test");
        
        // Assert
        var createdCheeps = fakeCheepRepository.GetCreatedCheeps();
        Assert.Single(createdCheeps);
        Assert.Equal("alice@alice.com", createdCheeps[0].UserName);
        Assert.Equal("Hey this is a test", createdCheeps[0].Message);
    }

    [Fact]
    public async Task AddNewCheep_WithValidLength()
    {
        //Arrange
        var repository = new FakeCheepRepository(1, new Dictionary<int, int>());
        var service = new CheepService(repository);

        var author = "alice";
        var message = new string('a', 160);
        
        //Act
        await service.AddNewCheep(author, message);
        
        //Assert
        var createdCheeps = repository.GetCreatedCheeps();
        Assert.Single(createdCheeps);

        var storedCheeps = createdCheeps[0];
        Assert.Equal(author, storedCheeps.UserName);
        Assert.Equal(message, storedCheeps.Message);
    }

    [Fact]
    public async Task AddNewCheep_WithTooLongMessage()
    {
        //Arrange
        var repository = new FakeCheepRepository(1, new Dictionary<int, int>());
        var service = new CheepService(repository);
        
        var author = "alice";
        var message = new string('a', 161);
        
        //Act and Assert
        await Assert.ThrowsAsync<ValidationException>(() => service.AddNewCheep(author, message));
        
        var createdCheeps = repository.GetCreatedCheeps();
        Assert.Empty(createdCheeps);
    }

    [Theory]
    [InlineData("Alice", "Alice@Alice.com", 3, 7)]
    [InlineData("Bob", "Bob@Bob.com", 3, 12)]
    [InlineData("Charlie", "Charlie@Charlie.com", 3, 15)]
    [InlineData("David", "David@David.com", 3, 17)]
    public async Task DeleteAuthor(string username, string email, int expectedAuthors, int expectedCheeps)
    {
        //arrange
        await using var context = Utility.CreateFakeChirpDbContext();
        Utility.SeedDatabase(context);
        var cheepRepo = new CheepRepository(context);
        var authorRepo = new AuthorRepository(context);
        var authorService = new AuthorService(authorRepo, cheepRepo);
        
        await authorService.AddAuthorToFollowsList("Bob", "Alice");
        await authorService.AddAuthorToFollowsList("Charlie", "Alice");
        await authorService.AddAuthorToFollowsList("Alice", "Bob");
        await authorService.AddAuthorToFollowsList("Alice", "Charlie");
        await authorService.AddAuthorToFollowsList("Bob", "Charlie");
        await authorService.AddAuthorToFollowsList("David", "Charlie");
        
        //act
        Task<AuthorDTO> removedAuthor = null!;
        var exception = await Record.ExceptionAsync(() => removedAuthor = authorService.DeleteAuthor(username));
        var amountOfAuthors = context.Authors.Count();
        var amountOfCheeps = context.Cheeps.Count();
        
        //assert
        Assert.Null(exception);
        Assert.Equal(expectedAuthors, amountOfAuthors);
        Assert.Equal(expectedCheeps, amountOfCheeps);
        var dto = await removedAuthor;
        Assert.Equal(username, dto.Name);
        Assert.Equal(email, dto.Email);
        
        foreach (var author1 in context.Authors)
        {
            var follows = await authorService.GetFollowsList(author1.UserName);
            Assert.DoesNotContain(follows, a => a.Name == username);
        }
    }
}
