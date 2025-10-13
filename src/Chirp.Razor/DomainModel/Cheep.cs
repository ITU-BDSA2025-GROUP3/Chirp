using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor.DomainModel;

public class Cheep
{
    public int CheepId { get; set; }
    
    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    
    [Required]
    public required DateTime TimeStamp { get; set; }
    
    [Required]
    public required int AuthorId { get; set; }
    [Required]
    public required Author Author { get; set; }
    
    
}