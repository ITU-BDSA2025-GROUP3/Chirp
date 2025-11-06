using Chirp.Core;
using Chirp.Core.DomainModel;
using Chirp.Core.RepositoryInterfaces;
using Chirp.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private const int PAGE_SIZE = 32;
    private readonly ChirpDbContext _dbContext;

    public CheepRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Cheep>> ReadCheeps(int page)
    {
        var query = await _dbContext.Cheeps
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * PAGE_SIZE)
            .Take(PAGE_SIZE)
            .ToListAsync();
        return query;
    }


    /// <summary>
    /// Returns cheeps on a specified page.
    /// </summary>
    /// <param name="page">The page to return cheeps from</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"> Is thrown if the page number is less than 1</exception>
    public async Task<List<Cheep>> ReadCheepsFrom(int page, int authorId)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        var query = await _dbContext.Cheeps
            .Where(cheep => cheep.AuthorId == authorId)
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * PAGE_SIZE)
            .Take(PAGE_SIZE)
            .ToListAsync();
        return query;
    }

    public Task<int> GetTotalCheeps()
    {
        return _dbContext.Cheeps.CountAsync();
    }

    public Task<int> GetTotalCheepsFor(int authorId)
    {
        return _dbContext.Cheeps.CountAsync(cheep => cheep.AuthorId == authorId);
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
        var command = await _dbContext.Authors.SingleOrDefaultAsync(a => a.Email == newCheep.Author);
        if (command == null)
        {
            throw new Exception("Author does not exist! Create a new author before you can write cheeps to timeline.");
        }

        var cheep = new Cheep()
        {
            AuthorId = command.AuthorId,
            Author = command,
            Text = newCheep.Message, 
            TimeStamp = DateTime.UtcNow,
        };
        _dbContext.Cheeps.Add(cheep);
        await _dbContext.SaveChangesAsync();
    }
}

