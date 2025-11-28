using Chirp.Core;
using Chirp.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Razor.Language;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private const string TagRegexString = "(#\\w+)";
    private static readonly Regex TagRegex = new(TagRegexString);

    public UserTimelineModel(ICheepService cheepService, IAuthorService authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
    }
    
    public required List<CheepDTO> Cheeps { get; set; }
    public List<AuthorDTO> Followers { get; private set; } = new();
    public int TotalAuthorPages { get; private set; }
    public int CurrentPage;

    private List<string> CreateTags()
    {
        List<string> tags = [];
        var matches = TagRegex.Matches(Message);
        foreach (Match match in matches)
        {
            tags.Add(match.Value.Normalize());
        }
        return tags;
    } 
    
    [BindProperty]
    [Required(ErrorMessage = "Please enter a Cheep!")]
    [StringLength(160, ErrorMessage = "Cheeps cannot exceed 160 characters.")]
    public string Message { get; set; } = string.Empty;
    public async Task<ActionResult> OnPostCheepAsync()
    {
        var author = User.Identity!.Name;
        if (!ModelState.IsValid)
        {
            await LoadAuthorCheeps(author!);
            return Page();
        }
        var tags = CreateTags();
        await _cheepService.AddNewCheep(author!, Message, tags);
        return RedirectToPage();
    }
    
    private async Task LoadAuthorCheeps(string author)
    {
        _authorService.CurrentPage = 1;
        Cheeps = await _authorService.GetAuthorCheeps(author, null);
        Followers = await _authorService.GetFollowsList(User.Identity!.Name!);
        TotalAuthorPages = await _authorService.GetTotalAuthorCheeps(author, null);
        CurrentPage = _authorService.CurrentPage;
    }
    
    public async Task<ActionResult> OnPostFollowAsync(string authorToFollow)
    {
        var follower = User.Identity!.Name;
        await _authorService.AddAuthorToFollowsList(authorToFollow, follower!);
        
        var follows = await _authorService.GetFollowsList(follower!);
        Console.WriteLine($"[{DateTime.Now}] {follower} now follows: {string.Join(",", follows.Select(a => a.Name))}");
        return RedirectToPage();
    }
    
    public async Task<ActionResult> OnPostUnfollowAsync(string authorToUnfollow)
    {
        var follower = User.Identity!.Name;
        await _authorService.RemoveAuthorFromFollowsList(authorToUnfollow, follower!);
        
        Console.WriteLine($"[{DateTime.Now}] {follower} now unfollows: {authorToUnfollow}");
        return RedirectToPage();
    }

    public async Task<ActionResult> OnGetAsync(string author)
    {
        try
        {
            int pageQuery = Request.Query.ContainsKey("page") ? Convert.ToInt32(Request.Query["page"]) : 1;
            if (pageQuery < 1) throw new ArgumentOutOfRangeException();
            List<string?> tags = Request.Query.ContainsKey("tags") ? Request.Query["tag"].ToList()! : [];
            _authorService.CurrentPage = pageQuery;
            Cheeps = await _authorService.GetAuthorCheeps(author, tags!);
            Followers = await _authorService.GetFollowsList(User.Identity!.Name!);
            TotalAuthorPages = await _authorService.GetTotalAuthorCheeps(author, tags!);
            CurrentPage = pageQuery;
        }    catch (FormatException)
        {
            return BadRequest($"Invalid page query. Page query provided: {Request.Query["page"]}");
        }
        catch (Exception e) when (e is ArgumentOutOfRangeException or OverflowException)
        {
            return BadRequest($"Page query '{Request.Query["page"]}' is out of range: 1:{Int32.MaxValue}.");
        }
        return Page();
    }
}
