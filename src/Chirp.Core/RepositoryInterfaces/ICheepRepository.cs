using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

public interface ICheepRepository
{
    public Task<List<Cheep>> ReadCheeps(int page, int pageSize);
    public Task<List<Cheep>> ReadCheepsFrom(int page, int pageSize, int authorId);
    public Task<int> GetTotalCheeps();
    public Task<int> GetTotalCheepsFor(int authorId);
    public Task CreateCheep(CheepDTO newCheep);
    public Task AddCheep(Cheep cheep);
}