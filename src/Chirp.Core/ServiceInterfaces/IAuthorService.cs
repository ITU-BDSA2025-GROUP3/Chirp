namespace Chirp.Core.ServiceInterfaces;

public interface IAuthorService
{
    int CurrentPage { get; set; }
    public Task<List<CheepDTO>> GetAuthorCheeps(string author);
    public Task<int> GetTotalAuthorCheeps(string author);
}
