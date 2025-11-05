using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Services;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public PublicModel(ICheepService service)
    {
        _service = service;
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
        // TODO replace hardcoded author string with user identity
        // var author = User.Identity.Name;
        var author = "Helge";
        if (!ModelState.IsValid)
        {
            await LoadCheeps();
            return Page();
        }
        await _service.AddNewCheep(author, Message);
        return RedirectToPage();
    }
    private async Task LoadCheeps()
    {
        _service.CurrentPage = 1;
        Cheeps = await _service.GetCheeps();
        TotalPages = await _service.GetTotalCheeps();
        CurrentPage = _service.CurrentPage;
    }

    public async Task<ActionResult> OnGetAsync()
    {
        try
        {
            int pageQuery = Request.Query.ContainsKey("page") ? Convert.ToInt32(Request.Query["page"]) : 1;
            if (pageQuery < 1) throw new ArgumentOutOfRangeException();
            
            _service.CurrentPage = pageQuery;
            Cheeps = await _service.GetCheeps();
            TotalPages = await _service.GetTotalCheeps();
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