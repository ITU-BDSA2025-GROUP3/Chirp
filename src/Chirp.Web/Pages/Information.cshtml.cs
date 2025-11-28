using System.Text;
using Chirp.Core;
using Chirp.Core.ServiceInterfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Chirp.Web.Pages;

 [Authorize]
public class InformationModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;

    public InformationModel(ICheepService cheepService, IAuthorService authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
    }
    
    
    public AuthorDTO? CurrentAuthor { get; set; }
    public List<CheepDTO> Cheeps { get; set; }
    
    public List<AuthorDTO> Followers { get; set; } = new();

    public async Task OnGetAsync()
    {
        await getUserNameAndEmail();
        await getFollowList();
        await getCheepsList();
    }

    public async Task getUserNameAndEmail()
    {
        var userName = HttpContext.User.Identity.Name;
       // CurrentAuthor = await _authorService.GetAuthor(userName);
    }
    
    // link to other user this user is fowllowing
    public async Task getFollowList()
    {
        var userName = HttpContext.User.Identity.Name;
        var list = await _authorService.GetFollowsList(userName);
        Followers = list.ToList();
    }

    // all cheeps of the user should be listed
    public async Task getCheepsList()
    {
        var userName = HttpContext.User.Identity.Name;
        var cheeps = await _authorService.GetAuthorCheeps(userName);
        Cheeps = cheeps.ToList();
    }
    
    // optional
    /*public async Task<IActionResult> OnGetDownloadAsync()
    {
        return await Task.FromResult<IActionResult>(Page());
    }*/
}

