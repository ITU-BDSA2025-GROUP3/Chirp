using Chirp.Core;
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

    public async Task<int> GetAuthorId(string authorNameOrEmail)
    {
        if (string.IsNullOrWhiteSpace(authorNameOrEmail)) return 0;
        var query = await _dbContext.Authors
            .Where(author => author.UserName == authorNameOrEmail || author.Email == authorNameOrEmail)
            .Select(author => author.Id)
            .FirstOrDefaultAsync();
        return query;
    }

    public async Task<List<int>> GetAuthorIDs(int authorId)
    {
        var authorIds = await _dbContext.Authors
            .Where(author => author.Id == authorId)
            .SelectMany(author => author.Follows.Select(followed => followed.Id))
            .ToListAsync();
        authorIds.Add(authorId);
        authorIds = authorIds.Distinct().ToList();
        return authorIds;
    }

    public async Task<List<Author>> GetFollowedList(string authorNameOrEmail)
    {
        if (string.IsNullOrWhiteSpace(authorNameOrEmail)) return new List<Author>();
        var query = await _dbContext.Authors
            .Where(author => author.UserName == authorNameOrEmail || author.Email == authorNameOrEmail)
            .Include(author => author.Follows)
            .FirstOrDefaultAsync();
        return query?.Follows.ToList() ?? new List<Author>();
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
            .Where(author => author.UserName == nameOfAuthorToAdd || author.Email == nameOfAuthorToAdd)
            .Select(author => author)
            .FirstOrDefaultAsync();
        var authorToAddTo = await _dbContext.Authors
            .Where(author => author.UserName == nameOfAuthorFollowing  || author.Email == nameOfAuthorFollowing)
            .Select(author => author)
            .FirstOrDefaultAsync();
        
        if (authorToAdd == null || authorToAddTo == null)
            throw new InvalidOperationException($"Author not found:{nameOfAuthorToAdd}&{nameOfAuthorFollowing}");
        
        authorToAddTo.Follows.Add(authorToAdd);
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
            .Where(author => author.UserName == nameOfAuthorToRemove || author.Email == nameOfAuthorToRemove)
            .Select(author => author)
            .FirstOrDefaultAsync();
        var authorToRemoveFrom = await _dbContext.Authors
            .Where(author => author.UserName == nameOfAuthorFollowing || author.Email == nameOfAuthorFollowing)
            .Select(author => author)
            .FirstOrDefaultAsync();
        if (authorToRemove == null || authorToRemoveFrom == null)
            throw new InvalidOperationException($"Author not found:{authorToRemove}&{authorToRemoveFrom}");
        
        authorToRemoveFrom.Follows.Remove(authorToRemove);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Completely removes an Author from the Chirp Application and all references to it in the database. It returns the Author object just deleted.
    /// <b>WARNING:</b> This cannot be undone completely e.x other Authors will lose their references to this object!
    /// </summary>
    /// <param name="authorNameOrEmail">username or email of the Author</param>
    /// <returns>The Deleted Author</returns>
    /// <exception cref="ArgumentException">If the parameter is null or is whitespace</exception>
    public async Task<Author> DeleteAuthor(string authorNameOrEmail)
    {
        if (string.IsNullOrWhiteSpace(authorNameOrEmail))
            throw new ArgumentException("authorNameOrEmail must not be null or whitespace!");

        var authorToRemove = await _dbContext.Authors
            .Where(author => author.UserName == authorNameOrEmail || author.Email == authorNameOrEmail)
            .Select(author => author).Include(author => author.Follows)
            .FirstAsync();

        //Remove all references to all people we follow
        authorToRemove.Follows.Clear();
        
        _dbContext.Remove(authorToRemove);
        await _dbContext.SaveChangesAsync();
        return authorToRemove;
    }

    public async Task<Author?> GetAuthor(string  authorNameOrEmail)
    {
        if (string.IsNullOrWhiteSpace(authorNameOrEmail)) return null;
        var query = await _dbContext.Authors
            .Where(author => author.UserName == authorNameOrEmail || author.Email == authorNameOrEmail)
            .FirstOrDefaultAsync();
        return query;
    }
}

