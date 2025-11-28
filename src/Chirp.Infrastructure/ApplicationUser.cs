using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure;

public class ApplicationUser : IdentityUser
{
    
    [Required]
    [StringLength(100)]
    public required string Firstname { get; set; }
    
    [StringLength(100)]
    public string? Surname { get; set; }
    
}