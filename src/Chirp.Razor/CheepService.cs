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
    private readonly DBFacade _DBFacade;
    public CheepService()
    {
        // get the databse path from a variable or our temp directory
        var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ?? Path.Combine(Path.GetTempPath(), "chirp.db");
        _DBFacade = new DBFacade($"Data Source={dbPath}");
    }
    
    public List<CheepViewModel> GetCheeps()
    {
        return _DBFacade.GetAllCheeps();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        return _DBFacade.GetCheepsFromAuthor(author);
    }
}
