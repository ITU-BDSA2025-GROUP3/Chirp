using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

public interface IAuthorRepository
{
    public Task<int> GetAuthorIdFrom(string authorNameOrEmail);
    public Task CreateAuthor(string authorName, string authorEmail);

    public Task<Author> GetAuthor(string authorNameOrEmail);
}