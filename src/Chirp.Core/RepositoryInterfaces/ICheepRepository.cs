using Chirp.Core.DomainModel;

namespace Chirp.Core.RepositoryInterfaces;

/// <summary>
/// Interface for the repository level methods of the Chirp Application for interacting with data related to Cheeps.
/// These methods are NOT intended to be used directly in the pages. Use Service methods for that.
/// </summary>
public interface ICheepRepository
{
    public Task<List<Cheep>> ReadPublicCheeps(int page, int pageSize);
    public Task<List<Cheep>> ReadTimelineCheeps(int page, int pageSize, List<int> authorIds);
    public Task<int> GetTotalPublicCheeps();
    public Task<int> GetTotalTimelineCheeps(List<int> authorIds);
    public Task CreateCheep(CheepDTO newCheep);
}