using System.Globalization;

using Chirp.Razor;

using Microsoft.Data.Sqlite;


public interface ICheepService
{
    int CurrentPage { get; set; }
    public List<CheepDTO> GetCheeps();
    public List<CheepDTO> GetCheepsFromAuthor(string author);
    public int GetTotalPages();
    public int GetTotalPagesFromAuthor(string author);
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
        // get the databse path from a variable or our temp directory
        // var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? Path.Combine(Directory.GetCurrentDirectory(), "chirp.db");
        // _chirpDbContext = new ChirpDbContext($"Data Source={dbPath}");
        _cheepRepository = cheepRepository;
    }
    
    public List<CheepDTO> GetCheeps()
    {
        return _cheepRepository.ReadCheeps(CurrentPage).GetAwaiter().GetResult();
    }

    public List<CheepDTO> GetCheepsFromAuthor(string author)
    {
        return _cheepRepository.ReadCheeps(author, CurrentPage).GetAwaiter().GetResult();
    }

    public int GetTotalPages()
    {
        var total = _cheepRepository.GetTotalPages().GetAwaiter().GetResult();
        return Math.Max(1, (int)Math.Ceiling(total / (double)PAGE_SIZE));
    }
    
    public int GetTotalPagesFromAuthor(string author)
    {
        var total = _cheepRepository.GetTotalPages(author).GetAwaiter().GetResult();
        return Math.Max(1, (int)Math.Ceiling(total / (double)PAGE_SIZE));
    }
}