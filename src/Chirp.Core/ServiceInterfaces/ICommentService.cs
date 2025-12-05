namespace Chirp.Core.ServiceInterfaces;

/// <summary>
/// Interface for the core service level methods of the Chirp Application for interacting with data related to Comments.
/// These methods are intended to use the AuthorRepository methods applicable for the service level.
/// </summary>
public interface ICommentService
{
    public Task<List<CommentDTO>> GetComments();
    public Task AddNewComment(string author, string comment, int cheepId); 
}