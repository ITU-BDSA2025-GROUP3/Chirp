using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.DomainModel;

public class Author : IdentityUser
{
    public required int AuthorId { get; set; }
    
    [Required]
    [StringLength(100)]
    public override required string UserName { get; set; }
    
    [Required]
    [StringLength(100)]
    public override required string Email { get; set; }
    public required ICollection<Cheep> Cheeps { get; set; } =  new List<Cheep>();
}