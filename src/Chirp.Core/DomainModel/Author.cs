using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.DomainModel;

public class Author : IdentityUser<int>
{
    //public int AuthorId { get; set; }
    
    [Required]
    [StringLength(100)]
    public override required string UserName { get; set; }
    
    [Required]
    [StringLength(100)]
    public override required string Email { get; set; }
    public required ICollection<Cheep> Cheeps { get; set; } =  new List<Cheep>();
    public required ICollection<Author> Follows { get; set; } =  new List<Author>();
}