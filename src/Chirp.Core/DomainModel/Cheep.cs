using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DomainModel;

/// <summary>
/// Cheep represents the messages made by Authors in the Chirp Application and these are at most 160 characters long.
/// A timestamp of the date and time the Cheep was made, as well as a reference to the id of the Author who made the Cheep
///    /// are provided.
/// </summary>
public class Cheep
{
    public int CheepId { get; set; }
    
    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    
    [Required]
    public required DateTime TimeStamp { get; set; }
    
    [Required]
    public required int IdOfAuthor { get; set; }
    [Required]
    public required Author Author { get; set; }
}