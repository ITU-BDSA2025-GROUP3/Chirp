using System.IO.Compression;
using System.Text;
using Chirp.Core;
using Chirp.Core.DomainModel;
using Chirp.Core.ServiceInterfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace Chirp.Web.Pages;

 [Authorize]
public class InformationModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly ICommentService _commentService;
    private readonly SignInManager<Author> _signInManager;
    private readonly ILogger<InformationModel> _logger;
    
    public InformationModel(ICheepService cheepService, IAuthorService authorService, ICommentService commentService, SignInManager<Author> signInManager, ILogger<InformationModel> logger)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _commentService = commentService;
        _signInManager = signInManager;
        _logger = logger;
    }
    
    
    public AuthorDTO? CurrentAuthor { get; set; }
    public List<CheepDTO> Cheeps { get; set; } = new();
    
    public List<AuthorDTO> Followers { get; set; } = new();
    public List<CommentDTO> Comments { get; set; } = new();

    public async Task OnGetAsync()
    {
        await getUserNameAndEmail();
        await getFollowList();
        await getCheepsList();
        await getCommentsList();
    }

    public async Task getUserNameAndEmail()
    {
        var userNameOrEmail = HttpContext.User.Identity!.Name!; 
        CurrentAuthor = await _authorService.GetAuthor(userNameOrEmail);
    }
    
    // link to other user this user is fowllowing
    public async Task getFollowList()
    {
        var userName = HttpContext.User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(userName))
        {
            Followers = new List<AuthorDTO>();
            return;
        }
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

    public async Task getCommentsList()
    {
        var userName = HttpContext.User.Identity!.Name!;
        Console.WriteLine("User  with username: " +  userName);
        var comments = await _commentService.GetCommentsListFromUser(userName);
        Console.WriteLine("Comments: " + comments.Count);
        Comments = comments.ToList();
        
    }

    public async Task<IActionResult> OnGetDownloadAsync()
    {
        await getUserNameAndEmail();
        await getFollowList();
        await getCheepsList();
        await getCommentsList();

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
            
            // Users previous comments text file
            var userComments = zip.CreateEntry("previousComments.txt");
            using (var streamWriter = new StreamWriter(userComments.Open(), Encoding.UTF8))
            {
                foreach (var comment in Comments)
                {
                    streamWriter.WriteLine($"{comment.CheepId} {comment.TimeStamp}: {comment.Comment}");
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
        
        //Sign out the user
        await _signInManager.SignOutAsync();
        
        //Delete the user
        await _authorService.DeleteAuthor(userNameOrEmail);
        
        _logger.LogInformation("User has been deleted!");
        
        //Return to home page after logged out
        return LocalRedirect("/");
        
    }
}