using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor.DomainModel;

public class Cheep
{
    public int CheepId { get; set; }
    
    [Required]
    [StringLength(500)]
    public required string Text { get; set; }
    public DateTime TimeStamp { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
    
    
}