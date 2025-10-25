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
        var repository = new FakeCheepRepository(totalCheeps);
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
        var repository = new FakeAuthorRepository(new Dictionary<string, int> { { author, totalCheeps } });
        var service = new AuthorService(repository);

        var pages = await service.GetTotalAuthorCheeps(author);

        Assert.Equal(expectedPages, pages);
    }
    
    [Fact]
    public async Task GetTotalAuthorCheeps_ReturnsOneWhenAuthorMissing()
    {
        var service = new AuthorService(new FakeAuthorRepository());
        var pages = await service.GetTotalAuthorCheeps(" ");
        Assert.Equal(1, pages);
    }

    
    private sealed class FakeAuthorRepository : IAuthorRepository
    {
        private readonly Dictionary<string, int> _authorTotals;

        public FakeAuthorRepository(Dictionary<string, int>? authorTotals = null)
        {
            _authorTotals = authorTotals ?? new Dictionary<string, int>();
        }

        public Task<List<CheepDTO>> ReadAuthorCheeps(string authorName, int page) => throw new NotSupportedException();

        public Task CreateAuthor(string authorName, string authorEmail) => throw new NotSupportedException();

        public Task<int> GetTotalAuthorCheeps(string authorName) =>
            Task.FromResult(_authorTotals.TryGetValue(authorName, out var total) ? total : 0);
    }
    
    private sealed class FakeCheepRepository : ICheepRepository
    {
        private readonly int _totalCheeps;

        public FakeCheepRepository(int totalCheeps)
        {
            _totalCheeps = totalCheeps;
        }

        public Task<List<CheepDTO>> ReadCheeps(int page) => throw new NotSupportedException();

        public Task CreateCheep(CheepDTO newCheep) => throw new NotSupportedException();

        public Task<int> GetTotalCheeps() => Task.FromResult(_totalCheeps);

        public Task AddCheep(Cheep cheep)
        {
            throw new NotImplementedException();
        }
    }
}
