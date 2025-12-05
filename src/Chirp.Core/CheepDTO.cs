namespace Chirp.Core;

/// <summary>
/// Data transfer object representation of Cheeps, containing the username of the Author, the message content of the cheep, timestamp and id of the Cheep
/// </summary>
public class CheepDTO
{
    public string UserName { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string TimeStamp { get; set; } = null!;
    public int CheepId { get; set; } = 0;
}