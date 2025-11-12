using Chirp.Core;
using Chirp.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    public UserTimelineModel(ICheepService cheepService, IAuthorService authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
    }
    
    public required List<CheepDTO> Cheeps { get; set; }
    public int TotalAuthorPages { get; private set; }
    public int CurrentPage;
    
    [BindProperty]
    [Required(ErrorMessage = "Please enter a Cheep!")]
    [StringLength(160, ErrorMessage = "Cheeps cannot exceed 160 characters.")]
    public string Message { get; set; } = string.Empty;
    public async Task<ActionResult> OnPostAsync()
    {
        // TODO replace hardcoded author string with user identity
        var author = User.Identity!.Name;
        if (!ModelState.IsValid)
        {
            await LoadAuthorCheeps(author!);
            return Page();
        }
        await _cheepService.AddNewCheep(author!, Message);
        return RedirectToPage();
    }
    
    private async Task LoadAuthorCheeps(string author)
    {
        _authorService.CurrentPage = 1;
        Cheeps = await _authorService.GetAuthorCheeps(author);
        TotalAuthorPages = await _authorService.GetTotalAuthorCheeps(author);
        CurrentPage = _authorService.CurrentPage;
    }
    
    [BindProperty]
    public string Follower { get; set; } = string.Empty;
    public void OnGetFollow()
    {
        var author = User.Identity!.Name;
        _authorService.AddAuthorToFollowsList(Follower, author!);
    }
    public void OnGetUnfollow()
    {
        var author = User.Identity!.Name;
        _authorService.RemoveAuthorFromFollowsList(Follower, author!);
    }

    public async Task<ActionResult> OnGetAsync(string author)
    {
        try
        {
            int pageQuery = Request.Query.ContainsKey("page") ? Convert.ToInt32(Request.Query["page"]) : 1;
            if (pageQuery < 1) throw new ArgumentOutOfRangeException();
            _authorService.CurrentPage = pageQuery;
            Cheeps = await _authorService.GetAuthorCheeps(author);
            TotalAuthorPages = await _authorService.GetTotalAuthorCheeps(author);
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
