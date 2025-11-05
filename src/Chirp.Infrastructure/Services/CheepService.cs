using System.Globalization;

using Chirp.Core;
using Chirp.Core.RepositoryInterfaces;
using Chirp.Core.ServiceInterfaces;

namespace Chirp.Infrastructure.Services;
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
        var cheeps = await _cheepRepository.ReadCheeps(CurrentPage, PAGE_SIZE);
        var cheepDTOs = cheeps.Select(cheep => new CheepDTO
        {
            Author = cheep.Author.Name,
            Message = cheep.Text,
            TimeStamp = new DateTimeOffset(cheep.TimeStamp)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();
        return cheepDTOs;
    }
    
    public async Task<int> GetTotalCheeps()
    {
        var total = await _cheepRepository.GetTotalCheeps();
        return Math.Max(1, (total + PAGE_SIZE - 1) / PAGE_SIZE);
    }
}