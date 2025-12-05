namespace Chirp.Core;

/// <summary>
/// Data transfer object representation of a Comment as one made on a Cheep.
/// It consists of the username of the Author who made the comment, the content of the comment,
/// the timestamp and the id of the Cheep to which the Comment was made
/// </summary>
public class CommentDTO
{
    public string UserName { get; set; } = null!;
    public string Comment { get; set; } = null!;
    public string TimeStamp { get; set; } = null!;
    public int CheepId { get; set; }
    
}