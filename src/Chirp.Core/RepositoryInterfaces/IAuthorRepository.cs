using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

/// <summary>
/// Interface for the repository level methods of the Chirp Application for interacting with data related to Authors.
/// These methods are NOT intended to be used directly in the pages. Use Service methods for that.
/// </summary>
public interface IAuthorRepository
{
    public Task<int> GetAuthorId(string authorNameOrEmail);
    public Task<List<int>> GetAuthorIDs(int authorId);
    public Task<List<Author>> GetFollowedList(string authorName);
    public Task AddAuthorToFollows(string nameOfAuthorToAdd, string nameOfAuthorFollowing);
    public Task RemoveAuthorFromFollows(string nameOfAuthorToRemove, string nameOfAuthorFollowing);
    public Task<Author> DeleteAuthor(string authorNameOrEmail);
    public Task<Author> GetAuthor(string authorNameOrEmail);
}