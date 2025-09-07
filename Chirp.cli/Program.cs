// See https://aka.ms/new-console-template for more information

using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using SimpleDB;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

class Chirp
{
    private static IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();

    public static int Main(string[] args)
    {
        RootCommand rootCommand = new RootCommand("chirp");

        //Create the read command
        Command readCommand = new Command("read", "Read cheeps and display to terminal");
        //optional argument of the number of cheeps to read TODO not yet implemented a limit
        Argument<int> readLimit = new Argument<int>("limit")
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
        Command cheepCommand = new Command("cheep", "Cheep a message");
        Argument<string> cheepArgument = new Argument<string>("message") {Description = "The message to be cheeped"};
        
        cheepCommand.Arguments.Add(cheepArgument);
        rootCommand.Subcommands.Add(cheepCommand);
        
        cheepCommand.SetAction(parseResult =>
        {
            Cheep cheep = stringToCheep(parseResult.GetValue(cheepArgument));
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