using Chirp.Core;
using Chirp.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly ICommentService _commentService;
    public PublicModel(ICheepService cheepService, IAuthorService authorService, ICommentService commentService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _commentService = commentService;
    }
    public required List<CheepDTO> Cheeps { get; set; }
    public List<CommentDTO> Comments { get; set; } = new();
    public List<AuthorDTO> Followers { get; private set; } = new();
    public int TotalPages { get; private set; }
    public int CurrentPage;
    
    [BindProperty]
    [Required(ErrorMessage = "Please enter a Cheep!")]
    [StringLength(160, ErrorMessage = "Cheeps cannot exceed 160 characters.")]
    public string Message { get; set; } = string.Empty;
    public async Task<ActionResult> OnPostCheepAsync()
    {
        var author = User.Identity!.Name;
        if (!ModelState.IsValid)
        {
            await LoadCheeps();
            return Page();
        }
        await _cheepService.AddNewCheep(author!, Message);
        return RedirectToPage();
    }
    private async Task LoadCheeps()
    {
        _cheepService.CurrentPage = 1;
        Cheeps = await _cheepService.GetCheeps();
        Comments = await _commentService.GetComments();
        Followers = await _authorService.GetFollowsList(User.Identity!.Name!);
        TotalPages = await _cheepService.GetTotalCheeps();
        CurrentPage = _cheepService.CurrentPage;
    }

    [BindProperty] 
    public int CommentTargetId { get; set; }
    [BindProperty] 
    [Required(ErrorMessage = "Please enter a Comment!")]
    [StringLength(160, ErrorMessage = "Comments cannot exceed 160 characters.")]
    public string Comment { get; set; } = string.Empty;

    // TODO add comment and display under cheep commented on
    public async Task<ActionResult> OnPostCommentFormAsync(int cheepId)
    {
        CommentTargetId = cheepId;
        var author = User.Identity!.Name;
        await _commentService.AddNewComment(author!, Comment, cheepId);
        await LoadCheeps();
        return Page();
    }
    
    public async Task<ActionResult> OnPostShowCommentsAsync(int cheepId)
    {
        CommentTargetId = cheepId;
        if (!ModelState.IsValid)
        {
            await LoadCheeps();
        }
        await LoadCheeps();
        return Page();
    }
    
    public async Task<ActionResult> OnPostFollowAsync(string authorToFollow)
    {
        var follower = User.Identity!.Name;
        await _authorService.AddAuthorToFollowsList(authorToFollow, follower!);
        
        var follows = await _authorService.GetFollowsList(follower!);
        Console.WriteLine($"[{DateTime.Now}] {follower} now follows: {string.Join(",", follows.Select(a => a.Name))}");
        return RedirectToPage();
    }
    
    public async Task<ActionResult> OnPostUnfollowAsync(string authorToUnfollow)
    {
        var follower = User.Identity!.Name;
        await _authorService.RemoveAuthorFromFollowsList(authorToUnfollow, follower!);
        
        Console.WriteLine($"[{DateTime.Now}] {follower} now unfollows: {authorToUnfollow}");
        return RedirectToPage();
    }

    public async Task<ActionResult> OnGetAsync()
    {
        try
        {
            int pageQuery = Request.Query.ContainsKey("page") ? Convert.ToInt32(Request.Query["page"]) : 1;
            if (pageQuery < 1) throw new ArgumentOutOfRangeException();
            
            _cheepService.CurrentPage = pageQuery;
            Cheeps = await _cheepService.GetCheeps();
            Comments = await _commentService.GetComments();
            Followers = await _authorService.GetFollowsList(User.Identity!.Name!);
            TotalPages = await _cheepService.GetTotalCheeps();
            CurrentPage = pageQuery;
        }
        catch (FormatException)
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
