namespace Chirp.Core.ServiceInterfaces;

public interface ICommentService
{
    public Task<List<CommentDTO>> GetComments();
    public Task<List<CommentDTO>> GetCommentsListFromUser(String user);
    public Task AddNewComment(string author, string comment, int cheepId); 
}