using Chirp.Core;
using Chirp.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

using Chirp.Core.RepositoryInterfaces;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    public PublicModel(ICheepService cheepService, IAuthorService authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
    }
    public required List<CheepDTO> Cheeps { get; set; }
    public int TotalPages { get; private set; }
    public int CurrentPage;
    
    [BindProperty]
    [Required(ErrorMessage = "Please enter a Cheep!")]
    [StringLength(160, ErrorMessage = "Cheeps cannot exceed 160 characters.")]
    public string Message { get; set; } = string.Empty;
    public async Task<ActionResult> OnPostAsync()
    {
        var author = User.Identity!.Name;
        if (!ModelState.IsValid)
        {
            await LoadCheeps();
            return Page();
        }
        await _cheepService.AddNewCheep(author!, Message);
        return RedirectToPage();
    }
    private async Task LoadCheeps()
    {
        _cheepService.CurrentPage = 1;
        Cheeps = await _cheepService.GetCheeps();
        TotalPages = await _cheepService.GetTotalCheeps();
        CurrentPage = _cheepService.CurrentPage;
    }

    [BindProperty]
    public string Follower { get; set; } = string.Empty;
    public async void OnGetFollow()
    {
        if (!ModelState.IsValid)
        {
            await LoadCheeps();
            throw new Exception();
        }
        var author = User.Identity!.Name;
        _authorService.AddAuthorToFollowsList(Follower, author!);
    }
    public async void OnGetUnfollow()
    {
        if (!ModelState.IsValid)
        {
            await LoadCheeps();
            throw new Exception();
        }
        var author = User.Identity!.Name;
        _authorService.RemoveAuthorFromFollowsList(Follower, author!);
    }

    public async Task<ActionResult> OnGetAsync()
    {
        try
        {
            int pageQuery = Request.Query.ContainsKey("page") ? Convert.ToInt32(Request.Query["page"]) : 1;
            if (pageQuery < 1) throw new ArgumentOutOfRangeException();
            
            _cheepService.CurrentPage = pageQuery;
            Cheeps = await _cheepService.GetCheeps();
            TotalPages = await _cheepService.GetTotalCheeps();
            CurrentPage = pageQuery;
        }
        catch (FormatException)
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