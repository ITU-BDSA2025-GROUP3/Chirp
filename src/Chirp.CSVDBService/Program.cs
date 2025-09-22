using Microsoft.AspNetCore.Builder;

namespace CSVDBService;

public class Program
{
    private static CSVDatabase<Cheep> database = CSVDatabase<Cheep>.Instance;
    
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        app.MapGet("/cheeps", () =>
        {
            return database.Read();
        });
        app.MapPost("/cheep", (Cheep userCheep) => database.Store(userCheep));

        Console.WriteLine("Starting CSVDB service...!");
        app.Run();
        
    }

    public static CSVDatabase<Cheep> GetDatabase()
    {
        return database;
    }
    
    public record Cheep
    {
        public required string Author { get; set; }
        public required string Message { get; set; }
        public long Timestamp { get; set; }
    }
    
}