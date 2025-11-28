namespace Chirp.Core;

public class CheepDTO
{
    public string UserName { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string TimeStamp { get; set; } = null!;

    public List<string>? Tags { get; set; } = [];
}