using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Services;

using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;

public class _SubmitMessage : PageModel
{
    
    private readonly ICheepService _service;
    
    [BindProperty]
    [Required]
    [StringLength(160, ErrorMessage = "Maximum length is {1}")]
    [Display(Name = "Message Text")]
    public string Message { get; set; }
    public string Author { get; set; }

    public _SubmitMessage(ICheepService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnPostAsync()
    {
        // TODO replace hardcoded author string with user identity
        // Author = User.Identity.Name;
        Author = "Helge";
        if (!ModelState.IsValid)
        {
            return Page();
        }
        await _service.AddNewCheep(Author, Message);
        return RedirectToPage();
    }
}