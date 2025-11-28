namespace Chirp.Core.ServiceInterfaces;

public interface ICheepService
{
    int CurrentPage { get; set; }
    public Task<List<CheepDTO>> GetCheeps(List<string>? tags);
    public Task<int> GetTotalCheeps(List<string>? tags);
    public Task AddNewCheep(string author, string message, List<string>? tags); 
}
