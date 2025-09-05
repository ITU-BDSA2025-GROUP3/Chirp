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
            
            // STORE NEW MSG IN CSV FILE
            StoreCheep(msg);
            // PRINT CONTENTS FROM CSV FILE
            PrintCheep();
        }

        if (args[0] == "read")
        {
            // IF READ JUST PRINT FROM CSV FILE
            PrintCheep();
        }
    }
    
    // APPEND CHIRP MESSAGES TO CSV FILE
    private static void StoreCheep(string msg)
    {
        // APPEND CHIRP MESSAGES TO CSV FILE
        using(var sw = new StreamWriter(filePath, append: true))
        {
            var record = new ChirpMsg
            {
                Author = Environment.UserName,
                Message = msg,
                Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
            };
            
            string line = $"{(record.Author)},\"{(record.Message)}\",{record.Timestamp}";
            sw.WriteLine(line);
        }
    }

    private static void PrintCheep()
    {
        using (var parser = new TextFieldParser(filePath))
        {
            parser.SetDelimiters(","); // Fields are comma seperated
            parser.HasFieldsEnclosedInQuotes = true; // Embedded commas: "Hi, this is a message" are treated as one field

            // skip header so [author, message, Timestamp]
            _ = parser.ReadFields(); // Read first field and discard w/ discard pattern (_=)
            
            while (!parser.EndOfData)
            {
                Console.WriteLine(parser.ReadToEnd());
                // ONLY READ MESSAGE FIELD -->
                // var fields = parser.ReadFields(); // [Author, Message, Timestamp]
                // if (fields is { Length: > 1 }) // only proceed if there is a second column w/ message
                //     Console.WriteLine(fields[1]);  // Message only 
            }
        }
    }

    public record ChirpMsg
    {
        public required string Author { get; set; }
        public required string Message { get; set; }
        public long Timestamp { get; set; }
    }
}