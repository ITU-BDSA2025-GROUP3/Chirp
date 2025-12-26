using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

public interface ICommentRepository
{
    public Task<List<Comment>> GetCommentsList();
    public Task<List<Comment>> GetCommentsListFromUser(String username);
    public Task CreateComment(CommentDTO newComment);
}