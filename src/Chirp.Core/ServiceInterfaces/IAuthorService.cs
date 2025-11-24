namespace Chirp.Core.ServiceInterfaces;

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
}
