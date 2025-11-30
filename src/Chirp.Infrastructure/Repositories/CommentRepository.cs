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
    
    public async Task CreateComment(CommentDTO newComment)
    {
        // retrieve the author who is commenting
        var author = await _dbContext.Users.SingleOrDefaultAsync(user => user.UserName == newComment.UserName);
        // create a new comment with the content of the posted message
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
    
    public async Task<List<Comment>> GetCommentsList()
    {
        var query = await _dbContext.Comments
            .Include(comment => comment.Author)
            .OrderByDescending(comment => comment.TimeStamp)
            .ToListAsync();
        return query;
    }
}