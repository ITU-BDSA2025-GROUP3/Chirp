namespace Chirp.Core.ServiceInterfaces;

public class CommentDTO
{
    public string UserName { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string TimeStamp { get; set; } = null!;
}