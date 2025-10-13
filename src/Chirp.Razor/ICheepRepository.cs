using Chirp.Razor.DomainModel;

namespace Chirp.Razor;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadCheeps(string authorName, int page);

    public Task<List<CheepDTO>> ReadCheeps(int page);

    public Task<int> GetTotalCheeps();

    public Task<int> GetTotalCheeps(string authorName);
    
    public Task AddCheep(Cheep cheep);
    
}
