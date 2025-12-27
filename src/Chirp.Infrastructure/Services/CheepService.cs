using System.Globalization;
using System.ComponentModel.DataAnnotations;

using Chirp.Core;
using Chirp.Core.RepositoryInterfaces;
using Chirp.Core.ServiceInterfaces;

namespace Chirp.Infrastructure.Services;
public class CheepService : ICheepService
{
    private const int PAGE_SIZE = 32;
    //Sets confirguable databse path
    private readonly ICheepRepository _cheepRepository;
    //Set or get the currentPage to be viewed
    public int CurrentPage { get; set; } = 1;
    public CheepService(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }

    public async Task<List<CheepDTO>> GetCheeps()
    {
        var cheeps = await _cheepRepository.ReadPublicCheeps(CurrentPage, PAGE_SIZE);
        var cheepDTOs = cheeps.Select(cheep => new CheepDTO
        {
            UserName = cheep.Author.UserName ?? "",
            Message = cheep.Text,
            TimeStamp = new DateTimeOffset(cheep.TimeStamp)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture),
            CheepId = cheep.CheepId
        }).ToList();
        return cheepDTOs;
    }
    
    public async Task<int> GetTotalCheeps()
    {
        var total = await _cheepRepository.GetTotalPublicCheeps();
        return Math.Max(1, (total + PAGE_SIZE - 1) / PAGE_SIZE);
    }

    public async Task AddNewCheep(string author, string message)
    {
        if (string.IsNullOrWhiteSpace(author))
            throw new ValidationException("Author is required.");
        if (string.IsNullOrWhiteSpace(message))
            throw new ValidationException("Cheep cannot be empty.");
        if (message.Length > 160)
            throw new ValidationException("Cheeps cannot exceed 160 characters.");
        
        var cheepDTOs = new CheepDTO
        {
            UserName = author,
            Message = message,
            TimeStamp = new DateTimeOffset(DateTime.UtcNow)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture)
        };
        await _cheepRepository.CreateCheep(cheepDTOs);
    }
}