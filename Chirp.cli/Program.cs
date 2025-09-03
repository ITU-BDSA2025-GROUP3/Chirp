// See https://aka.ms/new-console-template for more information

using Microsoft.VisualBasic.FileIO;

internal class Chirp
{
    static string filePath = "chirp_cli_db.csv";
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
            
            // APPEND CHIRP MESSAGES TO CSV FILE
            using(var sw = new StreamWriter(filePath, append: true))
            {
                var record = new ChripMsg
                {
                    Author = Environment.UserName,
                    Message = msg,
                    Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
                };
            
                string line = $"{(record.Author)},\"{(record.Message)}\",{record.Timestamp}";
                sw.WriteLine(line);
            }
            // PRINT CONTENTS FROM CSV FILE
            PrintCheep();
        }

        if (args[0] == "read")
        {
            // IF READ JUST PRINT FROM CSV FILE
            PrintCheep();
        }
    }

    public static void PrintCheep()
    {
        // READ ONLY MESSAGE COLUMN 
        using (var parser = new TextFieldParser(filePath))
        {
            parser.SetDelimiters(","); // Fields are comma seperated
            parser.HasFieldsEnclosedInQuotes = true; // Embedded commas: "Hi, this is a message" are treated as one field

            // skip header
            _ = parser.ReadFields(); // Read first field and discard w/ discard pattern (_=)

            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields(); // [Author, Message, Timestamp]
                if (fields is { Length: > 1 }) // only proceed if there is a second column w/ message
                    Console.WriteLine(fields[1]);  // Message only 
            }
        }
    }

    public record ChripMsg
    {
        public required string Author { get; set; }
        public required string Message { get; set; }
        public long Timestamp { get; set; }
    }
}