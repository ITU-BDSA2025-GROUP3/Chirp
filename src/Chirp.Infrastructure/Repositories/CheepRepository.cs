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
    
    // TODO query the backend for the cheep to append the comment to, need to alsop retrieve who the commenter author is
    public async Task CreateComment(String nameOfAuthorCommenting, CheepDTO newComment, int CheepId)
    {
        // retrieve the ID of the cheep being commented on
        var cheep = await _dbContext.Cheeps.SingleOrDefaultAsync(cheep => cheep.CheepId == CheepId);
        // retrieve the author who is commenting
        var author = await _dbContext.Users.SingleOrDefaultAsync(user => user.UserName == newComment.UserName);
        // create a new comment with the content of the posted message
        var comment = new Cheep()
        {
            IdOfAuthor = author.Id,
            Author = author,
            Text = "", 
            TimeStamp = DateTime.UtcNow,
        };
        
        cheep.Comments.Add(comment);
        _dbContext.Cheeps.Add(comment);
        await _dbContext.SaveChangesAsync();
    }
        
    // TODO get from DB a list of cheep comments assoctiated with a specific post
    public async Task<List<Cheep>> GetCommentsList(int page, int pageSize)
    {
        var query = await _dbContext.Cheeps
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return query;
    }
    
    public async Task<List<Cheep>> ReadPublicCheeps(int page, int pageSize)
    {
        var query = await _dbContext.Cheeps
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return query;
    }
    
    /// <summary>
    /// Returns cheeps on a specified page.
    /// </summary>
    /// <param name="page">The page to return cheeps from</param>
    /// <param name="pageSize"></param>
    /// <param name="authorIds"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"> Is thrown if the page number is less than 1</exception>
    public async Task<List<Cheep>> ReadTimelineCheeps(int page, int pageSize, List<int> authorIds)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));
        
        var query = await _dbContext.Cheeps
            .Where(cheep => authorIds.Contains(cheep.IdOfAuthor))
            .Include(cheep => cheep.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return query;
    }

    public Task<int> GetTotalPublicCheeps()
    {
        return _dbContext.Cheeps.CountAsync();
    }

    public Task<int> GetTotalTimelineCheeps(List<int> authorIds)
    {
        return _dbContext.Cheeps.CountAsync(cheep => authorIds.Contains(cheep.IdOfAuthor));
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
            Comments = new List<Cheep>(),
        };
        command.Cheeps.Add(cheep);
        _dbContext.Cheeps.Add(cheep);
        await _dbContext.SaveChangesAsync();
    }
}

