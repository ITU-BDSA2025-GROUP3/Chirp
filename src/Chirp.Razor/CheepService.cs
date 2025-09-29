using Chirp.Razor;

using Microsoft.Data.Sqlite;


public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    //Sets confirguable databse path
    private readonly DBFacade _dbFacade;
    public CheepService()
    {
        // get the databse path from a variable or our temp directory
        var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? Path.Combine(Path.GetTempPath(), "chirp.db");
        _dbFacade = new DBFacade($"Data Source={dbPath}");
    }
    
    public List<CheepViewModel> GetCheeps()
    {
        return _dbFacade.GetAllCheeps();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return _dbFacade.GetCheepsFromAuthor(author);
    }
}
