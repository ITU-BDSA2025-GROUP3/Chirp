using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DomainModel;

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
    public required ICollection<Author> Follows { get; set; } =  new List<Author>();
}