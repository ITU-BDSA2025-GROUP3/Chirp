namespace Chirp.Core;

public class CommentDTO
{
    public string UserName { get; set; } = null!;
    public string Comment { get; set; } = null!;
    public string TimeStamp { get; set; } = null!;
    public int CheepId { get; set; } = 0;
    
}