using Chirp.Core.DomainModel;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;

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

        public Task<int> GetAuthorIdFrom(string authorNameOrEmail)
        {
            if (string.IsNullOrWhiteSpace(authorNameOrEmail))
            {
                return Task.FromResult<int>(0);
            }
            var key = authorNameOrEmail.Trim();
            return Task.FromResult(_authorIds.TryGetValue(key, out var id) ? id : 0);
        }

        public Task CreateAuthor(string authorName, string authorEmail) => throw new NotSupportedException();
    }
    
    private sealed class FakeCheepRepository : ICheepRepository
    {
        private readonly int _totalCheeps;
        private readonly Dictionary<int, int> _authorTotals;

        public FakeCheepRepository(int totalCheeps, Dictionary<int, int>? authorTotals)
        {
            _totalCheeps = totalCheeps;
            _authorTotals = authorTotals ?? new Dictionary<int, int>();
        }

        public Task<List<Cheep>> ReadCheeps(int page) => Task.FromResult(new List<Cheep>());
        public Task<List<Cheep>> ReadCheepsFrom(int page, int authorId) => Task.FromResult(new List<Cheep>());
        public Task<int> GetTotalCheeps() => Task.FromResult(_totalCheeps);
        public Task<int> GetTotalCheepsFor(int authorId) => Task.FromResult(_authorTotals.TryGetValue(authorId, out var total) ? total : 0);
        public Task CreateCheep(CheepDTO newCheep) => throw new NotSupportedException();
        public Task AddCheep(Cheep cheep)
        {
            throw new NotImplementedException();
        }
    }
    
    [InlineData(65, 3)]
    public async Task GetTotalCheeps_ReturnsNumberOfPages(int totalCheeps, int expectedPages)
    {
        
        var fakeRepository = new FakeCheepRepository(totalCheeps, new Dictionary<int, int>());
        var service = new CheepService(fakeRepository);

        
        
    }
}
