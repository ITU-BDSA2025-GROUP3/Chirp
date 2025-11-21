using Chirp.Core.ServiceInterfaces;

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class InformationModel(IAuthorService authorService, ICheepService cheepService) : PageModel
{
    public required List<CheepDTO> Cheeps { get; set; }
    
    // Show name
    // email
    // link to other user this user is fowllowing
    // all cheeps of the user should be listed
    // download button
    
}