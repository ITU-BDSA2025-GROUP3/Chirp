using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Core.DomainModel;

[Index(nameof(Tags))]
public class Cheep
{
    public int CheepId { get; set; }
    
    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    
    [Required]
    public required DateTime TimeStamp { get; set; }
    
    [Required]
    public required int IdOfAuthor { get; set; }
    [Required]
    public required Author Author { get; set; }
    public List<string>? Tags { get; set; }
}