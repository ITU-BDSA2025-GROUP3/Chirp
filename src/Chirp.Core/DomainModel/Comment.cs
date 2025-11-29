using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DomainModel;

public class Comment
{
    public int CheepId { get; set; }
    public int CommentId { get; set; }
    
    [Required]
    [StringLength(160)]
    public required string Message { get; set; }
    
    [Required]
    public required DateTime TimeStamp { get; set; }
    
    [Required]
    public required int IdOfAuthor { get; set; }
    [Required]
    public required Author Author { get; set; }
    [Required]
    public required string CommentText { get; set; }
}