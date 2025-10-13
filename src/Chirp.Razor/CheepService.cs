using System.Globalization;

using Chirp.Razor;

using Microsoft.Data.Sqlite;


public interface ICheepService
{
    int CurrentPage { get; set; }
    public Task<List<CheepDTO>> GetCheeps();
    public Task<List<CheepDTO>> GetCheepsFromAuthor(string author);
    public Task<List<CheepDTO>> GetCheepsFromAuthorEmail(string authorEmail);
    public Task<int> GetTotalCheeps();
    public Task<int> GetTotalCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    private const int PAGE_SIZE = 32;
    //Sets confirguable databse path
    // private readonly ChirpDbContext _chirpDbContext;
    private readonly ICheepRepository _cheepRepository;
    //Set or get the currentPage to be viewed
    public int CurrentPage { get; set; } = 1;
    public CheepService(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }
    
    public async Task<List<CheepDTO>> GetCheeps()
    {
        return await _cheepRepository.ReadCheeps(CurrentPage);
    }

    public async Task<List<CheepDTO>> GetCheepsFromAuthor(string author)
    {
        return await _cheepRepository.ReadCheepsName(author, CurrentPage);
    }
    
    public async Task<List<CheepDTO>> GetCheepsFromAuthorEmail(string authorEmail)
    {
        return await _cheepRepository.ReadCheepsEmail(authorEmail, CurrentPage);
    }

    public async Task<int> GetTotalCheeps()
    {
        var total = await _cheepRepository.GetTotalCheeps();
        return Math.Max(1, (total + PAGE_SIZE - 1) / PAGE_SIZE);
    }
    
    public async Task<int> GetTotalCheepsFromAuthor(string author)
    {
        var total = await _cheepRepository.GetTotalCheeps(author);
        return Math.Max(1, (total + PAGE_SIZE - 1) / PAGE_SIZE);
    }
}