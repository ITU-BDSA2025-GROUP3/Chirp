namespace Chirp.Core.ServiceInterfaces;

public interface ICheepService
{
    public Task<List<CheepDTO>> GetComments();
    public Task AddNewComment(string author, string message, CheepDTO cheep); 
    int CurrentPage { get; set; }
    public Task<List<CheepDTO>> GetCheeps();
    public Task<int> GetTotalCheeps();
    public Task AddNewCheep(string author, string message); 
}
