using static Chirp;

/// <summary>
/// <c>UserInterface</c> displays in terminal, the Chirp application and actions taken within
/// </summary>
public static class UserInterface
{
    /// <summary>
    /// Prints a collection <c>IEnumerable{Cheep}</c> of cheeps to terminal in the cheep format
    /// </summary>
    /// <param name="cheeps">representing a collection of cheep messages, each with author, message and unix timestamp</param>
    internal static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (var cheep in cheeps)
        {
            PrintCheeps(cheep);
        }
    }

    /// <summary>
    /// Prints a single cheep to terminal in the cheep format
    /// </summary>
    /// <param name="cheep">representing a cheep message with author, message and unix timestamp</param>
    internal static void PrintCheeps(Cheep cheep)
    {
        Console.WriteLine($"{cheep.Author} @ {DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToLocalTime()}: {cheep.Message}");
    }
}