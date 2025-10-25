using System.Globalization;

using Chirp.Core.DomainModel;
using Chirp.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

using Author = Chirp.Core.DomainModel.Author;

namespace Chirp.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private const int PAGE_SIZE = 32;
    private readonly ChirpDbContext _dbContext;
    public AuthorRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns cheeps on a specified page, from a specified author, matched by author name.
    /// </summary>
    /// <param name="author">either Name or Email of the author as it appears in their cheeps</param>
    /// <param name="page">The page to return cheeps from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"> Is thrown if the page number is less than 1</exception>
    public async Task<List<CheepDTO>> ReadAuthorCheeps(string author, int page)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        var query = await (
                from Author in _dbContext.Authors
                join cheep in _dbContext.Cheeps on Author.AuthorId equals cheep.AuthorId
                where Author.Name == author || Author.Email == author
                orderby cheep.TimeStamp descending
                select new { Author.Name, cheep.Text, cheep.TimeStamp })
            .Skip((page - 1) * PAGE_SIZE) // offset equivalent
            .Take(PAGE_SIZE) //limit equivalent
            .ToListAsync();

        return query.Select(cheep => new CheepDTO
        {
            Author = cheep.Name,
            Message = cheep.Text,
            TimeStamp = new DateTimeOffset(cheep.TimeStamp).ToLocalTime().ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();
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
        var nameNormalized = authorName.Trim().ToLowerInvariant();
        var command = await _dbContext.Authors.FirstOrDefaultAsync(author => author.Name.ToLower() == nameNormalized);

        if (command != null)
        {
            throw new Exception("Author exists! logged in now as <user>");
        }

        var author = new Author
        {
            AuthorId = 0,
            Name = authorName.Trim(), 
            Email = authorEmail.Trim(), 
            Cheeps = new List<Cheep>()
        };
        _dbContext.Authors.Add(author);
        await _dbContext.SaveChangesAsync();
    }

    public Task<int> GetTotalCheeps()
    {
        return _dbContext.Cheeps.CountAsync();
    }
    
    public Task<int> GetTotalAuthorCheeps(string authorName)
    {
        var query = (
            from author in _dbContext.Authors
            join cheep in _dbContext.Cheeps on author.AuthorId equals cheep.AuthorId
            where author.Name == authorName || author.Email == authorName
            select cheep.CheepId);
        return query.CountAsync();
    }
}

