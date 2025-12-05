using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DomainModel;

/// <summary>
/// Comment represents messages made as a comment left on a Cheep message by an Author, which may be different from the
/// Author of the original Cheep. These Comment objects contain a reference to the id of the original Cheep and the id
/// of the Author who made the comment. The Comment object also contain a timestamp of the date and time the comment was
/// made. Comment messages are at most 160 character long.
/// </summary>
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
}