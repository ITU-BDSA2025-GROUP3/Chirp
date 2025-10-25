using Chirp.Infrastructure.Repositories;

namespace Chirp.Infrastructure.Services;

public interface ICheepService
{
    int CurrentPage { get; set; }
    public Task<List<CheepDTO>> GetCheeps();
    public Task<int> GetTotalCheeps();
}

public class CheepService : ICheepService
{
    private const int PAGE_SIZE = 32;
    //Sets confirguable databse path
    // private readonly ChirpDbContext _chirpDbContext;
    private readonly ICheepRepository _cheepRepository;
    //Set or get the currentPage to be viewed
    public int CurrentPage { get; set; } = 1;
    public CheepService(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }
    
    public async Task<List<CheepDTO>> GetCheeps()
    {
        return await _cheepRepository.ReadCheeps(CurrentPage);
    }
    
    public async Task<int> GetTotalCheeps()
    {
        var total = await _cheepRepository.GetTotalCheeps();
        return Math.Max(1, (total + PAGE_SIZE - 1) / PAGE_SIZE);
    }
}