namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadCheeps(string authorName, int page);

    public Task<List<CheepDTO>> ReadCheeps(int page);

    public Task<int> GetTotalPages();

    public Task<int> GetTotalPages(string authorName);
}