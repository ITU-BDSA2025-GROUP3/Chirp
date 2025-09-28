public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
    
    public List<CheepViewModel> GetPaginatedResult(int currentPage, int pageSize = 10);
    public int GetCount();
}

public class CheepService : ICheepService
{
    // These would normally be loaded from a database for example
    private static readonly List<CheepViewModel> _cheeps = new()
        {
            new CheepViewModel("Helge", "Hello, BDSA students!", UnixTimeStampToDateTimeString(1759049933)),
            new CheepViewModel("Adrian", "Hej, velkommen til kurset.", UnixTimeStampToDateTimeString(1759049901)),
            new CheepViewModel("Kami", "gurugamesh.", UnixTimeStampToDateTimeString(1759049904)),
            new CheepViewModel("Bambi", "yolo.", UnixTimeStampToDateTimeString(1690895308)),
            new CheepViewModel("Hambi", "waow zadong", UnixTimeStampToDateTimeString(1759049911)),
            new CheepViewModel("Grampy", "is that hazel?", UnixTimeStampToDateTimeString(1690895308)),
            new CheepViewModel("Granny", "haagendaz", UnixTimeStampToDateTimeString(1759049904)),
            new CheepViewModel("Plant", "water.", UnixTimeStampToDateTimeString(1690895308)),
        };

    public List<CheepViewModel> GetCheeps()
    {
        return _cheeps;
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return _cheeps.Where(x => x.Author == author).ToList();
    }

    private static string UnixTimeStampToDateTimeString(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }
    public List<CheepViewModel> GetPaginatedResult(int currentPage, int pageSize)
    {
        if (currentPage < 1) currentPage = 1; // guard clause, against negative pages
        // var page = GetCheeps();
        var page = GetCheeps().Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        // return page.OrderBy(d => d.Timestamp).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        return page;
    }

    public int GetCount() // counts how many cheeps there are
    {
        return GetCheeps().Count;
    }
}
