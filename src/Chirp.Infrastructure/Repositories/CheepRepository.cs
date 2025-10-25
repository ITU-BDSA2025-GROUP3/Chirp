using System.Globalization;

using Chirp.Core.DomainModel;
using Chirp.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

using Author = Chirp.Core.DomainModel.Author;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private const int PAGE_SIZE = 32;
    private readonly ChirpDbContext _dbContext;
    public CheepRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
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

    /// <summary>
    /// creates new cheep messages, checks if user is logged in, identified by unique email
    /// </summary>
    /// <param name="newCheep">new instance of the DTO</param>
    /// <returns></returns>
    /// <exception cref="Exception"> Is thrown if the user does not exist as an author in the database,
    /// in the future will be redirected to loggin/create-user page
    /// </exception>
    public async Task CreateCheep(CheepDTO newCheep) {
        var command = await _dbContext.Authors.SingleOrDefaultAsync(a => a.Name == newCheep.Author);
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

    public Task<int> GetTotalCheeps()
    {
        return _dbContext.Cheeps.CountAsync();
    }
    
    /// <summary>
    /// Adds a new cheep to the database.
    /// </summary>
    /// <param name="cheep">The cheep to be added</param>
    public async Task AddCheep(Cheep cheep)
    {
        _dbContext.Cheeps.Add(cheep);
        await _dbContext.SaveChangesAsync();
    }

}

