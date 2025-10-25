using Chirp.Infrastructure.Repositories;

namespace Chirp.Infrastructure.Services;

public interface IAuthorService
{
    int CurrentPage { get; set; }
    public Task<List<CheepDTO>> GetAuthorCheeps(string author);
    public Task<int> GetTotalAuthorCheeps(string author);
}

public class AuthorService : IAuthorService
{
    private const int PAGE_SIZE = 32;
    //Sets confirguable databse path
    // private readonly ChirpDbContext _chirpDbContext;
    private readonly IAuthorRepository _cheepRepository;
    //Set or get the currentPage to be viewed
    public int CurrentPage { get; set; } = 1;
    public AuthorService(IAuthorRepository authorRepository)
    {
        _cheepRepository = authorRepository;
    }

    public async Task<List<CheepDTO>> GetAuthorCheeps(string author)
    {
        return await _cheepRepository.ReadAuthorCheeps(author, CurrentPage);
    }
    
    public async Task<int> GetTotalAuthorCheeps(string author)
    {
        var total = await _cheepRepository.GetTotalAuthorCheeps(author);
        return Math.Max(1, (total + PAGE_SIZE - 1) / PAGE_SIZE);
    }
}