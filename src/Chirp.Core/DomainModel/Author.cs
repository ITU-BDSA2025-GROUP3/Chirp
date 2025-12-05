using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.DomainModel;

/// <summary>
/// Author represents a normal user of the Chirp Application and is also the identity of the user in the EF framework.
/// An Author must have a unique username and email. The username is also the .Name of the Identity in the razor pages.
/// The username and email of the Author must be at most 100 characters long.
/// An Author has zero or more Cheep messages they have authored.
/// An Author has zero or more other Authors that they follow, making those Authors' Cheeps visible in this Author's private timeline.
/// </summary>
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