using Chirp.Core.DomainModel;

namespace Chirp.Infrastructure.Repositories;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> ReadCheeps(int page);
    

    public Task CreateCheep(CheepDTO newCheep);
    
    public Task<int> GetTotalCheeps();
    
    public Task AddCheep(Cheep cheep);
}