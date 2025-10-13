namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadCheepsName(string authorName, int page);
    
    public Task<List<CheepDTO>> ReadCheepsEmail(string authorEmail, int page);

    public Task<List<CheepDTO>> ReadCheeps(int page);
    
    public Task CreateCheep(CheepDTO newCheep, int authorId);

    public Task CreateAuthor(string authorName, string authorEmail);
    
    public Task<int> GetTotalCheeps();

    public Task<int> GetTotalCheeps(string authorName);
}