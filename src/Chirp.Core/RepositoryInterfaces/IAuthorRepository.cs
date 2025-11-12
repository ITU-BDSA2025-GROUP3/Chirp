using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

public interface IAuthorRepository
{
    public Task<int> GetAuthorIdFrom(string authorNameOrEmail);
    public Task CreateAuthor(string authorName, string authorEmail);
    public Task<List<Author>> GetFollowedList(string authorName);
    public Task AddAuthorToFollows(string nameOfAuthorToAdd, string nameOfAuthorFollowing);
    public Task RemoveAuthorFromFollows(string nameOfAuthorToRemove, string nameOfAuthorFollowing);

}