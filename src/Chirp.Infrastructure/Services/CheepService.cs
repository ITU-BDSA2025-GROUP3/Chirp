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
    
    // TODO this method is called from the page model,
    // TODO we need to retrieve the author commenting, the comment's message content, and the cheep being commented on
    public async Task AddNewComment(string author, string message, CheepDTO cheep)
    {
        // if (string.IsNullOrWhiteSpace(author))
        //     throw new ValidationException("Author is required.");
        // if (string.IsNullOrWhiteSpace(message))
        //     throw new ValidationException("Cheep cannot be empty.");
        // if (message.Length > 160)
        //     throw new ValidationException("Cheeps cannot exceed 160 characters.");
        //
        // TODO convert incoming cheepDTO into an ID
        // var cheepID = _cheepRepository.getCheepId();
        var comment = new CheepDTO
        {
            UserName = author,
            Message = message,
            TimeStamp = new DateTimeOffset(DateTime.UtcNow)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture)
        };
        await _cheepRepository.CreateComment(author, comment, cheepId);
    }
    
    // TODO this method needs to retrieve all comments associated to a specific cheep post,
    // and construct them into cheepDTO's to be used in the frontend
    public async Task<List<CheepDTO>> GetComments()
    {
        var comments = await _cheepRepository.GetCommentsList(CurrentPage, PAGE_SIZE);
        var cheepDTOs = comments.Select(cheep => new CheepDTO
        {
            UserName = cheep.Author.UserName,
            Message = cheep.Text,
            TimeStamp = new DateTimeOffset(cheep.TimeStamp)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture)
        }).ToList();
        return cheepDTOs;
    }

    public async Task<List<CheepDTO>> GetCheeps()
    {
        var cheeps = await _cheepRepository.ReadPublicCheeps(CurrentPage, PAGE_SIZE);
        var cheepDTOs = cheeps.Select(cheep => new CheepDTO
        {
            UserName = cheep.Author.UserName,
            Message = cheep.Text,
            TimeStamp = new DateTimeOffset(cheep.TimeStamp)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture)
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