using Chirp.Core;
using Chirp.Core.ServiceInterfaces;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel(IAuthorService service) : PageModel
{
    public required List<CheepDTO> Cheeps { get; set; }
    public int TotalAuthorPages { get; private set; }
    public int CurrentPage;

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
