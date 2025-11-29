using System.IO.Compression;
using System.Text;
using Chirp.Core;
using Chirp.Core.ServiceInterfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


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
        var userNameOrEmail = HttpContext.User.Identity!.Name!; 
        CurrentAuthor = await _authorService.GetAuthor(userNameOrEmail);
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
        var userName = HttpContext.User.Identity!.Name!;
        var cheeps = await _authorService.GetMyCheeps(userName);
        Cheeps = cheeps.ToList();
    }

    public async Task<IActionResult> OnGetDownloadAsync()
    {
        await getUserNameAndEmail();
        await getFollowList();
        await getCheepsList();

        using var stream = new MemoryStream();

        using (var zip = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Username and email text file
            var userInfo = zip.CreateEntry("myInfo.txt");
            using (var streamWriter = new StreamWriter(userInfo.Open(), Encoding.UTF8))
                if (CurrentAuthor != null)
                {
                    streamWriter.WriteLine($"Name: {CurrentAuthor.Name}");
                    streamWriter.WriteLine($"Email: {CurrentAuthor.Email}");
                }
                else
                {
                    streamWriter.WriteLine("There is no information found.");
                }
            // Users follow list
            var userFollowList = zip.CreateEntry("myFollowList.txt");
            using (var streamWriter = new StreamWriter(userFollowList.Open(), Encoding.UTF8))
            {
                foreach (var follower in Followers)
                {
                    streamWriter.WriteLine($"Name: {follower.Name}");
                }
            }

            // Users previous cheeps text file
            var userCheeps = zip.CreateEntry("previousCheeps.txt");
            using (var streamWriter = new StreamWriter(userCheeps.Open(), Encoding.UTF8))
            {
                foreach (var cheep in Cheeps)
                {
                    streamWriter.WriteLine($"{cheep.TimeStamp}: {cheep.Message}");
                }
            }
        }
        var bytes = stream.ToArray();
        var filename = "userData.zip";
        return File(bytes, "application/zip", filename);
    }

    public async Task<IActionResult> OnPostForgetMeAsync()
    {
        //Get the username for deletion
        var userNameOrEmail = HttpContext.User.Identity!.Name!; 
        //todo Sign the user out
        //Delete the user
        await _authorService.DeleteAuthor(userNameOrEmail);
        
        //Return to home page after logged out
        return LocalRedirect("/");
        
    }
}