// See https://aka.ms/new-console-template for more information

using SimpleDB;
using System.CommandLine;
using System.Runtime.CompilerServices;

//Make class visible to test directory
[assembly:InternalsVisibleTo("Chirp.Intergration.Tests")]

class Chirp //Modifier should NOT be public, need proper internal visibility status!
{
    private static IDatabaseRepository<Cheep> database = CSVDatabase<Cheep>.Instance;

    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand("chirp");

        //Create the read command
        var readCommand = new Command("read", "Read cheeps and display to terminal");
        var readLimit = new Argument<int>("limit")
        {
            Description = "Limit on the number of latest cheeps to read",
            DefaultValueFactory = parseResult => 5
        };
        readCommand.Arguments.Add(readLimit);
        rootCommand.Subcommands.Add(readCommand);
        readCommand.SetAction(parseResult =>
        {
            UserInterface.PrintCheeps(database.Read(parseResult.GetValue(readLimit)));
        });
        //Create the cheep command
        var cheepCommand = new Command("cheep", "Cheep a message");
        var cheepArgument = new Argument<string>("message") {Description = "The message to be cheeped"};
        
        cheepCommand.Arguments.Add(cheepArgument);
        rootCommand.Subcommands.Add(cheepCommand);
        
        cheepCommand.SetAction(parseResult =>
        {
            var message = parseResult.GetValue(cheepArgument)!; // '!' used to assert non-null msg val
            Cheep cheep = stringToCheep(message);
            database.Store(cheep);
            UserInterface.PrintCheeps(cheep);
        });
        

        return rootCommand.Parse(args).Invoke();
    }

    private static Cheep stringToCheep(string msg)
    {
        var record = new Cheep
        {
            Author = Environment.UserName,
            Message = msg,
            Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
        };

        return record;
    }

    public record Cheep
    {
        public required string Author { get; set; }
        public required string Message { get; set; }
        public long Timestamp { get; set; }
    }
}