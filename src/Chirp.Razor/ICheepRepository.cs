namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadAuthorCheeps(string authorName, int page);

    public Task<List<CheepDTO>> ReadCheeps(int page);
    
    public Task CreateCheep(CheepDTO newCheep, int authorId);

    public Task CreateAuthor(string authorName, string authorEmail);
    
    public Task<int> GetTotalCheeps();

    public Task<int> GetTotalAuthorCheeps(string authorName);
}