using Chirp.Core;
using Chirp.Core.ServiceInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly ICommentService _commentService;
    public UserTimelineModel(ICheepService cheepService, IAuthorService authorService, ICommentService commentService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _commentService = commentService;
    }
    
    public required List<CheepDTO> Cheeps { get; set; }
    public List<AuthorDTO> Followers { get; private set; } = new();
    public List<CommentDTO> Comments { get; set; } = new();
    public int TotalAuthorPages { get; private set; }
    public int CurrentPage;
    
    [BindProperty]
    [Required(ErrorMessage = "Please enter a Cheep!")]
    [StringLength(160, ErrorMessage = "Cheeps cannot exceed 160 characters.")]
    public string Message { get; set; } = string.Empty;
    public async Task<ActionResult> OnPostCheepAsync(int page)
    {
        ModelState.Remove(nameof(Comment));
        if (!ModelState.IsValid)
        {
            await LoadAuthorCheeps(User.Identity!.Name!, page);
            return Page();
        }
        await _cheepService.AddNewCheep(User.Identity!.Name!, Message);
        return RedirectToPage();
    }
    
    private async Task LoadAuthorCheeps(string author, int page)
    {
        _authorService.CurrentPage = page;
        Cheeps = await _authorService.GetAuthorCheeps(author);
        Followers = await _authorService.GetFollowsList(User.Identity!.Name!);
        Comments = await _commentService.GetComments();
        TotalAuthorPages = await _authorService.GetTotalAuthorCheeps(author);
        CurrentPage = page;
    }
    
    [BindProperty] 
    public int CommentTargetId { get; set; }
    [BindProperty] 
    [Required(ErrorMessage = "Please enter a Comment!")]
    [StringLength(160, ErrorMessage = "Comments cannot exceed 160 characters.")]
    public string Comment { get; set; } = string.Empty;
    
    public async Task<ActionResult> OnPostCommentFormAsync(string author, int cheepId, int page)
    {
        ModelState.Remove(nameof(Message));
        if (!ModelState.IsValid)
        {
            CommentTargetId = cheepId;
        }
        CommentTargetId = cheepId;
        ModelState.Clear();
        await _commentService.AddNewComment(User.Identity!.Name!, Comment, cheepId);
        await LoadAuthorCheeps(author, page);
        return Page();
    }
    
    public async Task<ActionResult> OnPostToggleCommentsAsync(string author, int cheepId, int page)
    {
        CommentTargetId = CommentTargetId == cheepId ? 0 : cheepId;
        ModelState.Clear();
        await LoadAuthorCheeps(author, page);
        return Page();
    }
    
    public async Task<ActionResult> OnPostFollowAsync(string authorToFollow, int page)
    {
        var follower = User.Identity!.Name;
        await _authorService.AddAuthorToFollowsList(authorToFollow, follower!);
        
        var follows = await _authorService.GetFollowsList(follower!);
        Console.WriteLine($"[{DateTime.Now}] {follower} now follows: {string.Join(",", follows.Select(a => a.Name))}");
        return RedirectToPage();
    }
    
    public async Task<ActionResult> OnPostUnfollowAsync(string authorToUnfollow, int page)
    {
        var follower = User.Identity!.Name;
        await _authorService.RemoveAuthorFromFollowsList(authorToUnfollow, follower!);
        
        Console.WriteLine($"[{DateTime.Now}] {follower} now unfollows: {authorToUnfollow}");
        return RedirectToPage();
    }

    public async Task<ActionResult> OnGetAsync(string author)
    {
        try
        {
            int pageQuery = Request.Query.ContainsKey("page") ? Convert.ToInt32(Request.Query["page"]) : 1;
            if (pageQuery < 1) throw new ArgumentOutOfRangeException();
            _authorService.CurrentPage = pageQuery;
            Cheeps = await _authorService.GetAuthorCheeps(author);
            Followers = await _authorService.GetFollowsList(User.Identity!.Name!);
            Comments = await _commentService.GetComments();
            TotalAuthorPages = await _authorService.GetTotalAuthorCheeps(author);
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