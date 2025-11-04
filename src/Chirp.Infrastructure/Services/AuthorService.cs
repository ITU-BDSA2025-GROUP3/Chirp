using System.Globalization;
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
    //Sets confirguable database path
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    //Set or get the currentPage to be viewed
    public int CurrentPage { get; set; } = 1;
    public AuthorService(IAuthorRepository authorRepository, ICheepRepository cheepRepository)
    {
        _authorRepository = authorRepository;
        _cheepRepository = cheepRepository;
    }
    public async Task<List<CheepDTO>> GetAuthorCheeps(string author)
    {
        var authorId = await _authorRepository.GetAuthorIdFrom(author);
        if (authorId == 0) return new List<CheepDTO>();
        var cheeps = await _cheepRepository.ReadCheepsFrom(CurrentPage, PAGE_SIZE, authorId);
        var cheepDTOs = cheeps.Select(cheep => new CheepDTO
        {
            Author = cheep.Author.Name,
            Message = cheep.Text,
            TimeStamp = new DateTimeOffset(cheep.TimeStamp)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();
        return cheepDTOs;
    }
    
    public async Task<int> GetTotalAuthorCheeps(string author)
    {
        var authorId = await _authorRepository.GetAuthorIdFrom(author);
        if (authorId == 0) return 1;
        
        var total = await _cheepRepository.GetTotalCheepsFor(authorId);
        return Math.Max(1, (total + PAGE_SIZE - 1) / PAGE_SIZE);
    }
}