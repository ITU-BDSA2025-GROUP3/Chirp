using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

public interface ICheepRepository
{
    public Task<List<Cheep>> ReadPublicCheeps(int page, int pageSize, List<string>? tags);
    public Task<List<Cheep>> ReadTimelineCheeps(int page, int pageSize, List<int> authorIds, List<string>? tags);
    public Task<int> GetTotalPublicCheeps(List<string> tags);
    public Task<int> GetTotalTimelineCheeps(List<int> authorIds, List<string> tags);
    public Task CreateCheep(CheepDTO newCheep);
}