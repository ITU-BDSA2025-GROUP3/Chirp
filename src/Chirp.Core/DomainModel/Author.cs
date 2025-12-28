using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.DomainModel;

public class Author : IdentityUser<int>
{
    //public int AuthorId { get; set; }

    [Required]
    [StringLength(100)]
    public override string? UserName
    {
        get => base.UserName;
        set => base.UserName = value;
    }

    [Required]
    [StringLength(100)]
    public override string? Email
    {
        get => base.Email;
        set => base.Email = value;
    }
    public required ICollection<Cheep> Cheeps { get; set; } =  new List<Cheep>();
    public required ICollection<Author> Follows { get; set; } =  new List<Author>();
}