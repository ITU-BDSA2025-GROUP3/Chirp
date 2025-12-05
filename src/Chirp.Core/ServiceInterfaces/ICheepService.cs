namespace Chirp.Core.ServiceInterfaces;

/// <summary>
/// Interface for the core service level methods of the Chirp Application for interacting with data related to Cheeps in general.
/// These methods are intended to use the AuthorRepository methods applicable for the service level.
/// </summary>
public interface ICheepService
{
    int CurrentPage { get; set; }
    public Task<List<CheepDTO>> GetCheeps();
    public Task<int> GetTotalCheeps();
    public Task AddNewCheep(string author, string message); 
}
