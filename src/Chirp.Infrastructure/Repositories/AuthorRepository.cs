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
            .Where(author => author.UserName == authorNameOrEmail || author.Email == authorNameOrEmail)
            .Select(author => author.Id)
            .FirstOrDefaultAsync();
        return query;
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
        var author = new Core.DomainModel.Author
        {
            UserName = authorName.Trim(), 
            Email = authorEmail.Trim(), 
            Cheeps = new List<Cheep>()
        };
        _dbContext.Authors.Add(author);
        await _dbContext.SaveChangesAsync();
    }
}

