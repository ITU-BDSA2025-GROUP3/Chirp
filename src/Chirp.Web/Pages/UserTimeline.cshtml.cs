using Chirp.Core;
using Chirp.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages;

public class UserTimelineModel(IAuthorService service) : PageModel
{
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
        // var author = User.Identity.Name;
        var author = "Helge";
        if (!ModelState.IsValid)
        {
            await LoadAuthorCheeps(author);
            return Page();
        }
        await _cheepService.AddNewCheep(author, Message);
        return RedirectToPage();
    }
    
    private async Task LoadAuthorCheeps(string author)
    {
        service.CurrentPage = 1;
        Cheeps = await service.GetAuthorCheeps(author);
        TotalAuthorPages = await service.GetTotalAuthorCheeps(author);
        CurrentPage = service.CurrentPage;
    }

    public async Task<ActionResult> OnGetAsync(string author)
    {
        try
        {
            int pageQuery = Request.Query.ContainsKey("page") ? Convert.ToInt32(Request.Query["page"]) : 1;
            if (pageQuery < 1) throw new ArgumentOutOfRangeException();
            service.CurrentPage = pageQuery;
            Cheeps = await service.GetAuthorCheeps(author);
            TotalAuthorPages = await service.GetTotalAuthorCheeps(author);
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
