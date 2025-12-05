namespace Chirp.Core;

/// <summary>
/// Data transfer object representation of the Author object, consisting of the Username (Name) and the email associated with an Author.
/// </summary>
public class AuthorDTO
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}