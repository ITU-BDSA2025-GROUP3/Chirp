using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;

    public PublicModel(ICheepService service)
    {
        _service = service;
    }
    
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1; // starts at 1
    public int PageSize { get; set; } = 2; // sets the amount of cheep records per page before page-break
    public int Count { get; set; } // counts how many records there are in total
    
    // total pages needed based on amount of cheeps and cheeps per page
    public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize)); 

    public List<CheepViewModel> Cheeps { get; set; }
    
    public void OnGet()
    {
        Count = _service.GetCount();
        if (CurrentPage < 1) CurrentPage = 1; // guard clause to prevent negative page requests, defaults to page 1
        if (TotalPages > 0 && CurrentPage > TotalPages) CurrentPage = TotalPages; // guard clause to prevent going over total pages, defaults to last page
        Cheeps = _service.GetPaginatedResult(CurrentPage, PageSize);
    }

    // public ActionResult OnGet()
    // {
    //     Cheeps = _service.GetCheeps();
    //     return Page();
    // }
}
