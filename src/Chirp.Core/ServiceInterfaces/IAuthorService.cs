namespace Chirp.Core.ServiceInterfaces;

/// <summary>
/// Interface for the core service level methods of the Chirp Application for interacting with data related to Authors.
/// These methods are intended to use the AuthorRepository methods applicable for the service level.
/// </summary>
public interface IAuthorService
{
    int CurrentPage { get; set; }
    public Task<List<CheepDTO>> GetAuthorCheeps(string author);
    public Task<int> GetTotalAuthorCheeps(string author);
    public Task<List<AuthorDTO>> GetFollowsList(string author);
    public Task RemoveAuthorFromFollowsList(string authorToRemove, string fromAuthor);
    public Task AddAuthorToFollowsList(string authorToAdd, string toAuthor);
    public Task<bool> AuthorExists(string author);
    public Task<AuthorDTO> DeleteAuthor(string authorNameOrEmail);
    public Task<AuthorDTO> GetAuthor(string author);
    public Task<List<CheepDTO>> GetMyCheeps(string author);
}
