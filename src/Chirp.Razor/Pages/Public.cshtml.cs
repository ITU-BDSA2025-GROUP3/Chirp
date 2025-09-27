using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic.CompilerServices;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        try
        {
            int pageQuery = Convert.ToInt32(Request.Query["page"]);
            if (pageQuery < 1) throw new ArgumentOutOfRangeException();
            //TODO use a get method on _service for sql querying the next 32 cheeps, offset by pageQuery - 1
            _service.GetCheeps();
        }
        catch (FormatException e)
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
