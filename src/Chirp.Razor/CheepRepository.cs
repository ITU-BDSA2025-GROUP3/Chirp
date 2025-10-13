using System.Globalization;

using Chirp.Razor.DomainModel;
using Chirp.Razor.Migrations;

using Microsoft.EntityFrameworkCore;

using Author = Chirp.Razor.DomainModel.Author;

namespace Chirp.Razor;

public class CheepRepository : ICheepRepository
{
    private const int PAGE_SIZE = 32;
    private readonly ChirpDbContext _dbContext;
    public CheepRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns cheeps on a specified page, from a specified author.
    /// </summary>
    /// <param name="authorName">Name of the author as it appears in their cheeps</param>
    /// <param name="page">The page to return cheeps from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"> Is thrown if the page number is less than 1</exception>
    public async Task<List<CheepDTO>> ReadCheepsName(string authorName, int page)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        var query = await (
                from author in _dbContext.Authors
                join cheep in _dbContext.Cheeps on author.AuthorId equals cheep.AuthorId
                where author.Name == authorName
                orderby cheep.TimeStamp descending
                select new { author.Name, cheep.Text, cheep.TimeStamp })
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
    
    public async Task<List<CheepDTO>> ReadCheepsEmail(string authorEmail, int page)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        var query = await (
                from author in _dbContext.Authors
                join cheep in _dbContext.Cheeps on author.AuthorId equals cheep.AuthorId
                where author.Email == authorEmail
                orderby cheep.TimeStamp descending
                select new { author.Name, cheep.Text, cheep.TimeStamp })
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
    /// Returns cheeps on a specified page.
    /// </summary>
    /// <param name="page">The page to return cheeps from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"> Is thrown if the page number is less than 1</exception>
    public async Task<List<CheepDTO>> ReadCheeps(int page)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
        
        var query = await (
                from author in _dbContext.Authors
                join cheep in _dbContext.Cheeps on author.AuthorId equals cheep.AuthorId
                orderby cheep.TimeStamp descending
                select new { author.Name, cheep.Text, cheep.TimeStamp })
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

    public async Task CreateCheep(CheepDTO newCheep, int authorId) {
        var command = await (
            from author in _dbContext.Authors
            where author.AuthorId == authorId
            select new {newCheep.Author, newCheep.Message, newCheep.TimeStamp}
        ).FirstAsync();

        if (command == null)
        {
            throw new Exception("Author does not exist! Create a new author.");
        }
        var parsed = DateTime.ParseExact(
            newCheep.TimeStamp,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeLocal 
        );
        _dbContext.Cheeps.Add(new Cheep() { Text = newCheep.Message, TimeStamp = parsed});
        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateAuthor(string authorName, string authorEmail)
    {
        var command = await (
            from author in _dbContext.Authors
            where author.Name == authorName && author.Email == authorEmail
            select new {author.AuthorId, author.Name, author.Email}
        ).FirstAsync();

        if (command == null)
        {
            _dbContext.Authors.Add(new Author(){Name = authorName, Email = authorEmail, Cheeps = new List<Cheep>()});
            await _dbContext.SaveChangesAsync();
        }

        if (command != null)
        {
            throw new Exception("Author already exists!");
        }
    }

    public Task<int> GetTotalCheeps()
    {
        return _dbContext.Cheeps.CountAsync();
    }
    
    public Task<int> GetTotalCheeps(string authorName)
    {
        var query = (
            from author in _dbContext.Authors
            join cheep in _dbContext.Cheeps on author.AuthorId equals cheep.AuthorId
            where author.Name == authorName
            select cheep.CheepId);
        return query.CountAsync();
    }
}

