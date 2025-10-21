using System.ComponentModel.DataAnnotations;

namespace Chirp.Web.DomainModel;

public class Author
{
    public required int AuthorId { get; set; }
    
    [Required]
    public required string Name { get; set; }
    
    [Required]
    public required string Email { get; set; }
    public required ICollection<Cheep> Cheeps { get; set; } =  new List<Cheep>();
}