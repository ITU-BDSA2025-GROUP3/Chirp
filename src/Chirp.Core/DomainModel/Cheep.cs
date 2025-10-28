﻿using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DomainModel;

public class Cheep
{
    public int CheepId { get; set; }
    
    [Required]
    [StringLength(160)]
    public required string Text { get; set; }
    
    [Required]
    public required DateTime TimeStamp { get; set; }
    
    [Required]
    [StringLength(200)]
    public required string AuthorId { get; set; }
    
    [Required]
    public required Author Author { get; set; }
    
    
}