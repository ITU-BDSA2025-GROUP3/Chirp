namespace Chirp.Infrastructure.Repositories;

public interface IAuthorRepository
{
    public Task<List<CheepDTO>> ReadAuthorCheeps(string authorName, int page);
    
    public Task CreateAuthor(string authorName, string authorEmail);

    public Task<int> GetTotalAuthorCheeps(string authorName);
}