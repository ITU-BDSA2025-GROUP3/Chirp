using System.Globalization;
using Chirp.Core;
using Chirp.Core.DomainModel;
using Chirp.Core.RepositoryInterfaces;
using Chirp.Core.ServiceInterfaces;

namespace Chirp.Infrastructure.Services;

public class AuthorService : IAuthorService
{
    private const int PAGE_SIZE = 32;
    //Sets confirguable database path
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    //Set or get the currentPage to be viewed
    public int CurrentPage { get; set; } = 1;
    public AuthorService(IAuthorRepository authorRepository, ICheepRepository cheepRepository)
    {
        _authorRepository = authorRepository;
        _cheepRepository = cheepRepository;
    }
    public async Task<List<CheepDTO>> GetAuthorCheeps(string author)
    {
        var authorId = await _authorRepository.GetAuthorId(author);
        if (authorId == 0) return new List<CheepDTO>();
        var authorIds = await _authorRepository.GetAuthorIDs(authorId);
        
        var cheeps = await _cheepRepository.ReadTimelineCheeps(CurrentPage, PAGE_SIZE, authorIds);
        var cheepDTOs = cheeps.Select(cheep => new CheepDTO
        {
            UserName = cheep.Author.UserName,
            Message = cheep.Text,
            TimeStamp = new DateTimeOffset(cheep.TimeStamp)
                .ToLocalTime()
                .ToString("MM/dd/yy H:mm:ss", CultureInfo.InvariantCulture),
            CheepId = cheep.CheepId
        }).ToList();
        return cheepDTOs;
    }
    
    public async Task<int> GetTotalAuthorCheeps(string author)
    {
        var authorId = await _authorRepository.GetAuthorId(author);
        if (authorId == 0) return 1;
        var authorIds = await _authorRepository.GetAuthorIDs(authorId);
        
        var total = await _cheepRepository.GetTotalTimelineCheeps(authorIds);
        return Math.Max(1, (total + PAGE_SIZE - 1) / PAGE_SIZE);
    }

    public async Task<bool> AuthorExists(string usernameOrEmail)
    {
        var id = await _authorRepository.GetAuthorId(usernameOrEmail);
        if (id == 0)
        {
            return false;
        }
        return true;
    }

    public async Task<List<AuthorDTO>> GetFollowsList(string author)
    {
       
        var follows = await _authorRepository.GetFollowedList(author);
        if (follows.Count == 0) return [];
        var authorDTOs = follows.Select(author => new AuthorDTO { Name = author.UserName }).ToList();
        return authorDTOs;
    }

    public async Task RemoveAuthorFromFollowsList(string authorToRemove, string fromAuthor)
    {
        await _authorRepository.RemoveAuthorFromFollows(authorToRemove, fromAuthor);
    }

    public async Task AddAuthorToFollowsList(string authorToAdd, string toAuthor)
    {
        await _authorRepository.AddAuthorToFollows(authorToAdd, toAuthor);
    }

    /// <summary>
    /// Completely removes an Author from the Chirp Application and all references to it in the database.
    /// It returns the AuthorDTO object just deleted for display purposes.
    /// <b>WARNING:</b> This cannot be undone completely e.x other Authors will lose their references to this object!
    /// </summary>
    /// <param name="authorNameOrEmail"></param>
    /// <returns>An AuthorDTO object representing the Author just deleted. For UI display purposes</returns>
    public async Task<AuthorDTO> DeleteAuthor(string authorNameOrEmail)
    {
        var author = await _authorRepository.DeleteAuthor(authorNameOrEmail);
        return new AuthorDTO
        {
            Name = author.UserName,
            Email = author.Email
        };
    }
}