using Chirp.Razor;

using Microsoft.Data.Sqlite;


public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    int CurrentPage { get; set; }
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
    public int GetTotalPages();
    public int GetTotalPagesFromAuthor(string author);
}

public class CheepService : ICheepService
{
    //Sets confirguable databse path
    private readonly ChirpDbContext _chirpDbContext;
    //Set or get the currentPage to be viewed
    public int CurrentPage { get; set; } = 1;
    /*public CheepService(IConfiguration configuration)
    {
        // get the databse path from a variable or our temp directory
        var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? Path.Combine(Directory.GetCurrentDirectory(), "chirp.db");
        _chirpDbContext = new ChirpDbContext($"Data Source={dbPath}");
    }*/
    
    public List<CheepViewModel> GetCheeps()
    {
        return _chirpDbContext.GetAllCheeps(CurrentPage);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return _chirpDbContext.GetCheepsFromAuthor(author, CurrentPage);
    }

    public int GetTotalPages()
    {
        return _chirpDbContext.GetTotalPages();
    }

    public int GetTotalPagesFromAuthor(string author)
    {
        return _chirpDbContext.GetTotalPagesFromAuthor(author);
    }
}