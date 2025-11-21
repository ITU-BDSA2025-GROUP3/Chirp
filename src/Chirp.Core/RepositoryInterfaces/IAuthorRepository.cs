using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

public interface IAuthorRepository
{
    public Task<int> GetAuthorID(string authorNameOrEmail);
    public Task<List<int>> GetAuthorIDs(int authorId);
    public Task<List<Author>> GetFollowedList(string authorName);
    public Task AddAuthorToFollows(string nameOfAuthorToAdd, string nameOfAuthorFollowing);
    public Task RemoveAuthorFromFollows(string nameOfAuthorToRemove, string nameOfAuthorFollowing);
    
    public Task<Author> GetAuthor(string authorNameOrEmail);
}