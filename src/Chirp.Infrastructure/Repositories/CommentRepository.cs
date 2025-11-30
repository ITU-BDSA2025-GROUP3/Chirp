using Chirp.Core;
using Chirp.Core.DomainModel;
using Chirp.Core.RepositoryInterfaces;
using Chirp.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ChirpDbContext _dbContext;

    public CommentRepository(ChirpDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    /// <summary>
    /// Creates a new comment object in the database, retrieves the author who is commenting and creates a new comment with the content of the posted message
    /// </summary>
    /// <param name="newComment">comment DTO object</param>
    public async Task CreateComment(CommentDTO newComment)
    {
        var author = await _dbContext.Users.SingleOrDefaultAsync(user => user.UserName == newComment.UserName);
        var comment = new Comment()
        {
            IdOfAuthor = author.Id,
            Author = author,
            Message = newComment.Comment,
            TimeStamp = DateTime.UtcNow,
            CheepId = newComment.CheepId,
        };
        
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Retrieves a list of all comments
    /// </summary>
    /// <returns>
    /// A query of the resulting comments
    /// </returns>
    public async Task<List<Comment>> GetCommentsList()
    {
        var query = await _dbContext.Comments
            .Include(comment => comment.Author)
            .OrderByDescending(comment => comment.TimeStamp)
            .ToListAsync();
        return query;
    }
}