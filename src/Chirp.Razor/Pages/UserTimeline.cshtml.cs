using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public required List<CheepDTO> CheepsName { get; set; }
    public required List<CheepDTO> CheepsEmail { get; set; }
    public int TotalAuthorPages { get; private set; }
    public int CurrentPage;
    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnGetAsync(string author)
    {
        try
        {
            int pageQuery = Request.Query.ContainsKey("page") ? Convert.ToInt32(Request.Query["page"]) : 1;
            if (pageQuery < 1) throw new ArgumentOutOfRangeException();
            _service.CurrentPage = pageQuery;
            CheepsName = await _service.GetCheepsFromAuthor(author);
            CheepsEmail = await _service.GetCheepsFromAuthorEmail(author);
            TotalAuthorPages = await _service.GetTotalCheepsFromAuthor(author);
            CurrentPage = pageQuery;
        }    catch (FormatException e)
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
