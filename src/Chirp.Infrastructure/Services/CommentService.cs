using System.ComponentModel.DataAnnotations;

using Chirp.Core.RepositoryInterfaces;
using Chirp.Core.ServiceInterfaces;
using System.Globalization;
using Chirp.Core;
using Chirp.Core.DomainModel;

namespace Chirp.Infrastructure.Services;

public class CommentService : ICommentService
{
    private const int PAGE_SIZE = 32;
    private readonly ICommentRepository _commentRepository;
    public int CurrentPage { get; set; } = 1;
    public CommentService(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }
    
    // TODO this method is called from the page model,
    // TODO we need to retrieve the author commenting, the comment's message content, and the cheep being commented on
    public async Task AddNewComment(string author, string comment, int cheepId)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new ValidationException("comment author is required.");
        if (string.IsNullOrWhiteSpace(comment))
            throw new ValidationException("comment cannot be empty.");
        if (comment.Length > 160)
            throw new ValidationException("comments cannot exceed 160 characters.");
        
        // TODO convert incoming cheepDTO into an ID
        var commentDto = new CommentDTO()
        {
            UserName = author,
            Comment = comment,
            TimeStamp = new DateTimeOffset(DateTime.UtcNow)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture)
        };
        await _commentRepository.CreateComment(author, commentDto, cheepId);
    }
    
    // TODO this method needs to retrieve all comments associated to a specific cheep post,
    // and construct them into cheepDTO's to be used in the frontend
    public async Task<List<CommentDTO>> GetComments()
    {
        var comments = await _commentRepository.GetCommentsList();
        var commentDto = comments.Select(comment => new CommentDTO
        {
            UserName = comment.Author.UserName,
            Comment = comment.Message,
            TimeStamp = new DateTimeOffset(comment.TimeStamp)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture),
            CheepId = comment.CheepId
        }).ToList();

        return commentDto;
    }
}