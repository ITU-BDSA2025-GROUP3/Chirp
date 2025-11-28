using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

public interface ICheepRepository
{
    public Task<List<Cheep>> GetCommentsList(int page, int pageSize);
    public Task CreateComment(string nameOfAuthorCommenting, CheepDTO newComment, int cheepId);
    public Task<List<Cheep>> ReadPublicCheeps(int page, int pageSize);
    public Task<List<Cheep>> ReadTimelineCheeps(int page, int pageSize, List<int> authorIds);
    public Task<int> GetTotalPublicCheeps();
    public Task<int> GetTotalTimelineCheeps(List<int> authorIds);
    public Task CreateCheep(CheepDTO newCheep);
}