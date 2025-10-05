using System.Globalization;

using Chirp.Razor.DomainModel;

using Microsoft.EntityFrameworkCore;

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
    public async Task<List<CheepDTO>> ReadCheeps(string authorName, int page)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        var query = await (
                from author in _dbContext.Authors
                join cheep in _dbContext.Cheeps on author.AuthorId equals cheep.AuthorId
                where author.Name == authorName
                orderby cheep.TimeStamp descending
                select new { author.Name, cheep.Text, TimeStamp = cheep.TimeStamp })
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
                select new { author.Name, cheep.Text, TimeStamp = cheep.TimeStamp })
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

