using Chirp.Core.DomainModel;
using Chirp.Core.RepositoryInterfaces;
using Chirp.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

using Author = Chirp.Core.DomainModel.Author;

namespace Chirp.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDbContext _dbContext;

    public AuthorRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> GetAuthorIdFrom(string authorNameOrEmail)
    {
        if (string.IsNullOrWhiteSpace(authorNameOrEmail)) return 0;
        var query = await _dbContext.Authors
            .Where(author => author.Name == authorNameOrEmail || author.Email == authorNameOrEmail)
            .Select(author => author.AuthorId)
            .FirstOrDefaultAsync();
        return query;
    }

    public async Task<List<Author>> GetFollowedList(string authorNameOrEmail)
    {
        if (string.IsNullOrWhiteSpace(authorNameOrEmail)) return new List<Author>();
        var query = await _dbContext.Authors
            .Where(author => author.Name == authorNameOrEmail || author.Email == authorNameOrEmail)
            .Include(author => author.Follows)
            .ToListAsync();
        return query;
    }

    /// <summary>
    /// Adds the author of the first argument to the list of authors the 2nd argument follows. 
    /// </summary>
    /// <param name="nameOfAuthorToAdd"></param>
    /// <param name="nameOfAuthorFollowing"></param>
    public async Task AddAuthorToFollows(string nameOfAuthorToAdd, string nameOfAuthorFollowing)
    {
        if (string.IsNullOrWhiteSpace(nameOfAuthorToAdd) || string.IsNullOrWhiteSpace(nameOfAuthorFollowing))
            throw new ArgumentException();
        var authorToAdd = await _dbContext.Authors
            .Where(author => author.Name == nameOfAuthorToAdd)
            .Select(author => author)
            .FirstOrDefaultAsync();
        var authorToAddTo = await _dbContext.Authors
            .Where(author => author.Name == nameOfAuthorFollowing)
            .Select(author => author)
            .FirstOrDefaultAsync();
        authorToAddTo!.Follows.Add(authorToAdd!);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Removes the author of the first argument to the list of authors the 2nd argument follows. 
    /// </summary>
    /// <param name="nameOfAuthorToRemove"></param>
    /// <param name="nameOfAuthorFollowing"></param>
    public async Task RemoveAuthorFromFollows(string nameOfAuthorToRemove, string nameOfAuthorFollowing)
    {
        if (string.IsNullOrWhiteSpace(nameOfAuthorToRemove) || string.IsNullOrWhiteSpace(nameOfAuthorFollowing))
            throw new ArgumentException();
        var authorToRemove = await _dbContext.Authors
            .Where(author => author.Name == nameOfAuthorToRemove)
            .Select(author => author)
            .FirstOrDefaultAsync();
        var authorToRemoveFrom = await _dbContext.Authors
            .Where(author => author.Name == nameOfAuthorFollowing)
            .Select(author => author)
            .FirstOrDefaultAsync();
        authorToRemoveFrom!.Follows.Remove(authorToRemove!);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// creates new author, checks if the author already exists in the database, if not then create new author
    /// </summary>
    /// <param name="authorName">Name of the author, checks database existence on this param as it is unique</param>
    /// <param name="authorEmail">Email of the author</param>
    /// <returns></returns>
    /// <exception cref="Exception"> Is thrown if the user already exists as an author in the database,
    /// in the future will be redirected to loggin page or automagically log in user. 
    /// </exception>
    public async Task CreateAuthor(string authorName, string authorEmail)
    {
        var author = new Author
        {
            AuthorId = 0, //TODO why does this work?
            Name = authorName.Trim(),
            Email = authorEmail.Trim(),
            Cheeps = new List<Cheep>(),
            Follows = new List<Author>()
        };
        _dbContext.Authors.Add(author);
        await _dbContext.SaveChangesAsync();
    }
}