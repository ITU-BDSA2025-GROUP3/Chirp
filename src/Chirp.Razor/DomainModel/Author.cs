using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor.DomainModel;

public class Author
{
    public required int AuthorId { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Email { get; set; }
    public required ICollection<Cheep> Cheeps { get; set; } =  new List<Cheep>();
}