using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.DomainModel;

public class Author : IdentityUser
{
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    public required ICollection<Cheep> Cheeps { get; set; } =  new List<Cheep>();
}