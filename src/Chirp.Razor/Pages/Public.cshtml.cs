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
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 2;
    public int Count { get; set; }
    
    public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

    public List<CheepViewModel> Cheeps { get; set; }
    
    public void OnGet()
    {
        Count = _service.GetCount();
        if (CurrentPage < 1) CurrentPage = 1;
        if (TotalPages > 0 && CurrentPage > TotalPages) CurrentPage = TotalPages;
        Cheeps = _service.GetPaginatedResult(CurrentPage, PageSize);
    }

    // public ActionResult OnGet()
    // {
    //     Cheeps = _service.GetCheeps();
    //     return Page();
    // }
}
