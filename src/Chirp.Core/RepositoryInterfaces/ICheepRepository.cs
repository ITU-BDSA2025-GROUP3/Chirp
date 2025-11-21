using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

public interface ICheepRepository
{
    public Task<List<Cheep>> ReadPublicCheeps(int page, int pageSize);
    public Task<List<Cheep>> ReadTimelineCheeps(int page, int pageSize, List<int> authorIds);
    public Task<int> GetTotalCheeps();
    public Task<int> GetTotalCheepsFor(int authorId);
    public Task CreateCheep(CheepDTO newCheep);
}