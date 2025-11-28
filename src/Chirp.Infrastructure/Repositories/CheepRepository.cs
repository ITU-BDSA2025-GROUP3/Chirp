using Chirp.Core;
using Chirp.Core.DomainModel;
using Chirp.Core.RepositoryInterfaces;
using Chirp.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDbContext _dbContext;

    public CheepRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Cheep>> ReadPublicCheeps(int page, int pageSize, List<string> tags)
    {
        var query = await _dbContext.Cheeps
            .Where(cheep => tags.Count == 0 || cheep.Tags.Any(tags.Contains))
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return query;
    }

    /// <summary>
    /// Returns cheeps on a specified page, filtering by any tags if provided.
    /// </summary>
    /// <param name="page">The page to return cheeps from</param>
    /// <param name="pageSize"></param>
    /// <param name="authorIds"></param>
    /// <param name="tags"> will interpret empty or null as any or none cheep tags are allowed, i.e. no filtering.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"> Is thrown if the page number is less than 1</exception>
    public async Task<List<Cheep>> ReadTimelineCheeps(int page, int pageSize, List<int> authorIds, List<string>? tags)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
        
        var query = await _dbContext.Cheeps
            .Where(cheep => authorIds.Contains(cheep.IdOfAuthor) && (tags == null || tags.Count == 0 || cheep.Tags.Any(tags.Contains)))
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return query;
    }

    public Task<int> GetTotalPublicCheeps(List<string>? tags)
    {
        return _dbContext.Cheeps.CountAsync();
    }

    public Task<int> GetTotalTimelineCheeps(List<int> authorIds, List<string>? tags)
    {
        return _dbContext.Cheeps
            .CountAsync(cheep => authorIds.Contains(cheep.IdOfAuthor) 
                                 && (tags == null || tags.Count == 0 || cheep.Tags.Any(tags.Contains)));
    }

    /// <summary>
    /// creates new cheep messages, checks if user is logged in, identified by unique email
    /// </summary>
    /// <param name="newCheep">new instance of the DTO</param>
    /// <returns></returns>
    /// <exception cref="Exception"> Is thrown if the user does not exist as an author in the database,
    /// in the future will be redirected to loggin/create-user page
    /// </exception>
    public async Task CreateCheep(CheepDTO newCheep)
    {
        var command = await _dbContext.Authors.SingleOrDefaultAsync(a => a.UserName == newCheep.UserName);
        if (command == null)
        {
            throw new Exception("Author does not exist! Create a new author before you can write cheeps to timeline.");
        }

        var cheep = new Cheep()
        {
            IdOfAuthor = command.Id,
            Author = command,
            Text = newCheep.Message, 
            TimeStamp = DateTime.UtcNow,
            Tags = newCheep.Tags
        };
        command.Cheeps.Add(cheep);
        _dbContext.Cheeps.Add(cheep);
        await _dbContext.SaveChangesAsync();
    }
}

