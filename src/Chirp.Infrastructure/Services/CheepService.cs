using System.Globalization;

using Chirp.Core.DomainModel;
using Chirp.Infrastructure.Repositories;

namespace Chirp.Infrastructure.Services;

public interface ICheepService
{
    int CurrentPage { get; set; }
    public Task<List<CheepDTO>> GetCheeps();
    public Task<int> GetTotalCheeps();
    public Task AddNewCheep(string authorname, string message);
}

public class CheepService : ICheepService
{
    private const int PAGE_SIZE = 32;
    //Sets confirguable databse path
    // private readonly ChirpDbContext _chirpDbContext;
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    //Set or get the currentPage to be viewed
    public int CurrentPage { get; set; } = 1;
    public CheepService(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }
    
    public async Task<List<CheepDTO>> GetCheeps()
    {
        var cheeps = await _cheepRepository.ReadCheeps(CurrentPage);
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
    
    public async Task<int> GetTotalCheeps()
    {
        var total = await _cheepRepository.GetTotalCheeps();
        return Math.Max(1, (total + PAGE_SIZE - 1) / PAGE_SIZE);
    }

    public async Task AddNewCheep(String authorname, string message)
    {
        var cheepDTOs = new CheepDTO
        {
            Author = authorname,
            Message = message,
            TimeStamp = new DateTimeOffset(DateTime.UtcNow)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture)
        };
        await _cheepRepository.CreateCheep(cheepDTOs);
    }
}