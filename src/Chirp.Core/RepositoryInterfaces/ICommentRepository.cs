using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;


/// <summary>
/// Interface for the repository level methods of the Chirp Application for interacting with data related to Comments on Cheeps.
/// These methods are NOT intended to be used directly in the pages. Use Service methods for that.
/// </summary>
public interface ICommentRepository
{
    public Task<List<Comment>> GetCommentsList();
    public Task CreateComment(CommentDTO newComment);
}