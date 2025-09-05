// See https://aka.ms/new-console-template for more information

using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using SimpleDB;

internal class Chirp
{
    private static IDatabaseRepository<Cheep> database = new CSVDatabase<Cheep>();
    public static void Main(string[] args)
    {
        
        if (args[0] == "cheep")
        {
            // EXIT IF NO MSG AFTER '-- CHEEP'
            if (args.Length < 2)
            {
                Console.WriteLine("incorrect usage, write a message");
                Environment.Exit(1);
            }
            string msg = args[1];
            
            // STORE NEW MSG IN CSV FILE
            var cheep = stringToCheep(msg);
            database.Store(cheep);
            
        }

        if (args[0] == "read")
        {
            // IF READ JUST PRINT FROM CSV FILE
            // PrintCheep();
            UserInterface.PrintCheeps(database.Read());
            
        }
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