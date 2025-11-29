using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

public interface ICommentRepository
{
    public Task<List<Cheep>> GetCommentsList(int cheepId);
    public Task CreateComment(string nameOfAuthorCommenting, CommentDTO newComment, int cheepId);
}