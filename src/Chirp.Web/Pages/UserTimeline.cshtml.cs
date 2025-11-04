using Chirp.Infrastructure;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly IAuthorService _authorService;
    private readonly ICheepService _cheepService;
    public required List<CheepDTO> Cheeps { get; set; }
    public int TotalAuthorPages { get; private set; }
    public int CurrentPage;
    public UserTimelineModel(IAuthorService authorService, ICheepService cheepService)
    {
        _authorService = authorService;
        _cheepService = cheepService;
    }
    
    [BindProperty]
    [Required(ErrorMessage = "Please enter a Cheep!")]
    [StringLength(160, ErrorMessage = "Cheeps cannot exceed 160 characters.")]
    public string Message { get; set; } = string.Empty;
    public async Task<ActionResult> OnPostAsync()
    {
        // TODO replace hardcoded author string with user identity
        // var author = User.Identity.Name;
        var author = "Helge";
        if (!ModelState.IsValid)
        {
            return Page();
        }
        await _cheepService.AddNewCheep(author, Message);
        return RedirectToPage();
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
